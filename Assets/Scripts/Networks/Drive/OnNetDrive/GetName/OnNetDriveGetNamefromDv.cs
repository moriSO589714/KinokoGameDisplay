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

        var result = request.Execute();
        string folderName = result.Name;
        return folderName;
    }
}
