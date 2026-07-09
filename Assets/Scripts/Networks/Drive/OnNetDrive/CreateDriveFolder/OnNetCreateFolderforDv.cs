using Google.Apis.Drive.v3;

public class OnNetCreateFolderforDv : OnNetCreateFolder
{
    private DriveService _driveService;

    public OnNetCreateFolderforDv(DriveService driveService)
    {
        _driveService = driveService;
    }

    public string CreateFolder(string parentDriveId, string folderName)
    {
        //フォルダのリソースを作成
        Google.Apis.Drive.v3.Data.File fileResource = new Google.Apis.Drive.v3.Data.File() 
        {
            Name = folderName,
            MimeType = "application/vnd.google-apps.folder",
            Parents = new[] { parentDriveId }
        };

        //リクエストの作成
        FilesResource.CreateRequest request = _driveService.Files.Create(fileResource);
        //作成したドライブフォルダのドライブIDを返す用リクエストに含める
        request.Fields = "id";

        //フォルダの作成
        Google.Apis.Drive.v3.Data.File file = request.Execute();

        //ドライブIDを返して終了
        return file.Id;
    }
}
