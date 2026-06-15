using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using UnityEngine;

/// <summary>
/// テスト用のデータを作成して保存する
/// </summary>
public class OnNetDriveGetFilefromTest : OnNetDriveGetFile
{
    const string testSlicedGamePath = "ForTestAppSliced";
    private OnNetDriveTestData onNetDriveTestData = new OnNetDriveTestData();

    public void GetFile(string driveId, string fileName, string dledPath)
    {
        //デフォルトでプロジェクトフォルダ直下にある最小構成のアプリを指定のフォルダにコピーする
        string copyFilePath = Path.Combine(testSlicedGamePath, fileName);
        string dlPath = Path.Combine(dledPath, fileName);

        //ダウンロード時の遅延を発生させる
        System.Threading.Thread.Sleep((int)Mathf.Floor(CheckInEnvironment.waitSecondsOnDownload * 1000));
        //ファイルをコピー
        File.Copy(copyFilePath, dlPath, true);
    }
}
