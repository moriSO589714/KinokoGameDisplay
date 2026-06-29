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
    public List<GameData> CurrentFilterConditions { get; private set; } = new List<GameData>();

    //現在のタグの予測変換用辞書
    public WordEmtCell TagsLib { get; private set; }

    //ゲームデータをリストにセット
    public void AddGameData(GameData gameData)
    {
        //追加
        if(CheckGameData(gameData)) AllGameDatas.Add(gameData);
        //予測変換用辞書のリロード
        SetEstimateLibs();
    }
    public void AddGameDataList(List<GameData> gameDatas)
    {
        List<GameData> addGameDataList = new List<GameData>();
        foreach(GameData gameData in gameDatas)
        {
            if(CheckGameData(gameData)) addGameDataList.Add(gameData);
        }
        AllGameDatas.AddRange(addGameDataList);
        //予測変換用辞書のリロード
        SetEstimateLibs();
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
                if(int.Parse(singletonGameData.GameVersion) < int.Parse(gameData.GameVersion) && singletonGameData.Status == GameStatus.Downloaded)
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

    /// <summary>
    /// 予測変換に利用する辞書をセットしなおす
    /// </summary>
    private void SetEstimateLibs()
    {
        SetTagsEstimateLib();
    }

    private void SetTagsEstimateLib()
    {
        TagsLib = CreateTagsEstimateLib();
    }

    /// <summary>
    /// AllGameDatasから予測変換用の辞書を作成する
    /// </summary>
    private WordEmtCell CreateTagsEstimateLib()
    {
        Dictionary<string, int> words = new Dictionary<string, int>();
        foreach(GameData gd in AllGameDatas)
        {
            if (gd.GameTags == null) continue;
            foreach(string tag in gd.GameTags)
            {
                if (words.ContainsKey(tag))
                {
                    words[tag]++;
                }
                else
                {
                    words[tag] = 1;
                }
            }
        }

        WordEmtCell tagEstimateLib = WECLibCreater.CreateLibFromLineAndPriority(words);
        return tagEstimateLib;
    }
}

