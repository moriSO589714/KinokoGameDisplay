using DG.Tweening.Plugins;
using Google.Apis.Drive.v3;
using Google.Apis.Sheets.v4;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class test : MonoBehaviour
{
    [SerializeField] GameObject obj;

    // Start is called before the first frame update
    void Start()
    {
        new LoadFlexibleDir().SetFlexibleDirByJson();
        AllDirs allDirs = AllDirs.GetInstance();
        string jsonPathKey = allDirs.JsonPathKey;
        string spStId = allDirs.SpreadSheetID;
        SheetsService sheetsService = new CreateAPIService(jsonPathKey).CreateSheetAPIService();
        NetworksSingleton networksSingleton = NetworksSingleton.Instance;

        networksSingleton.ReturnElementOrder(true);
        /*
        GameData g = new GameData();
        g.GameTags = new string[1] { "アクション" };
        List<GameData> gameDatas =  new CollectivelyGetFromSpSt().FilterGameDataFromSpSt(g);
        */
        Debug.Log("last");
    }
}
