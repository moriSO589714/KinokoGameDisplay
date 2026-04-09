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
        new EachDataLoad().LocalDataLoad();
        DriveService driveService = new DriveBased().CreateDriveAPI(AllDirs.GetInstance().JsonPathKey);
        try
        {
            new DriveBased().DownloadFromDrive(driveService, "1e0LSKmtMHu2jkT84DfbEKAIojIhcEFqL", "KinokinoAsobitai");
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

        Debug.Log("end");
        /*
        new EachDataLoad().LocalDataLoad();
        obj.GetComponent<GameBoxsManager>().SetGameBoxsByGameDataList(GameDatasSingleton.Instance.GameDatas);
        Debug.Log("End");
        */
    }
}
