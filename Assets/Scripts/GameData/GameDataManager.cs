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
        CollectivelyGetFromSpSt collectivelyGetFromSpSt = new CollectivelyGetFromSpSt();
        
        //スプレッドシートから全てのGameDataを取得してくる
        if(filterObjects == null)
        {
            string jsonPathKey = AllDirs.GetInstance().JsonPathKey;
            string spId = AllDirs.GetInstance().SpreadSheetID;

            List<GameData> gotGameDatas = collectivelyGetFromSpSt.AllGameDataFromSpSt();
            gameDatasSingleton.AddGameDataList(gotGameDatas);
        }
        //条件をもとにスプレッドシートからGameDataを取得してくる
        else
        {
            string jsonPathKey = AllDirs.GetInstance().JsonPathKey;
            string spId = AllDirs.GetInstance().SpreadSheetID;
            OnNetGameInfo onNetGameInfo = null;
            if (!CheckInEnvironment.CheckDoingNet())
            {
                onNetGameInfo = new OnNetGameInfoFromTest();
            }
            else
            {
                CreateAPIService createAPIService = new CreateAPIService(jsonPathKey);
                SheetsService sheetsService = createAPIService.CreateSheetAPIService();
                onNetGameInfo = new OnNetGameInfoFromSpSt(sheetsService, spId);
            }

            foreach(GameData g in filterObjects)
            {
                List<GameData> getDatas = collectivelyGetFromSpSt.FilterGameDataFromSpSt(g);
                gameDatasSingleton.AddGameDataList(getDatas);
            }
        }
    }
}
