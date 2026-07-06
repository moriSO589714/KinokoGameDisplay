using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

public class GameDlProc
{
    public GameData GameData { get; private set; }

    private OnNetDriveMetaData _onNetDriveMetaData = null;
    private OnNetDriveGetFile _onNetDriveFetFile = null;
    private bool _doingTaskFlag = false;
    private GameDlProgress _gameDlProgress = null;

    /// <summary>
    /// 対象のゲームの現在の状態を表す列挙型
    /// </summary>
    private enum LastGameDLState 
    {
        Completely,//完全にダウンロード済
        ExistSplit,//スライス状態で存在する
        FileError,//ファイルのいずれかにエラーがあり、フォルダを作り直す必要がある
        None//全くダウンロードされていない
    }

    public GameDlProc(OnNetDriveMetaData metaData, OnNetDriveGetFile onNetDriveFetFile, GameData gameData)
    {
        _onNetDriveMetaData = metaData;
        _onNetDriveFetFile = onNetDriveFetFile;
        GameData = gameData;
    }

    public async UniTask DLGameInUniTask(CancellationToken ct, GameDlProgress gameDlProgress = null)
    {
        _doingTaskFlag = true;
        if (gameDlProgress != null)
        {
            _gameDlProgress = gameDlProgress;
        }
        //ダウンロードタスクをスレッドプールで実行
        await UniTask.RunOnThreadPool(DLGame, cancellationToken: ct);
    }

    /// <summary>
    /// 実行中のダウンロードタスクを停止する
    /// </summary>
    public void ForceEndThisProc()
    {
        if (_doingTaskFlag)
        {
            _doingTaskFlag = false;
        }
    }

    /// <summary>
    /// 固有IDからゲームデータをローカルに保存する
    /// </summary>
    public void DLGame()
    {
        string gameId = GameData.GameID;
        string driveId = GameData.GameDriveId;
        AllDirs allDirs = AllDirs.GetInstance();

        //使用するパスの定義
        string tempDirPath = allDirs.TmpDLPath;
        string tempGameDLPath = CreateDirPath.TempGamePathForDl(tempDirPath: tempDirPath, gameId: gameId);
        string tempSlicedGameDLPath = CreateDirPath.TempSlicedGamePathForDl(tempDirPath: tempDirPath, gameId: gameId);

        //保存用一時ディレクトリの作成
        DirectoryActs.CreateAndCheckDir(tempSlicedGameDLPath);
        //前回ダウンロードした形跡がないかを確認する
        LastGameDLState lastGameDLState = CheckLastStatus(gameId, tempSlicedGameDLPath);

        if(lastGameDLState == LastGameDLState.Completely)
        {
            throw new GameDlCustomException("このゲームは既にダウンロードされています", GameDlErrorType.Others);
        }

        if(lastGameDLState == LastGameDLState.FileError)
        {
            DirectoryActs.RefleshDir(tempSlicedGameDLPath);
        }

        //ドライブからメタデータを取得してくる
        Dictionary<string, string> metaDic = _onNetDriveMetaData.GetFileList(driveId);
        Dictionary<string, string> needDlFileDic = new Dictionary<string, string>();

        DLData newDLData = null;

        //スライスされたファイルが既に存在する場合(ダウンロードしかけ)
        if(lastGameDLState == LastGameDLState.ExistSplit)
        {
            MistakeFiles mistakeFiles = null;
            mistakeFiles = new FreezingTools().hasAllRequiredData(tempSlicedGameDLPath);

            string dlDataFilePath = Path.Combine(tempSlicedGameDLPath, mistakeFiles.FileName + new DLData().DLDataInfoFileExtention);
            DLData oldDLData = new DLData(dlDataFilePath);
            newDLData = GetDLDataFromDrive(metaDic, tempSlicedGameDLPath);

            //前回のダウンロードからゲームデータに更新がかけられているかを確認する
            if (CheckUpdated(oldDLData, newDLData))
            {
                DirectoryActs.RefleshDir(tempSlicedGameDLPath);
                newDLData.SerializeDLData(tempSlicedGameDLPath);
                needDlFileDic = metaDic;
            }
            else //アップデートが行われておらずそのまま流用可能
            {
                needDlFileDic = CreateLackDic(mistakeFiles, metaDic);
            }
        }
        else
        {
            newDLData = GetDLDataFromDrive(metaDic, tempSlicedGameDLPath);
            needDlFileDic = metaDic;
        }

        if(_gameDlProgress != null)
        {
            _gameDlProgress.MaxDLfiles = (int)newDLData.SplitFileNum;
        }

        //拡張子が.000以外のファイルを進捗を表示させながら保存する
        foreach (var pair in needDlFileDic)
        {
            if (pair.Key.EndsWith(newDLData.DLDataInfoFileExtention)) continue;
            if (!_doingTaskFlag) return; //もし停止するようフラッグが変わっていた場合処理を中断する

            _onNetDriveFetFile.GetFile(pair.Value, pair.Key, tempSlicedGameDLPath);

            if(_gameDlProgress != null)
            {
                _gameDlProgress.NowDLedFileCount++;
            }
        }


        string gameFileName = newDLData.FileName;
        //完成したゲームデータが置かれるパス
        string newGamePath = CreateDirPath.GameDataPath(saveGamesDirName: allDirs.GameFilePath, gameId: gameId, gameDirName: gameFileName);
        
        //保存したファイル群をマージする(FileCombineに不足ファイルが合った際に呼ぶ処理を登録できる)
        new FileCombine().MergeSplitedFile(tempGameDLPath, newGamePath);

        GameData.Status = GameStatus.Downloaded;
        string thisGameJsonPath = CreateDirPath.GameJsonPath(savedJsonsPath: allDirs.JsonsDirPath, gameId: gameId);
        //ダウンロード済みデータとしてjsonに保存
        JSONTools.SerializeJson(GameData, thisGameJsonPath);

        //ダウンロードに利用した一時保存関係のファイル・フォルダを全て削除する
        DirectoryActs.CompleteDirDelete(tempGameDLPath);
    }

    private LastGameDLState CheckLastStatus(string gameID, string tempSliceGamePath)
    {
        AllDirs allDirs = AllDirs.GetInstance();
        //既にダウンロード済み
        if (File.Exists(Path.Combine(allDirs.JsonsDirPath, gameID + ".json")))
        {
            return LastGameDLState.Completely;
        }

        string[] files = Directory.GetFiles(tempSliceGamePath);

        //スライスされたファイルが存在しない
        if (files == null || files.Length == 0)
        {
            return LastGameDLState.None;
        }
        //スライス状態で存在する
        else
        {
            return LastGameDLState.ExistSplit;
        }


    }

    //既に前回ダウンロード途中の跡がある際に、前回から現在まででアップデートが行われたかを確認し、行われていたならディレクトリを削除する
    private bool CheckUpdated(DLData oldData, DLData newData)
    {
        //ファイル容量もしくは、GameIDが異なる
        if(!isSameDataSize(oldData, newData))
        {
            return true;
        }
        return false;
    }

    private bool isSameDataSize(DLData oldData, DLData newData)
    {
        if(oldData.GameSize != newData.GameSize || oldData.FileName != newData.FileName)
        {
            return false;
        }
        return true;

    }

    private Dictionary<string,string> CreateLackDic(MistakeFiles mistakeFiles, Dictionary<string, string> metaDic)
    {
        string fileName = mistakeFiles.FileName;
        //不足しているファイルをlong型のリストからstring型のファイル名に変更する
        List<string> lackStrList = mistakeFiles.LackFiles.Select(x => string.Format("{0}.{1:D3}", fileName, x)).ToList();
        Dictionary<string, string> lackFilesDic = new Dictionary<string, string>();
        foreach(string lackFileName in lackStrList)
        {
            if (metaDic.ContainsKey(lackFileName))
            {
                lackFilesDic[lackFileName] = metaDic[lackFileName];
            }
            else
            {
                throw new GameDlCustomException("ドライブに必要なファイルが保存されていないため、結合できません", GameDlErrorType.ImpossibleRecoveryErrorOnDrive);
            }
        }
        return lackFilesDic;
    }

    /// <summary>
    /// DLData(.000)のファイルをドライブから取得するための処理
    /// </summary>
    private DLData GetDLDataFromDrive(Dictionary<string, string> metaDic, string tempSlicedGameDLPath)
    {
        DLData gameDLData = new DLData();

        Dictionary<string, string> dlDataFileNameAndDriveId = metaDic.Where(x => x.Key.EndsWith(gameDLData.DLDataInfoFileExtention)).ToDictionary(x => x.Key, x => x.Value);
        //.000ファイルの数が不正な場合
        if (dlDataFileNameAndDriveId == null || dlDataFileNameAndDriveId.Count == 0 || dlDataFileNameAndDriveId.Count > 1)
        {
            throw new GameDlCustomException("ドライブにフォルダが存在しないか、ドライブにある'.00'形式のファイル数が不正です", GameDlErrorType.ImpossibleRecoveryErrorOnDrive);
        }
        string dlDataFileName = dlDataFileNameAndDriveId.Keys.First();
        string dlDataDriveId = dlDataFileNameAndDriveId.Values.First();
        //.000ファイル(DLDataが書かれたファイル)をダウンロードする
        _onNetDriveFetFile.GetFile(dlDataDriveId, dlDataFileName, tempSlicedGameDLPath);

        //.000ファイルからインストールするゲームの情報を取得
        gameDLData.DeserializeDataByFilePath(Path.Combine(tempSlicedGameDLPath, dlDataFileName));

        return gameDLData;
    }
}
