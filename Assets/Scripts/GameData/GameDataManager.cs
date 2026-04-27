using Google.Apis.Sheets.v4;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class GameDataManager
{
    /// <summary>
    /// 全パターンのシングルトンへのゲームデータ登録を行う(ほとんどの場合このメソッドからロードを行う)
    /// </summary>
    public void OverallGameDataLoad()
    {
        new GameDatasSingleton().ResetGameDataList();
        LoadGameDataFromJsons();
        GameData testGameData = new GameData();
        testGameData.GameTitle = "2DGolf";
        LoadGameDataFromSpSt(new List<GameData>() { testGameData });
    }


    /// <summary>
    /// jsonデータが入っているディレクトリからGameData群をロード(シングルトンに追加)する
    /// </summary>
    public void LoadGameDataFromJsons()
    {
        //jsonファイルが入っているディレクトリのパスを取得してくる
        AllDirs allDirs = AllDirs.GetInstance();
        string jsonsDirPath = allDirs.JsonsDirPath;
        if(jsonsDirPath == null)
        {
            throw new System.Exception("cannot get the path to the folder containing the JSON file");
        }
        //ゲームデータクラスのリスト化
        List<GameData> gameDatas = new JSONandGameDataChanger().JSONDirPathToGameData(jsonsDirPath);

        GameDatasSingleton gameDatasSingleton = GameDatasSingleton.Instance;
        gameDatasSingleton.AddGameDataList(gameDatas);
    }

    /// <summary>
    /// インターネット上(スプレッドシート)からGameData群をロード(シングルトンに追加)する
    /// </summary>
    /// <param name="filterObjects">絞りこみを行う場合、条件を代入したGameDataクラス,nullの場合絞りこみを行わない</param>
    public void LoadGameDataFromSpSt(List<GameData> filterObjects)
    {
        List<GameData> gameDatas = new List<GameData>();
        GameDatasSingleton gameDatasSingleton = GameDatasSingleton.Instance;
        //スプレッドシートから全てのGameDataを取得してくる
        if(filterObjects == null)
        {

        }
        //条件をもとにスプレッドシートからGameDataを取得してくる
        else
        {
            CollectivelyGetFromSpSt collectivelyGetFromSpSt = new CollectivelyGetFromSpSt();
            string jsonPathKey = AllDirs.GetInstance().JsonPathKey;
            string spId = AllDirs.GetInstance().SpreadSheetID;
            OnNetGameInfo onNetGameInfo = null;
            if (!CheckInEnvironment.CheckDoingNet())
            {
                onNetGameInfo = new TestOnNetGameInfo();
            }
            else
            {
                CreateAPIService createAPIService = new CreateAPIService(jsonPathKey);
                SheetsService sheetsService = createAPIService.CreateSheetAPIService();
                onNetGameInfo = new OnNetGameInfoFromSpSt(sheetsService, spId);
            }

            foreach(GameData g in filterObjects)
            {
                List<GameData> getDatas = collectivelyGetFromSpSt.FilterGameDataFromSpreadSheet(g, onNetGameInfo);
                gameDatasSingleton.AddGameDataList(getDatas);
            }
        }
    }

    //============================================================================================================
    //一旦利用しないものとする。最終実装まで必要なければ消すこと。(自動でソフトが追加するよりも利用者が能動的に追加したほうが良いため)
    //============================================================================================================
    /// <summary>
    /// ゲームデータのフォルダが入っているフォルダから、ゲームをシングルトンにロードする
    /// このメソッドはJsonで登録されたゲームデータのロードの後に実行される必要がある。
    /// </summary>
    public void LoadGameDataFromGameDir()
    {
        //ゲームデータが入っているフォルダのパスを取得してくる
        AllDirs allDirs = AllDirs.GetInstance();
        string gameFilePath = allDirs.GameFilePath;
        //パス上に存在する全てのフォルダを取得する
        string[] dirs = Directory.GetDirectories(gameFilePath);

        GameDatasSingleton gameDatasSingleton = GameDatasSingleton.Instance;
        List<GameData> gameDatasinSingleton = gameDatasSingleton.GameDatas;
        foreach(string str in dirs)
        {
            string fileName = Path.GetFileName(str);
            //同じファイル名のゲームが既にシングルトンに追加されていた場合処理をスキップする
            if (gameDatasinSingleton.Any(x => x.GameDirName == fileName)) continue;

            //GamaDataクラスにして、シングルトンに追加する
            GameData gameData = new GameData();
            gameData.GameDirName = fileName;
            gameDatasSingleton.AddGameData(gameData);
        }
    }


}
