using Google.Apis.Drive.v3;

public class OnNetDriveGetNamefromDv : OnNetDriveGetName
{
    private DriveService _driveService;

    public OnNetDriveGetNamefromDv(DriveService driveService)
    {
        _driveService = driveService;
    }

    public string GetFolderName(string driveId)
    {
        var request = _driveService.Files.Get(driveId);

        string folderName = "";
        try
        {
            var result = request.Execute();
            folderName = result.Name;
        }
        catch (Google.GoogleApiException ex)
        {
            throw new System.Exception("ドライブ上のフォルダ名取得に失敗しました", ex);
        }
        return folderName;
    }
}
