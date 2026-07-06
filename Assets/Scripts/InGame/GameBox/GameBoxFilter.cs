using System.Collections.Generic;
using System.Linq;

/// <summary>
/// GameDatasSingletonにあるGameDatasからフィルタリングしたものをGameBoxManagerにセットしてUI反映する
/// </summary>
public static class GameBoxFilter
{
    public static List<GameData> FilteringGameDatas(FilterCondition filterCondition, List<GameData> filteringTargetList)
    {
        return filtering(filterCondition, filteringTargetList);
    }

    /// <summary>
    /// GameDataクラスを利用してフィルタリングを行う。インスタンス1つに格納されたものはandだが、複数インスタンスはorで絞られる
    /// </summary>
    private static List<GameData> filtering(FilterCondition filterCondition, List<GameData> filteringTargetList)
    {
        //フィルタリングされたゲームデータが入るクラス
        List<GameData> filterdGames = new List<GameData>(filteringTargetList);
        
        //ゲームのステータス
        List<GameData> statusCandidate = new List<GameData>();
        foreach(GameStatus status in filterCondition.Statuses)
        {
            statusCandidate.AddRange(filterStatus(status, filterdGames));
        }
        filterdGames = new List<GameData>(statusCandidate);

        //ゲームのソフトウェア種類
        List<GameData> softwareCandidate = new List<GameData>();
        if (filterCondition.Softs.Count == 0)
        {
            softwareCandidate = filterdGames;
        }
        foreach(string soft in filterCondition.Softs)
        {
            softwareCandidate.AddRange(filterSoft(soft, filterdGames));
        }
        filterdGames = new List<GameData>(softwareCandidate);

        //ゲームのタイトル名
        List<GameData> gameNamesCandidate = new List<GameData>();
        if(filterCondition.GameNames.Count == 0)
        {
            gameNamesCandidate = filterdGames;
        }
        foreach(List<string> gameNames in filterCondition.GameNames)
        {
            gameNamesCandidate.AddRange(filterGameName(gameNames, filterdGames));
        }
        filterdGames = new List<GameData>(gameNamesCandidate);


        //ゲームの開発者
        List<GameData> devCandidate = new List<GameData>();
        if (filterCondition.GameDevs.Count == 0)
        {
            devCandidate = filterdGames;
        }
        foreach (List<string> devs in filterCondition.GameDevs)
        {
            devCandidate.AddRange(filterGameDev(devs, filterdGames));
        }
        filterdGames = new List<GameData>(devCandidate);


        //ゲームのタグ
        List<GameData> tagCandidate = new List<GameData>();
        if(filterCondition.GameTags.Count == 0)
        {
            tagCandidate = filterdGames;
        }
        foreach(List<string> tags in filterCondition.GameTags)
        {
            tagCandidate.AddRange(filterTag(tags, filterdGames));
        }
        filterdGames = new List<GameData>(tagCandidate);

        //重複したGameDataクラスを削除する
        filterdGames = filterdGames.Distinct().ToList();
        return filterdGames;
    }


    /// <summary>
    /// ゲームタイトルは部分一致でもリストに追加する
    /// </summary>
    private static List<GameData> filterGameName(List<string> gameNames, List<GameData> currentGameDatas)
    {
        if (gameNames == null || gameNames.Count == 0)
        {
            return currentGameDatas;
        }
        else
        {
            foreach(string gameName in gameNames)
            {
                currentGameDatas = currentGameDatas.Where(x => x.GameTitle.Contains(gameName)).ToList();
            }
            return currentGameDatas;
        }
    }

    /// <summary>
    /// 開発者名は完全一致
    /// </summary>
    private static List<GameData> filterGameDev(List<string> devs, List<GameData> currentGameDatas)
    {
        if(devs.Count() == 0)
        {
            return currentGameDatas;
        }
        else
        {
            foreach(string dev in devs)
            {
                currentGameDatas = currentGameDatas.Where(x => x.GameDevelopper.Contains(dev)).ToList();
            }
        }
        return currentGameDatas;
    }

    /// <summary>
    /// ソフトウェアでのフィルタリング
    /// </summary>
    private static List<GameData> filterSoft(string soft, List<GameData> currentGameDatas)
    {
        if(soft == null || soft == "")
        {
            return currentGameDatas;
        }
        else
        {
            List<GameData> returnList = currentGameDatas.Where(x => x.GameSoftwareType == soft).ToList();
            return returnList;
        }
    }

    private static List<GameData> filterTag(List<string> tags, List<GameData> currentGameDatas)
    {
        if(tags.Count() == 0)
        {
            return currentGameDatas;
        }
        else
        {
            foreach(string tag in tags)
            {
                currentGameDatas = currentGameDatas.Where(x => x.GameTags.Contains(tag)).ToList();
            }
            return currentGameDatas;
        }
    }

    private static List<GameData> filterStatus(GameStatus status, List<GameData> currentGameDatas)
    {
        currentGameDatas = currentGameDatas.Where(x => x.Status == status).ToList();
        return currentGameDatas;
    }
}
