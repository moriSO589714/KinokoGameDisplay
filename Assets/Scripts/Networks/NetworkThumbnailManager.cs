using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NetworkThumbnailManager
{
    public string UploadThumbnail
        (OnNetDriveUploadFile onNetDriveUploadFile, string parentDriveId, string localImagePath, string tmpFolderPath, string gameId)
    {
        //一時保存フォルダに画像のコピーを作成
        string fixFileName = gameId + Path.GetExtension(localImagePath);
        string tmpSavedPath = Path.Combine(tmpFolderPath, fixFileName); //コピー保存先のパス
        File.Copy(localImagePath, tmpSavedPath);

        //コピーをドライブにアップロード
        string picDriveId = onNetDriveUploadFile.UploadFile(parentDriveId, tmpSavedPath);

        //一時保存ファイルを削除
        File.Delete(tmpSavedPath);

        return picDriveId;
    }
}