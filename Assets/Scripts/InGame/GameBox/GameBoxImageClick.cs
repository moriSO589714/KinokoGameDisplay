using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GameBoxImageClick
{
    List<string> downloadingImagesList = new List<string>();

    public void OnClickAction(GameBox targetGameBox, CancellationToken ct)
    {
        StartDownloadImage(targetGameBox, ct);
    }

    private async UniTask StartDownloadImage(GameBox targetGameBox, CancellationToken ct)
    {
        GameData targetGameData = targetGameBox._myGameData;
        string driveId = targetGameData.GameImageId;
        if (downloadingImagesList.Contains(driveId))
        {
            return;
        }

        downloadingImagesList.Add(driveId);

        NetworksSingleton networksSingleton = NetworksSingleton.Instance;
        OnNetDriveGetFile onNetDriveGetFile;
        if (CheckInEnvironment.isOnNet)
        {
            onNetDriveGetFile = new OnNetDriveGetFilefromDv(networksSingleton.ReturnDriveService());
        }
        else
        {
            onNetDriveGetFile = new OnNetDriveGetFilefromTest();
        }
        ImageDlProc imageDlProc = new ImageDlProc(onNetDriveGetFile, targetGameData);

        await imageDlProc.DLImageInUniTask(ct);

        targetGameBox.SetImage(targetGameData.GameID);
        downloadingImagesList.Remove(targetGameData.GameImageId);
        targetGameBox.EndImageLoading?.Invoke();
    }
}
