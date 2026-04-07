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
            throw new System.Exception("jsonファイルが入ったフォルダのパスを取得できません。");
        }
        //ゲームデータクラスのリスト化
        List<GameData> gameDatas = new JSONandGameDataChanger().JSONDirPathToGameData(jsonsDirPath);

        GameDatasSingleton gameDatasSingleton = GameDatasSingleton.Instance;
        List<GameData> gameDatasinSingleton = gameDatasSingleton.GameDatas;
        //GameDatasSingletonにすでに追加されているフォルダ名ではないことを確認してから、追加する
        foreach(GameData g in gameDatas)
        {
            if (gameDatasinSingleton.Any(x => x.GameDirName == g.GameDirName)) continue;
            //追加
            gameDatasSingleton.AddGameData(g);
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
