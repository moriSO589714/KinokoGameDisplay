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
        string jsonPathKey = AllDirs.GetInstance().JsonPathKey;
        string spID = AllDirs.GetInstance().SpreadSheetID;
        GameData gameData = new GameData();
        gameData.GameTags = new string[1] {"カードゲーム"};
        new SpreadSheetDataGet().GetGameDataOfSpreadSheet(gameData, jsonPathKey,spID);
        
        Debug.Log("end");
    }
}
