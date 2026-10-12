using System.Collections.Generic;

/// <summary>
/// 保存されているゲームのデータを管理するシングルトンクラス
/// </summary>
public class GameDatasSingleton : BasedSingleton<GameDatasSingleton>
{
    public List<GameData> AllGameDatas { get; private set; } = new List<GameData>();

    //現在GameBoxとして表示されているゲームとその条件指定用のGameDataインスタンス
    public List<GameData> CurrentDisplayGames { get; private set; } = new List<GameData>();
    public FilterCondition CurrentFilterCondition { get; private set; } = null;

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
    public void RemoveGameData(GameData gameData)
    {
        AllGameDatas.Remove(gameData);
    }

    /// <summary>
    /// AllDataに存在するGameDataインスタンスを保持したまま、データを入れ替える
    /// (アップデートした際に利用)
    /// </summary>
    public void ReplaceGameData(GameData newGameData)
    {
        GameData oldGameData = null;
        foreach(GameData gameData in AllGameDatas)
        {
            if(gameData.GameID == newGameData.GameID)
            {
                oldGameData = gameData;
            }
        }

        GameDataReplacer.ComparisonAndReplace(oldGameData, newGameData);
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

    private bool CheckGameData(GameData gameData)
    {
        foreach(GameData singletonGameData in AllGameDatas)
        {
            //シングルトンに既に同じゲームIDのゲーム情報が登録されている場合
            if(singletonGameData.GameID == gameData.GameID)
            {
                CheckGameDataUpdate.CheckUpdate(oldGameData: singletonGameData, newGameData: gameData);
                return false;
            }
        }

        //ゲーム情報として扱う最低要件を満たせていない場合は追加しない
        if (!GameDataQualityCheck.CheckQuality(gameData)) return false;
        return true;
    }
}