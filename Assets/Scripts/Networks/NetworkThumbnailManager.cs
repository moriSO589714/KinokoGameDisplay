using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NetworkThumbnailManager
{
    public string UploadThumbnail
        (OnNetDriveUploadFile onNetDriveUploadFile, string parentDriveId, string localImagePath, string tmpFolderPath, string gameId)
    {
        AllDirs allDirs = AllDirs.GetInstance();
        string localImageExtention = Path.GetExtension(localImagePath);
        if (allDirs.ImageExtention != localImageExtention)
        {
            throw new System.Exception("指定されたサムネイル画像の拡張子は対応していないものです。対応しているファイル形式＞＞＞" + allDirs.ImageExtention);
        }

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