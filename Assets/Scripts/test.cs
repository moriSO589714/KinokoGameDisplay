using Cysharp.Threading.Tasks;
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
    [SerializeField] CandidateBoxManager manager;

    void Start()
    {
        Vector2 testPos = new Vector2(668, 58);
        manager.InstCandidateBoxs(new List<string>(10) { "aaa", "bbb", "ccc", "ddd", "eee", "fff" }, testPos);
        Debug.Log("end");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            manager.SlidePerUnit(true);
        }
    }
    private void DLGame()
    {
        AllDirs allDirs = AllDirs.GetInstance();
        DriveService service = new CreateAPIService(allDirs.JsonPathKey).CreateDriveAPIService();

        OnNetDriveMetaData onNetDriveMetaData = new OnNetDriveMetaDatafromDv(service);
        OnNetDriveGetFile onNetDriveGetFile = new OnNetDriveGetFilefromDv(service);

        GameData testGameData = new GameData();
        testGameData.GameID = "1t2e3s4t5g6a7m8e9I10D11";
        testGameData.GameDriveId = "1OYVPHDX4IPq2r4ZVWzQI3cyjLGPS_36o";

        GameDLProc gameDLProc = new GameDLProc(onNetDriveMetaData, onNetDriveGetFile, testGameData);
        gameDLProc.DLGameInUniTask();
    }

    private void DLTestGame()
    {
        AllDirs allDirs = AllDirs.GetInstance();
        OnNetDriveMetaData onNetDriveMetaData = new OnNetDriveMetaDatafromTest();
        OnNetDriveGetFile onNetDriveGetFile = new OnNetDriveGetFilefromTest();
        GameData testGameData = new GameData();
        testGameData.GameID = "aaaaaaaaaaaaaa";
        testGameData.GameDirName = "test";
        GameDLProc gameDLProc = new GameDLProc(onNetDriveMetaData, onNetDriveGetFile, testGameData);
        gameDLProc.DLGameInUniTask();
        Debug.Log("endDLGame");
    }
}