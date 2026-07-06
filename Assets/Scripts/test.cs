using Cysharp.Threading.Tasks;
using DG.Tweening.Plugins;
using Google.Apis.Drive.v3;
using Google.Apis.Sheets.v4;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class test : MonoBehaviour
{
    [SerializeField] GameObject _arrow;

    void Start()
    {
        RectTransform rectTransform = _arrow.GetComponent<RectTransform>();
        Debug.Log("Rotation>>>" + rectTransform.localEulerAngles);
        Debug.Log("end");
    }

    public void TestMethod()
    {

    }

    void Update()
    {
    }

    private void DLGame()
    {
        AllDirs allDirs = AllDirs.GetInstance();
        NetworksSingleton networksSingleton = NetworksSingleton.Instance;
        DriveService service = networksSingleton.ReturnDriveService();

        //メタデータ取得用のメソッドのあるクラスを生成
        OnNetDriveMetaData onNetDriveMetaData = new OnNetDriveMetaDatafromDv(service);
        //ゲームダウンロード用のクラスを生成
        OnNetDriveGetFile onNetDriveGetFile = new OnNetDriveGetFilefromDv(service);

        GameData testGameData = new GameData();
        testGameData.GameID = "1t2e3s4t5g6a7m8e9I10D11";
        testGameData.GameDriveId = "1OYVPHDX4IPq2r4ZVWzQI3cyjLGPS_36o";

        GameDlProc gameDLProc = new GameDlProc(onNetDriveMetaData, onNetDriveGetFile, testGameData);
        CancellationTokenSource cts = new CancellationTokenSource();
        gameDLProc.DLGameInUniTask(cts.Token);
    }

    /*
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
    */
}