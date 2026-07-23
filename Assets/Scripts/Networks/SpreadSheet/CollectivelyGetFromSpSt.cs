using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class CollectivelyGetFromSpSt
{
    /// <summary>
    /// すべてのゲーム情報をスプレッドシートから取得する
    /// </summary>
    public List<GameData> AllGameDataFromSpSt()
    {
        NetworksSingleton networksSingleton = NetworksSingleton.Instance;
        List<string> elementOrder = networksSingleton.ReturnElementOrder(false);

        List<List<string>> allGameInfoList = networksSingleton.ReturnGameInfoAllData(false);
        List<GameData> returnList = new List<GameData>();
        //1行ずつGameDataクラスにする
        foreach(List<string> strList in allGameInfoList)
        {
            GameData createGameData = ElementOrderManager.SheetValuesToGameData(elementOrder, strList);
            if(createGameData != null) returnList.Add(createGameData);
        }
        return new List<GameData>(returnList);
    }
}