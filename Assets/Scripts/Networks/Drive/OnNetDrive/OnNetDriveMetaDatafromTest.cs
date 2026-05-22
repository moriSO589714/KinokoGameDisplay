using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// OnNetDriveTestDataからテスト用のメタデータを返す
/// </summary>
public class OnNetDriveMetaDatafromTest : OnNetDriveMetaData
{
    private OnNetDriveTestData testData = new OnNetDriveTestData();

    public Dictionary<string, string> GetFileList(string driveFolderId)
    {
        return testData.MetaTestData;
    }
}
