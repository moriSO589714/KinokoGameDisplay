using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using System.IO;

/// <summary>
/// APIを利用してドライブにデータ保存リクエストを送る
/// </summary>
public class OnNetDriveGetFilefromDv : OnNetDriveGetFile
{
    private DriveService _driveService;

    public OnNetDriveGetFilefromDv(DriveService driveService)
    {
        _driveService = driveService;
    }

    /// <summary>
    /// APIを介してファイルを保存してくる処理
    /// </summary>
    public void GetFile(string driveId, string fileName, string dledPath)
    {
        //リクエストの作成
        var request = _driveService.Files.Get(driveId);
        string filePath = Path.Combine(dledPath, fileName);
        IDownloadProgress progress;
        using (FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
             progress = request.DownloadWithStatus(stream);
        }

        if(progress.Status != DownloadStatus.Completed)
        {
            throw new System.Exception("ダウンロードに失敗しました。", progress.Exception);
        }
    }
}
