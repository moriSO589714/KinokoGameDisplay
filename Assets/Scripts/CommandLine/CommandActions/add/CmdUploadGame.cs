using Cysharp.Threading.Tasks;
using Google.Apis.Drive.v3;
using Google.Apis.Sheets.v4;
using System;
using System.Threading;
using UnityEngine;

public class CmdUploadGame : CmdActForUseNetwork
{
    OnNetDriveUploadFile _onNetDriveUploadFile;
    OnNetCreateFolder _onNetCreateFolder;
    OnNetAppEndGameInfo _onNetAppEndGameInfo;
    OnNetGetParentId _onNetGetParentId;
    OnNetDriveGetName _onNetDriveGetName;
    OnNetDelete _onNetDelete;
    CancellationTokenSource _ctsForUpload;

    public override void FirstCall()
    {
        base.FirstCall();
        _cmdSceneManager.OutPutManager.SendLogMessage("アップロードモードに変更します", OutPutTextLogColorSets.SystemDefault);
        _cmdSceneManager.InputFieldManager._endModeAction += () => { _ctsForUpload?.Cancel(); };

        //スプシのロード中にコマンドの受付を行わないようにしておく
        _cmdSceneManager.InputFieldManager.ChangeAction(new CmdNothing().MessageGird);

        try
        {
            PrepareUsingNetwork();
        }
        catch (Exception e)
        {
            if (_ctsForLoadSpreadSheet.IsCancellationRequested) return;
            _cmdSceneManager.OutPutManager.SendLogMessage("ゲーム情報の取得に失敗しました。モードを終了します。", OutPutTextLogColorSets.AccentDefault);
            ReturnCmdReceiveMode();
            Debug.LogException(e);
            return;
        }
    }

    protected override async UniTask LoadSpreadSheetData()
    {
        await base.LoadSpreadSheetData();
        if (_ctsForLoadSpreadSheet.IsCancellationRequested) return;
        OtherPrepare();
    }

    private void OtherPrepare()
    {
        CmdGameInfoInputerForAdd cmdGameInfoInputerForAdd = new CmdGameInfoInputerForAdd(ReceiveInputedGameData);
        CmdReturn cmdReturn = new CmdReturn(ReturnCmdReceiveMode);
        cmdGameInfoInputerForAdd.StartInputData(cmdReturn);
    }   

    protected override void Init()
    {
        if (CheckInEnvironment.isOnNet)
        {
            DriveService driveService = NetworksSingleton.Instance.ReturnDriveService();
            SheetsService sheetsService = NetworksSingleton.Instance.ReturnSheetsService();
            _onNetDriveUploadFile = new OnNetDriveUploadFileforDv(driveService);
            _onNetCreateFolder = new OnNetCreateFolderforDv(driveService);
            string sheetId = AllDirs.GetInstance().SpreadSheetID;
            _onNetAppEndGameInfo = new OnNetAppEndGameInfoToSpSt(sheetsService, sheetId);
            _onNetGetParentId = new OnNetGetParentIdfromDv(driveService);
            _onNetDriveGetName = new OnNetDriveGetNamefromDv(driveService);
            _onNetDelete = new OnNetDeleteforDv(driveService);
        }
        else
        {
            _onNetDriveUploadFile = new OnNetDriveUploadFileforTest();
            _onNetCreateFolder = new OnNetCreateFolderforTest();
            _onNetAppEndGameInfo = new OnNetAppEndGameInfoToTest();
            _onNetGetParentId = new OnNetGetParentIdfromTest();
            _onNetDriveGetName = new OnNetDriveGetNamefromTest();
            _onNetDelete = new OnNetDeleteforTest();
        }
    }

    protected override void End()
    {
        _ctsForUpload?.Cancel();
        _ctsForUpload = null;
    }
    
    private void ReceiveInputedGameData(GameData uploadGameData, string localGamePath, string localImagePath)
    {
        //アップロード開始のメソッド(MessageGirdに入力先を変えておく)
        _cmdSceneManager.InputFieldManager.ChangeAction(new CmdNothing().MessageGird);
        _cmdSceneManager.OutPutManager.SendLogMessage("アップロードを開始します", OutPutTextLogColorSets.SystemDefault);
        UploadGame(uploadGameData, localGamePath, localImagePath);
    }

    private async UniTask UploadGame(GameData uploadGameInfo, string localGamePath, string localImagePath)
    {
        string logId = _cmdSceneManager.OutPutManager.SendLogMessage("ゲーム情報を最適化中", OutPutTextLogColorSets.SystemDefault);
        GameData uploadGameData = GameDataForUpload.CreateGameDataForUpload(uploadGameInfo, localGamePath, localImagePath);
        _cmdSceneManager.OutPutManager.SendLogMessage("ゲーム情報の最適化が完了", OutPutTextLogColorSets.SystemDefault, specifiedUUID:logId);

        _ctsForUpload = new CancellationTokenSource();
        GameUpProgress gameUpProgress = new GameUpProgress();
        gameUpProgress.OnChangeProgressAct += (string message) => DuringUploadLogger(message, logId);
        GameUploadProc gameUploadProc = new GameUploadProc(_onNetCreateFolder, _onNetDriveUploadFile, _onNetAppEndGameInfo, _onNetGetParentId, _onNetDriveGetName, _onNetDelete);
        try
        {
            await gameUploadProc.UploadGameInUniTask(_ctsForUpload.Token, uploadGameData, gameUpProgress: gameUpProgress);
        }
        catch (Exception e) 
        {
            _cmdSceneManager.OutPutManager.SendLogMessage($"アップロード中にエラーが発生しました\nエラー内容>>{e}", OutPutTextLogColorSets.AccentDefault);
            Debug.Log(e);
        }
        ReturnCmdReceiveMode();
    }

    private void DuringUploadLogger(string message, string logId)
    {
        _cmdSceneManager.OutPutManager.SendLogMessage(message, OutPutTextLogColorSets.SystemDefault, specifiedUUID:logId);
    }
}