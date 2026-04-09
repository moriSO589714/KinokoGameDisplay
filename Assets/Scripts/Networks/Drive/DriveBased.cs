using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System;
using System.IO;

public class DriveBased
{
    /// <summary>
    /// ドライブ接続用のAPIを作成する
    /// </summary>
    public DriveService CreateDriveAPI(string jsonKeyPath)
    {
        GoogleCredential credential;
        using (var stream = new FileStream(jsonKeyPath, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream).CreateScoped(DriveService.ScopeConstants.Drive);
        }

        //DriveAPIのサービス作成部分
        DriveService service = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "DriveService"
        });

        return service;
    }

    /// <summary>
    /// ドライブIDからファイルをダウンロードする
    /// </summary>
    public void DownloadFromDrive(DriveService driveService, string driveID, string downloadPath)
    {
        //メタデータを取得
        Google.Apis.Drive.v3.Data.File metadata = null;
        try
        {
            metadata = driveService.Files.Get(driveID).Execute();
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log("メタデータの取得に失敗しました Log>>>" + e);
            throw new Exception("failed to get metaData. Log>>>" + e);
        }

        //本ファイルのダウンロード
        FilesResource.GetRequest request = null;
        //リクエストの作成
        try
        {
            request = driveService.Files.Get(driveID);
        }
        catch(Exception e)
        {
            UnityEngine.Debug.Log("リクエストの作成に失敗しました Log>>>" + e);
            throw new Exception("failed to create request. Log>>>" + e);
        }

        //ファイルのダウンロード
        try
        {
            using (var stream = new FileStream(Path.Combine(downloadPath, metadata.Name), FileMode.Create, FileAccess.Write))
            {
                request.Download(stream);
            }

        }
        catch(Exception e)
        {
            UnityEngine.Debug.Log("ファイルのダウンロードに失敗しました。 Log>>>" + e);
            throw new Exception("failed to download the file. Log>>>" + e);
        }
    }
}
