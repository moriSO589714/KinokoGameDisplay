using Google.Apis.Drive.v3;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cmdTest : MonoBehaviour
{
    void Start()
    {
        DriveService service = NetworksSingleton.Instance.ReturnDriveService();
        OnNetCreateFolderforDv onc = new OnNetCreateFolderforDv(service);
        string parentDriveId = "1tUoCEh_TpSAGIffX5S9CvkzzBRotksam";
        string folderName = "ImTestFolder111";
        string resultDriveId = onc.CreateFolder(parentDriveId, folderName);
        Debug.Log("resultDriveId>>>" + resultDriveId);
        Debug.Log("end");
    }

    private void UploadFile()
    {
        DriveService service = NetworksSingleton.Instance.ReturnDriveService();
        OnNetDriveUploadFileforDv ond = new OnNetDriveUploadFileforDv(service);
        string driveId = "1tUoCEh_TpSAGIffX5S9CvkzzBRotksam";
        string filePath = "E:/GameCreate/Projects/Unity/KinokoGameDisplay/ForTestAppSliced/ForTestApp.001";
        string driveID = ond.UploadFile(driveId, filePath);
        Debug.Log("driveId>>>" + driveID);
    }
}
