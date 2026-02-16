using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 保存されているゲームのデータを管理するシングルトンクラス
/// </summary>
public class GameDatasSingleton : BasedSingleton<GameDatasSingleton>
{
    public List<GameData> GameDatas { get; private set; } = new List<GameData>();
    
    //ゲームデータをリストにセット
    public void AddGameData(GameData gameData)
    {
        GameDatas.Add(gameData);
    }
}

