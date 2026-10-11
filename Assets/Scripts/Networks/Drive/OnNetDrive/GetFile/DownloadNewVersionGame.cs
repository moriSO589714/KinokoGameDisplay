
public class DownloadNewVersionGame : DownloadGame
{
    protected override GameDlProc CreateGameDlProc(OnNetDriveGetFile onNetDriveGetFile, OnNetDriveMetaData onNetDriveMetaData, GameData gameData)
    {
        return new GameUpdateReceiveProc(onNetDriveMetaData, onNetDriveGetFile, gameData);
    }
}
