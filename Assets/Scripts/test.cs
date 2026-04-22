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
        /*
        SpreadSheetBased spreadSheetBased = new SpreadSheetBased();
        Dictionary<Vector2, string> dictionary = spreadSheetBased.ConvertWListintoDictionary( spreadSheetBased.ReturnSSValue(spreadSheetBased.CreateSSAPI(jsonPathKey),spID,new Vector2(2,3), new Vector2(5,6)));
        */
        
        GameData gameData = new GameData();
        gameData.GameTitle = "2DGolf";
        new SpreadSheetDataGet().FilterGameDataFromSpreadSheet(gameData, jsonPathKey,spID);
        
        Debug.Log("end");
    }
}
