using Google.Apis.Drive.v3;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class OnNetDriveUploadFileforDv : OnNetDriveUploadFile
{
    private DriveService _driveService;

    public OnNetDriveUploadFileforDv(DriveService driveService)
    {
        _driveService = driveService;
    }

    /// <summary>
    /// 特定のGoogleDrive上のディレクトリにローカルファイルをアップロードする処理
    /// </summary>
    public string UploadFile(string driveId, string filePath)
    {
        //アップロードするファイルのアップロード用リソースを作成する
        Google.Apis.Drive.v3.Data.File fileResource = new Google.Apis.Drive.v3.Data.File() 
        {
            Name = Path.GetFileName(filePath),
            Parents = new[] { driveId }
        };

        //アップロード用のファイルストリームを作成
        FileStream fileStream = new FileStream(filePath, FileMode.Open);

        //アップロード用リクエストの作成
        FilesResource.CreateMediaUpload request = _driveService.Files.Create(fileResource, fileStream, "application/" + Path.GetExtension(filePath));
        //アップロード時のドライブIDを返す用リクエストに含める
        request.Fields = "id";
        //アップロードの実行
        Google.Apis.Upload.IUploadProgress progress = request.Upload();

        //ファイルストリームを閉じる
        fileStream.Close();
        //アップロード終了時の処理
        if(progress.Status != Google.Apis.Upload.UploadStatus.Completed)
        {
            //アップロードが正常に終了しなかった場合
            throw new System.Exception("アップロードに失敗>>>" + progress.Status);
        }
        //正常終了した場合はGoogleDriveIDを返して終了
        return request.ResponseBody.Id;
    }
}
