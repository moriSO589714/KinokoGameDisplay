using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 保存されているゲームのデータを管理するシングルトンクラス
/// </summary>
public class GameDatasSingleton : BasedSingleton<GameDatasSingleton>
{
    public List<GameData> AllGameDatas { get; private set; } = new List<GameData>();

    //現在GameBoxとして表示されているゲームとその条件指定用のGameDataインスタンス
    public List<GameData> CurrentDisplayGames { get; private set; } = new List<GameData>();
    public List<GameData> CurrentFilterConditions { get; private set; } = new List<GameData>();
    
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

    public void SetCurrentDisplayGames(List<GameData> currentGameDatas, List<GameData> currentFilterConditions)
    {
        CurrentDisplayGames = currentGameDatas;
        CurrentFilterConditions = currentFilterConditions;
    }

    private bool CheckGameData(GameData gameData)
    {
        foreach(GameData singletonGameData in AllGameDatas)
        {
            //シングルトンに既に同じゲームIDのゲーム情報が登録されている場合
            if(singletonGameData.GameID == gameData.GameID)
            {
                //追加されるゲームのバージョンが新しい場合
                if(int.Parse(singletonGameData.GameVersion) < int.Parse(gameData.GameVersion))
                {
                    singletonGameData.Status = GameStatus.UpdateAvailable;
                }
                return false;
            }
        }

        //ゲーム情報として扱う最低要件を満たせていない場合は追加しない
        if (!GameDataQualityCheck.CheckQuality(gameData)) return false;
        return true;
    }
}

