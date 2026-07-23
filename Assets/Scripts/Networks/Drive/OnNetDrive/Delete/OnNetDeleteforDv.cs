using Google.Apis.Drive.v3;

public class OnNetDeleteforDv : OnNetDelete
{
    private DriveService _driveService;
    public OnNetDeleteforDv(DriveService driveService)
    {
        _driveService = driveService;
    }

    /// <summary>
    /// 指定されたDriveIdのフォルダを削除する
    /// </summary>
    public void DeleteFolder(string driveId)
    {
        //リクエストを作成
        var request = _driveService.Files.Delete(driveId);
        //リクエストを実行
        request.Execute();
    }
}
