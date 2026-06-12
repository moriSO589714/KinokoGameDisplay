using Google.Apis.Drive.v3;

public class DownloadGame
{
    /// <summary>
    /// ゲームデータ型からゲームダウンロードのタスク化とキューへの追加を行う
    /// </summary>
    public GameDlTask CreateGameDlTaskAndAddCue(GameData gameData, GameDlCue _cue)
    {
        DriveService driveService = NetworksSingleton.Instance.ReturnDriveService();
        OnNetDriveGetFile onNetDriveGetFile = null;
        OnNetDriveMetaData onNetDriveMetaData = null;
        if (CheckInEnvironment.CheckDoingNet())
        {
            onNetDriveGetFile = new OnNetDriveGetFilefromDv(driveService);
            onNetDriveMetaData = new OnNetDriveMetaDatafromDv(driveService);
        }
        else
        {
            onNetDriveGetFile = new OnNetDriveGetFilefromTest();
            onNetDriveMetaData = new OnNetDriveMetaDatafromTest();
        }

        GameDLProc gameDLProc = new GameDLProc(onNetDriveMetaData, onNetDriveGetFile, gameData);
        GameDlTask returnTask = new GameDlTask(gameDLProc);
        _cue.AddGameDlTask(returnTask);
        return returnTask;
    }
}
