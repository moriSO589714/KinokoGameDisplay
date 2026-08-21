using Google.Apis.Drive.v3;
using Google.Apis.Sheets.v4;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class cmdTest : MonoBehaviour
{
    CancellationTokenSource _cts = null;
    [SerializeField] EstimateCmdLibManager _object;
    [SerializeField] OutputManager _outputManager;
    string uuid = "";
    int counter = 0;

    void Start()
    {        
        Debug.Log("end");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {

        }
    }

    private void OnDisable()
    {
        if(_cts != null)
        {
            _cts.Cancel();
        }
    }

    private void DeleteGame()
    {
        DriveService driveService = NetworksSingleton.Instance.ReturnDriveService();
        OnNetGetParentId onNetGetParentId = new OnNetGetParentIdfromDv(driveService);
        OnNetDriveGetName onNetDriveGetName = new OnNetDriveGetNamefromDv(driveService);
        OnNetDelete onNetDelete = new OnNetDeleteforDv(driveService);
        
        DeleteProc deleteProc = new DeleteProc(onNetDelete, onNetGetParentId, onNetDriveGetName);

        string gameDriveId = "1lWS7yJHf2dob4vUx-gOLqDaM18vFRMj-";
        string gameOriginalId = "cec80c1b3d864d0c892d0e3887c3e6d4";
        CancellationTokenSource cts = new CancellationTokenSource();

        deleteProc.UniDeleteDriveGame(gameDriveId, gameOriginalId, cts.Token);
    }

    private void DeleteSheetRow()
    {
        SheetsService sheetsService = NetworksSingleton.Instance.ReturnSheetsService();
        string sheetId = AllDirs.GetInstance().SpreadSheetID;
        OnNetDeleteRow onNetDeleteRow = new OnNetDeleteRowToSpSt(sheetsService, sheetId);
        int numOfDelete = 31;
        onNetDeleteRow.DeleteRow(numOfDelete);
    }

    private void DeleteGameData()
    {
        DriveService driveService = NetworksSingleton.Instance.ReturnDriveService();
        OnNetGetParentId onNetGetParentId = new OnNetGetParentIdfromDv(driveService);
        OnNetDriveGetName onNetDriveGetName = new OnNetDriveGetNamefromDv(driveService);
        OnNetDelete onNetDelete = new OnNetDeleteforDv(driveService);
        string gameOriginalId = "9ba0c82c20304c41bdb7ff732913a048";
        string gameDriveId = "1kTzz84pzgbu5VKG91aQfkDoZoIZG6Fv9";

        CancellationTokenSource cts = new CancellationTokenSource();
        new DeleteProc(onNetDelete, onNetGetParentId, onNetDriveGetName).UniDeleteDriveGame(gameDriveId, gameOriginalId, cts.Token);
    }

    private void ReferenceParentId(string childId)
    {
        DriveService driveService = NetworksSingleton.Instance.ReturnDriveService();
        OnNetGetParentId onNetGetParentId = new OnNetGetParentIdfromDv(driveService);
        string testDriveId = "1iQzQnnhjO6NDSIiGUr9EEfjJaEMR51_0";
        string parentsId = onNetGetParentId.GetParentId(testDriveId);
        OnNetDriveGetName onNetDriveGetName = new OnNetDriveGetNamefromDv(driveService);
        string parentName = onNetDriveGetName.GetFolderName(parentsId);
        Debug.Log("ParentFolderName>>>" + parentName);
    }

    private void TryGameUploadProc()
    {
        GameData gameData = new GameData();
        gameData.GameTitle = "TestUploadGameTitle";
        gameData.GameExeName = "TestUploadGameExeName";
        gameData.GameDescription = "TestGameNOSetumei";
        gameData.GameDevelopper = new string[2] { "TestGameDev1", "TestGameDev2" };
        gameData.GameSoftwareType = "Unity";
        gameData.GameTags = new string[3] { "アクションゲーム", "脱出ゲーム", "パズル" };

        string localGamePath = "C:/Users/souza/Downloads/ForTestApp";
        string localImagePath = "C:/Users/souza/Pictures/スクリーンショット/Screenshot 2026-02-17 093932.png";

        DriveService driveService = NetworksSingleton.Instance.ReturnDriveService();
        OnNetDriveUploadFile ondu = new OnNetDriveUploadFileforDv(driveService);
        OnNetCreateFolder onc = new OnNetCreateFolderforDv(driveService);
        SheetsService sheetsService = NetworksSingleton.Instance.ReturnSheetsService();
        string sheetId = AllDirs.GetInstance().SpreadSheetID;
        OnNetAppEndGameInfoToSpSt appendInfo = new OnNetAppEndGameInfoToSpSt(sheetsService, sheetId);

        OnNetGetParentId onNetGetParentId = new OnNetGetParentIdfromDv(driveService);
        OnNetDriveGetName onNetDriveGetName = new OnNetDriveGetNamefromDv(driveService);
        OnNetDelete onNetDelete = new OnNetDeleteforDv(driveService);

        GameData uploadGameDetail = GameDataForUpload.CreateGameDataForUpload(gameData, localGamePath, localImagePath);
        CancellationTokenSource cts = new CancellationTokenSource();
        _cts = cts;
        new GameUploadProc(onc, ondu, appendInfo, onNetGetParentId, onNetDriveGetName, onNetDelete).UploadGameInUniTask(cts.Token, uploadGameDetail);
    }

    private void TryUploadImage()
    {
        NetworkThumbnailManager networkThumbnailManager = new NetworkThumbnailManager();
        DriveService driveService = NetworksSingleton.Instance.ReturnDriveService();
        OnNetDriveUploadFile ondu = new OnNetDriveUploadFileforDv(driveService);

        string parentDriveId = "1tUoCEh_TpSAGIffX5S9CvkzzBRotksam";
        string localImagePath = "E:/pictures/PICT0011.jpg";
        string tmpFolderPath = "E:/GameCreate/Projects/Unity/KinokoGameDisplay/KinokinoAsobitai";
        string gameId = "TESTGAMEID441";
        string picDriveId = networkThumbnailManager.UploadThumbnail(ondu, parentDriveId, localImagePath, tmpFolderPath, gameId);
        Debug.Log("uploadDriveId>>>" + picDriveId);
    }

    private void GameDatatoSheetFormat()
    {
        GameData demoGameData = new GameData();
        demoGameData.GameTitle = "demoGameTitle";
        demoGameData.GameTags = new string[3] { "demoTag1", "demoTag2", "demoTag3" };
        demoGameData.Status = GameStatus.NotDownloaded;

        NetworksSingleton networksSingleton = NetworksSingleton.Instance;
        List<string> elementOrder = networksSingleton.ReturnElementOrder(false);
        List<string> result = ElementOrderManager.GameDataToSheetFormat(elementOrder, demoGameData);
        Debug.Log(result);
    }

    private void AddSheetRangeData()
    {
        SheetsService sheetsService = NetworksSingleton.Instance.ReturnSheetsService();
        string sheetId = AllDirs.GetInstance().SpreadSheetID;
        OnNetAppEndGameInfoToSpSt appendInfo = new OnNetAppEndGameInfoToSpSt(sheetsService, sheetId);
        List<string> addDemoData = new List<string>()
        {
            "test"
        };
        appendInfo.AppEndGameInfo(addDemoData);
    }

    private void CreateDriveFolder()
    {
        DriveService service = NetworksSingleton.Instance.ReturnDriveService();
        OnNetCreateFolderforDv onc = new OnNetCreateFolderforDv(service);
        string parentDriveId = "1tUoCEh_TpSAGIffX5S9CvkzzBRotksam";
        string folderName = "ImTestFolder111";
        string resultDriveId = onc.CreateFolder(parentDriveId, folderName);
        Debug.Log("resultDriveId>>>" + resultDriveId);
    }

    private void UploadFile()
    {
        DriveService service = NetworksSingleton.Instance.ReturnDriveService();
        OnNetDriveUploadFileforDv ond = new OnNetDriveUploadFileforDv(service);
        string driveId = "1tUoCEh_TpSAGIffX5S9CvkzzBRotksam";
        string filePath = "E:/GameCreate/Projects/Unity/KinokoGameDisplay/ForTestAppSliced/ForTestApp.001";
        string driveID = ond.UploadFile(driveId, filePath);
        Debug.Log("driveId>>>" + driveID);
    }

    
}
