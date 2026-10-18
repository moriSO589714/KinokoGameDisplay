using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GameImageDownloaderForUI : MonoBehaviour
{
    List<string> downloadingImagesList = new List<string>();
    private CancellationTokenSource cts = new CancellationTokenSource();

    public void OnClickAction(GameData targetGameData, Action<string> setImageAct, Action endImageDownloadAct)
    {
        StartDownloadImage(targetGameData, setImageAct ,endImageDownloadAct);
    }

    private async UniTask StartDownloadImage(GameData targetGameData, Action<string> setImageAct, Action endImageDownloadAct = null)
    {
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

        try
        {
            await imageDlProc.DLImageInUniTask(cts.Token);
            setImageAct?.Invoke(targetGameData.GameID);
        }
        catch (Exception e) 
        {
            Debug.LogException(e);
        }
        
        downloadingImagesList.Remove(targetGameData.GameImageId);
        endImageDownloadAct?.Invoke();
    }

    private void OnDestroy()
    {
        if (cts != null) cts.Cancel();
    }
}
