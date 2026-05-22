using Cysharp.Threading.Tasks;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

public class GameDLProc
{
    private OnNetDriveMetaData _onNetDriveMetaData = null;
    private OnNetDriveGetFile _onNetDriveFetFile = null;
    private GameData _gameData;

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

    public GameDLProc(OnNetDriveMetaData metaData, OnNetDriveGetFile onNetDriveFetFile, GameData gameData)
    {
        _gameData = gameData;
        _onNetDriveMetaData = metaData;
        _onNetDriveFetFile = onNetDriveFetFile;
    }

    public async UniTask DLGameInUniTask()
    {
        //ダウンロードタスクをスレッドプールで実行
        await UniTask.RunOnThreadPool(DLGame);
    }

    /// <summary>
    /// 固有IDからゲームデータをローカルに保存する
    /// </summary>
    public void DLGame()
    {
        string gameId = _gameData.GameID;
        string driveId = _gameData.GameDriveId;
        AllDirs allDirs = AllDirs.GetInstance();

        //使用するパスの定義
        string tempDirPath = AllDirs.GetInstance().TmpDLPath;
        string tempGameDLPath = Path.Combine(tempDirPath, gameId);
        string tempSlicedGameDLPath = Path.Combine(tempGameDLPath, "sliced");

        //保存用一時ディレクトリの作成
        DirectoryActs.CreateAndCheckDir(tempSlicedGameDLPath);
        //前回ダウンロードした形跡がないかを確認する
        LastGameDLState lastGameDLState = CheckLastStatus(gameId, tempSlicedGameDLPath);

        if(lastGameDLState == LastGameDLState.Completely)
        {
            throw new Exception("このゲームは既にダウンロードされています");
        }

        if(lastGameDLState == LastGameDLState.FileError)
        {
            DirectoryActs.RefleshDir(tempSlicedGameDLPath);
        }

        //ドライブからメタデータを取得してくる
        Dictionary<string, string> metaDic = _onNetDriveMetaData.GetFileList(driveId);
        Dictionary<string, string> needDlFileDic = new Dictionary<string, string>();

        DLData newDLData = null;

        //スライスされたファイルが既に存在する場合
        //ここあまりにも長いからリファクタリングする！！！
        if(lastGameDLState == LastGameDLState.ExistSplit)
        {
            MistakeFiles mistakeFiles = null;
            try
            {
                mistakeFiles = new FreezingTools().hasAllRequiredData(tempSlicedGameDLPath);

                string dlDataFilePath = Path.Combine(tempSlicedGameDLPath, mistakeFiles.FileName + new DLData().DLDataFileExtention);
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
            catch (Exception e)
            {
                DirectoryActs.RefleshDir(tempSlicedGameDLPath);
                newDLData = GetDLDataFromDrive(metaDic, tempSlicedGameDLPath);
                needDlFileDic = metaDic;
            }
        }
        else
        {
            newDLData = GetDLDataFromDrive(metaDic, tempSlicedGameDLPath);
            needDlFileDic = metaDic;
        }

        //ダウンロードするファイルの数を取得
        long maxDLfiles = newDLData.SplitFileNum;
        //ダウンロード済みのファイルの数
        long nowDLedFileCounts = 0;
        //拡張子が.000以外のファイルを進捗を表示させながら保存する
        foreach (var pair in needDlFileDic)
        {
            if (pair.Key.EndsWith(newDLData.DLDataFileExtention)) continue;
            _onNetDriveFetFile.GetFile(pair.Value, pair.Key, tempSlicedGameDLPath);
            nowDLedFileCounts++;
            Debug.Log(nowDLedFileCounts + "まで終了" + "ダウンロードしたファイル>>>" + pair.Key);
        }


        string gameFileName = newDLData.FileName;
        //完成したゲームデータが置かれるパス
        string newGamePath = Path.Combine(allDirs.GameFilePath, gameId, gameFileName);
        
        //保存したファイル群をマージする(FileCombineに不足ファイルが合った際に呼ぶ処理を登録できる)
        new FileCombine().MergeSplitedFile(tempSlicedGameDLPath, newGamePath);
        
        _gameData.Status = GameStatus.Downloaded;
        string thisGameJsonPath = Path.Combine(allDirs.JsonsDirPath, gameId + ".json");
        //ダウンロード済みデータとしてjsonに保存
        JSONTools.SerializeJson(_gameData, thisGameJsonPath);

        //ダウンロードに利用した一時保存関係のファイル・フォルダを全て削除する
        DirectoryActs.CompleteDirDelete(tempGameDLPath);
        
    }

    private LastGameDLState CheckLastStatus(string gameID, string tempSliceGamePath)
    {
        AllDirs allDirs = AllDirs.GetInstance();
        if (File.Exists(Path.Combine(allDirs.GameFilePath, gameID)))
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

    //既に前回ダウンロード途中の後がある際に、前回から現在まででアップデートが行われたかを確認し、行われていたならディレクトリを削除する
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
                throw new Exception("ドライブに必要なファイルが保存されていないため、結合できません");
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

        Dictionary<string, string> dlDataFileNameAndDriveId = metaDic.Where(x => x.Key.EndsWith(gameDLData.DLDataFileExtention)).ToDictionary(x => x.Key, x => x.Value);
        //.000ファイルの数が不正な場合
        if (dlDataFileNameAndDriveId == null || dlDataFileNameAndDriveId.Count == 0 || dlDataFileNameAndDriveId.Count > 1)
        {
            throw new System.Exception("ドライブにフォルダが存在しないか、ドライブにある'.00'形式のファイル数が不正です");
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
