using Cysharp.Threading.Tasks;
using Google.Apis.Drive.v3;
using Google.Apis.Sheets.v4;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

/// <summary>
/// オンライン上(スプシ)のゲームデータを変更する
/// </summary>
public class CmdUpdateGameOnline : CmdActForUseNetwork
{
    private WordEmtCell _idLib;
    private List<GameData> _allGameDatas;

    private CancellationTokenSource _ctsForUpdate;

    private OnNetDelete _onNetDelete;
    private OnNetGetParentId _onNetGetParentId;
    private OnNetCreateFolder _onNetCreateFolder;
    private OnNetDriveUploadFile _onNetDriveUploadFile;
    private OnNetAppEndGameInfo _onNetAppEndGameInfo;
    private OnNetUpdateGameInfo _onNetUpdateGameInfo;
    private OnNetDriveGetName _onNetDriveGetName;

    protected override void Init()
    {
        base.Init();
        AllDirs allDirs = AllDirs.GetInstance();

        if (CheckInEnvironment.isOnNet)
        {

            NetworksSingleton networksSingleton = NetworksSingleton.Instance;
            DriveService driveService = networksSingleton.ReturnDriveService();
            SheetsService sheetsService = networksSingleton.ReturnSheetsService();
            _onNetDelete = new OnNetDeleteforDv(driveService);
            _onNetGetParentId = new OnNetGetParentIdfromDv(driveService);
            _onNetCreateFolder = new OnNetCreateFolderforDv(driveService);
            _onNetDriveUploadFile = new OnNetDriveUploadFileforDv(driveService);
            _onNetAppEndGameInfo = new OnNetAppEndGameInfoToSpSt(sheetsService, allDirs.SpreadSheetID);
            _onNetUpdateGameInfo = new OnNetUpdateGameInfoToSpSt(sheetsService, allDirs.SpreadSheetID);
            _onNetDriveGetName = new OnNetDriveGetNamefromDv(driveService);
        }
        else
        {
            _onNetDelete = new OnNetDeleteforTest();
            _onNetGetParentId = new OnNetGetParentIdfromTest();
            _onNetCreateFolder = new OnNetCreateFolderforTest();
            _onNetDriveUploadFile = new OnNetDriveUploadFileforTest();
            _onNetAppEndGameInfo = new OnNetAppEndGameInfoToTest();
            _onNetUpdateGameInfo = new OnNetUpdateGameInfoToTest();
            _onNetDriveGetName = new OnNetDriveGetNamefromTest();
        }
    }

    public override void FirstCall()
    {
        base.FirstCall();
        _cmdSceneManager.OutPutManager.SendLogMessage("オンラインアップデートモードに変更します", OutPutTextLogColorSets.SystemDefault);
        _ctsForUpdate = new CancellationTokenSource();

        try
        {
            PrepareUsingNetwork();
        }
        catch(System.Exception e)
        {
            if (_ctsForLoadSpreadSheet.IsCancellationRequested) return;

            _cmdSceneManager.OutPutManager.SendLogMessage("ゲーム情報の取得に失敗しました。モードを終了します", OutPutTextLogColorSets.AccentDefault);
            ReturnCmdReceiveMode();
            Debug.Log(e);
            return;
        }
    }

    protected override async UniTask LoadSpreadSheetData()
    {
        await base.LoadSpreadSheetData();
        if (_ctsForLoadSpreadSheet.IsCancellationRequested) return;

        GameDatasSingleton gameDatasSingleton = GameDatasSingleton.Instance;
        _allGameDatas = gameDatasSingleton.AllGameDatas;
        _idLib = CreateLibFromGameDatas.CreateGameIdLib(_allGameDatas);
        _cmdSceneManager.OutPutManager.SendLogMessage("アップデートするゲームのIDを送信してください", OutPutTextLogColorSets.SystemDefault);
        _cmdSceneManager.InputFieldManager.ChangeAction(SelectGame, _idLib);
    }

    private void SelectGame(string message)
    {
        List<GameData> matchGameDatas = new List<GameData>();
        matchGameDatas = _allGameDatas.Where(x => x.GameID == message).ToList();

        GameData matchGameData = new GameData();
        if(matchGameDatas.Count == -1)
        {
            _cmdSceneManager.OutPutManager.SendLogMessage("送信されたIDのゲームは存在しません。IDを送信し直してください", OutPutTextLogColorSets.AccentDefault);
            return;
        }
        else if (matchGameDatas.Count > 1)
        {
            _cmdSceneManager.OutPutManager.SendLogMessage("該当するIDのゲームが複数存在します。IDを送信し直してください", OutPutTextLogColorSets.AccentDefault);
            return;
        }
        else if(matchGameDatas.Count == 1)
        {
            matchGameData = matchGameDatas[0];
        }

        StartDataInput(new GameData(matchGameData), matchGameData);
    }

    private void StartDataInput(GameData matchGameData, GameData originData)
    {
        CmdGameInfoInputerForUpdate cmdGameInfoInputerForUpdate = new CmdGameInfoInputerForUpdate
            ((GameData writeGameData, string localPath, string localImagePath) => ReceiveInputedGameData(writeGameData, localPath, localImagePath, originData), matchGameData);
        CmdReturn cmdReturn = new CmdReturn(ReturnCmdReceiveMode);
        cmdGameInfoInputerForUpdate.StartInputData(cmdReturn);
    }

    private void ReceiveInputedGameData(GameData updatedGameData, string updateLocalGamePath, string updateLocalImagePath, GameData originGameData)
    {
        //ユーザー入力のものから文字の置き換えなど最適化を行う
        GameData useGameData = GameDataForUpload.CreateGameDataForUpload(updatedGameData, updateLocalGamePath, updateLocalImagePath);
        UploadGame(useGameData, originGameData);
    }

    private async UniTask UploadGame(GameData uploadGameData, GameData originGameData)
    {
        GameUpProgress progress = new GameUpProgress();
        string logId = _cmdSceneManager.OutPutManager.SendLogMessage("アップデートを開始します", OutPutTextLogColorSets.SystemDefault);
        progress.OnChangeProgressAct = (string val) => _cmdSceneManager.OutPutManager.SendLogMessage(val, OutPutTextLogColorSets.SystemDefault, specifiedUUID: logId);
        GameUpdateProc gameUpdateProc = new GameUpdateProc(_onNetDelete, _onNetGetParentId, _onNetCreateFolder, _onNetDriveUploadFile, _onNetAppEndGameInfo, _onNetUpdateGameInfo, _onNetDriveGetName);

        await gameUpdateProc.UpdateGameInUniTask(_ctsForUpdate.Token, newGameData:uploadGameData, originGameData:originGameData,false ,progress);
        
        _cmdSceneManager.OutPutManager.SendLogMessage("アップデートが終了しました", OutPutTextLogColorSets.SystemDefault);
        ReturnCmdReceiveMode();
    }
}