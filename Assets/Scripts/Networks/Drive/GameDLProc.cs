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

    public GameDLProc(OnNetDriveMetaData metaData, OnNetDriveGetFile onNetDriveFetFile)
    {
        _onNetDriveMetaData = metaData;
        _onNetDriveFetFile = onNetDriveFetFile;
    }

    /// <summary>
    /// 固有IDからゲームデータをローカルに保存する
    /// </summary>
    public void DLGame(GameData gamedata)
    {
        string gameId = gamedata.GameID;
        string driveId = gamedata.GameDriveId;
        AllDirs allDirs = AllDirs.GetInstance();

        //他のゲームがダウンロードされているディレクトリに同じIDのゲームが保存されていないか確認する処理
        if (File.Exists(Path.Combine(allDirs.GameFilePath, gameId)))
        {
            throw new System.Exception("既にダウンロードフォルダに同じIDのゲームが保存されています。削除し、再度実行してください。ID >>>" + gameId);
        }

        //保存用一時ディレクトリの作成
        DirectoryActs directoryActs = new DirectoryActs();
        string tempDirPath = AllDirs.GetInstance().TempPath;
        directoryActs.CreateAndCheckDir(tempDirPath);
        string tempGameDLPath = Path.Combine(tempDirPath, gameId);
        string tempSlicedGameDLPath = Path.Combine(tempGameDLPath, "sliced");
        directoryActs.CreateAndCheckDir(tempSlicedGameDLPath);

        MistakeFiles mistakeFiles = null;
        //前回同じゲームIDのゲームを保存したことがある場合、不足しているファイルの番号を取得する
        try
        {
            mistakeFiles = new FreezingTools().hasAllRequiredData(tempSlicedGameDLPath);
        }
        catch (Exception e)
        {
            //エラーが帰ってきた場合、フォルダを削除して再生成する
            RefleshDir(tempSlicedGameDLPath);
        }

        //ドライブからメタデータを取得してくる
        Dictionary<string, string> metaDic = _onNetDriveMetaData.GetFileList(driveId);
        Dictionary<string, string> needDlFileDic = new Dictionary<string, string>();
        DLData gameDLData = GetDLDataFromDrive(metaDic, tempSlicedGameDLPath);

        //前回ダウンロードした際と全体の容量が変わっている場合は、アップデートが入ったとみなし、全てダウンロードし直す
        if (mistakeFiles != null)
        {
            //.000ファイルのパスを作る
            string dlDataFilePath = Path.Combine(tempSlicedGameDLPath, mistakeFiles.FileName + ".000");
            DLData oldGameDLData = new DLData(dlDataFilePath);
            if (CheckUpdated(oldGameDLData, gameDLData, tempSlicedGameDLPath))
            {
                //ディレクトリを削除してダウンロードし直す
                RefleshDir(tempSlicedGameDLPath);
                needDlFileDic = metaDic;
            }
            else
            {
                //アップデートが存在しない
                //ダウンロードするファイルのディクショナリを作成
                needDlFileDic = CreateLackDic(mistakeFiles, metaDic);
            }
        }
        else
        {
            RefleshDir(tempSlicedGameDLPath);
            needDlFileDic = metaDic;
        }

        //ダウンロードするファイルの数を取得
        long maxDLfiles = gameDLData.SplitedFileNum;
        //ダウンロード済みのファイルの数
        long nowDLedFileCounts = 0;
        //拡張子が.000以外のファイルを進捗を表示させながら保存する
        foreach (var pair in needDlFileDic)
        {
            if (pair.Key.EndsWith(".000")) continue;
            _onNetDriveFetFile.GetFile(pair.Value, pair.Key, tempSlicedGameDLPath);
            nowDLedFileCounts++;
            Debug.Log(nowDLedFileCounts + "まで終了" + "ダウンロードしたファイル>>>" + pair.Key);
        }


        //保存したファイル群をマージする(FileCombineに不足ファイルが合った際に呼ぶ処理を登録できる)
        string gameTempPath = new FileCombine().MergeSplitedFile(tempSlicedGameDLPath, tempGameDLPath);
        string gameFileName = Path.GetFileName(gameTempPath);
        string newGamePath = Path.Combine(allDirs.GameFilePath, gameFileName);
        //ゲームデータを一時保存用のフォルダから他のゲームも保存されているフォルダに移動する
        Directory.Move(gameTempPath, newGamePath);

        gamedata.Status = GameStatus.Downloaded;
        string thisGameJsonPath = Path.Combine(allDirs.JsonsDirPath, gameId + ".json");
        //ダウンロード済みデータとしてjsonに保存
        JSONTools.SerializeJson(gamedata, thisGameJsonPath);

        //ダウンロードに利用した一時保存関係のファイル・フォルダを全て削除する
        new DirectoryActs().CompleteDirDelete(tempGameDLPath);
    }

    //指定のディレクトリを完全に削除して作り直す処理
    private void RefleshDir(string path)
    {
        DirectoryActs directoryActs = new DirectoryActs();
        directoryActs.CompleteDirDelete(path);
        directoryActs.CreateAndCheckDir(path);
    }

    //既に前回ダウンロード途中の後がある際に、前回から現在まででアップデートが行われたかを確認し、行われていたならディレクトリを削除する
    private bool CheckUpdated(DLData oldData, DLData newData, string path)
    {
        //ファイル容量もしくは、GameIDが異なる
        if(!isSameDataSize(oldData, newData))
        {
            RefleshDir(path);
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
        Dictionary<string, string> dlDataFileNameAndDriveId = metaDic.Where(x => x.Key.EndsWith(".000")).ToDictionary(x => x.Key, x => x.Value);
        //.000ファイルの数が不正な場合
        if (dlDataFileNameAndDriveId == null || dlDataFileNameAndDriveId.Count == 0 || dlDataFileNameAndDriveId.Count > 1)
        {
            throw new System.Exception("ドライブへの接続に失敗しました。ドライブにフォルダが存在しないか、ドライブにある'.00'形式のファイル数が不正です");
        }
        string dlDataFileName = dlDataFileNameAndDriveId.Keys.First();
        string dlDataDriveId = dlDataFileNameAndDriveId.Values.First();
        //.000ファイル(DLDataが書かれたファイル)をダウンロードする
        _onNetDriveFetFile.GetFile(dlDataDriveId, dlDataFileName, tempSlicedGameDLPath);

        //.000ファイルからインストールするゲームの情報を取得
        DLData gameDLData = new DLData();
        gameDLData.DeserializeDataByFilePath(Path.Combine(tempSlicedGameDLPath, dlDataFileName));

        return gameDLData;
    }
}
