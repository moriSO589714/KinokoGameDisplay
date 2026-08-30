using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 保存されているゲームのデータを管理するシングルトンクラス
/// </summary>
public class GameDatasSingleton : BasedSingleton<GameDatasSingleton>
{
    public List<GameData> AllGameDatas { get; private set; } = new List<GameData>();

    //現在GameBoxとして表示されているゲームとその条件指定用のGameDataインスタンス
    public List<GameData> CurrentDisplayGames { get; private set; } = new List<GameData>();
    public FilterCondition CurrentFilterCondition { get; private set; } = null;

    //予測変換用辞書
    private WordEmtCell _tagsLib;
    private WordEmtCell _devsLib;
    private WordEmtCell _toolsLib;
    private WordEmtCell _gameIdLib;
    private List<string> _titlesLib;

    //ゲームデータをリストにセット
    public void AddGameData(GameData gameData)
    {
        //追加
        if(CheckGameData(gameData)) AllGameDatas.Add(gameData);
    }
    public void AddGameDataList(List<GameData> gameDatas)
    {
        List<GameData> addGameDataList = new List<GameData>();
        foreach(GameData gameData in gameDatas)
        {
            if(CheckGameData(gameData)) addGameDataList.Add(gameData);
        }
        AllGameDatas.AddRange(addGameDataList);
    }

    //リストのリセット
    public void ResetGameDataList()
    {
        AllGameDatas = new List<GameData>();
    }

    public void SetCurrentDisplayGames(List<GameData> currentGameDatas, FilterCondition currentFilterCondition)
    {
        CurrentDisplayGames = currentGameDatas;
        CurrentFilterCondition = currentFilterCondition;
    }

    public WordEmtCell ReturnTagsLib()
    {
        _tagsLib = CreateLibFromGameDatas.CreateTagsLib(AllGameDatas);
        return _tagsLib;
    }

    public WordEmtCell ReturnDeveroppersLib()
    {
        _devsLib = CreateLibFromGameDatas.CreateDeveropperLib(AllGameDatas);
        return _devsLib;
    }

    public WordEmtCell ReturnToolsLib()
    {
        _toolsLib = CreateLibFromGameDatas.CreateToolsLib(AllGameDatas);
        return _toolsLib;
    }

    public WordEmtCell ReturnGameIdLib()
    {
        _gameIdLib = CreateLibFromGameDatas.CreateGameIdLib(AllGameDatas);
        return _gameIdLib;
    }

    public List<string> ReturnTitlesLib()
    {
        _titlesLib = CreateLibFromGameDatas.CreateTitlesLib(AllGameDatas);
        return _titlesLib;
    }

    private bool CheckGameData(GameData gameData)
    {
        foreach(GameData singletonGameData in AllGameDatas)
        {
            //シングルトンに既に同じゲームIDのゲーム情報が登録されている場合
            if(singletonGameData.GameID == gameData.GameID)
            {
                //追加されるゲームのバージョンが新しい場合
                if(long.Parse(singletonGameData.GameVersion) < long.Parse(gameData.GameVersion) && singletonGameData.Status == GameStatus.Downloaded)
                {
                    singletonGameData.Status = GameStatus.UpdateAvailable;
                }

                //それ以外に異なるデータが存在する場合
                FieldInfo[] gameDataFields = typeof(GameData).GetFields();
                foreach(FieldInfo field in gameDataFields)
                {
                    //ゲームのバージョンは処理を走らせない
                    if (field.Name == "GameVersion") continue;
                    if (field.Name == "Status") continue;
                    var singletonDataValue = field.GetValue(singletonGameData);
                    var newGameDataValue = field.GetValue(gameData);
                    if(singletonDataValue != newGameDataValue)
                    {
                        //シングルトン側のデータを最新のものに書き換える
                        field.SetValue(singletonGameData, newGameDataValue);
                    }
                }
                return false;
            }
        }

        //ゲーム情報として扱う最低要件を満たせていない場合は追加しない
        if (!GameDataQualityCheck.CheckQuality(gameData)) return false;
        return true;
    }
}

