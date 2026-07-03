using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CreateLibFromGameDatas
{
    public static WordEmtCell CreateTagsLib(List<GameData> gameDatasList)
    {
        string[][] tagsFromGameDatas = gameDatasList.Select(x => x.GameTags).ToArray();
        WordEmtCell tagsLib = PickUpElementFromStrDoubleArray(tagsFromGameDatas);
        return tagsLib;
    }

    public static WordEmtCell CreateDeveropperLib(List<GameData> gameDatasList)
    {
        string[][] devsFromGameDatas = gameDatasList.Select(x => x.GameDevelopper).ToArray();
        WordEmtCell devsLib = PickUpElementFromStrDoubleArray(devsFromGameDatas);
        return devsLib;
    }

    public static WordEmtCell CreateToolsLib(List<GameData> gameDatasList)
    {
        string[] toolsFromGameDatas = gameDatasList.Select(x => x.GameSoftwareType).ToArray();
        WordEmtCell toolsLib = PickUpElementFromStrArray(toolsFromGameDatas);
        return toolsLib;
    }

    public static List<string> CreateTitlesLib(List<GameData> gameDatasList)
    {
        List<string> titlesFromGameDatas = gameDatasList.Select(x => x.GameTitle).ToList();
        return titlesFromGameDatas;
    }

    private static WordEmtCell PickUpElementFromStrDoubleArray(string[][] elements)
    {
        Dictionary<string, int> words = new Dictionary<string, int>();
        foreach (string[] elementArray in elements)
        {
            if(elementArray == null) continue;
            foreach (string element in elementArray)
            {
                words = AddOnWordDic(words, element);
            }
        }
        WordEmtCell resultLib = WECLibCreater.CreateLibFromLineAndPriority(words);
        return resultLib;
    }

    private static WordEmtCell PickUpElementFromStrArray(string[] elements)
    {
        Dictionary<string, int> words = new Dictionary<string, int>();
        foreach(string element in elements)
        {
            words = AddOnWordDic(words, element);
        }
        WordEmtCell resultLib = WECLibCreater.CreateLibFromLineAndPriority(words);
        return resultLib;
    }

    private static Dictionary<string, int> AddOnWordDic(Dictionary<string, int> originDic, string addElement)
    {
        if (addElement == null || addElement == "") return originDic;
        if (originDic.ContainsKey(addElement))
        {
            originDic[addElement] += 1;
        }
        else
        {
            originDic[addElement] = 1;
        }
        return originDic;
    }
}
