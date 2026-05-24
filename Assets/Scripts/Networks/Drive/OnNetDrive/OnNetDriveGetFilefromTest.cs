using System.Diagnostics;
using System.IO;
using System.IO.Compression;

/// <summary>
/// テスト用のデータを作成して保存する
/// </summary>
public class OnNetDriveGetFilefromTest : OnNetDriveGetFile
{
    private OnNetDriveTestData onNetDriveTestData = new OnNetDriveTestData();

    public void GetFile(string driveId, string fileName, string dledPath)
    {
        string filePath = Path.Combine(dledPath, fileName);
        //ファイルパスの拡張子部
        string fileExtension = Path.GetExtension(filePath);
        DLData testDLData = onNetDriveTestData.TestDLData;

        if(fileExtension == testDLData.DLDataFileExtention)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                fs.Write(testDLData.ReturnByteData());
            }
        }
        else
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                fs.Write(onNetDriveTestData.TestFileByte);
            }
            string onlyFileName = Path.GetFileName(filePath);
            string zipFilePath = Path.Combine(dledPath, onlyFileName + ".zip");

            //空のzip書庫を作成する
            using (ZipArchive zipArc = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
            {
                //CreateEntryFromFileで空のzip書庫にファイルを追加する
                ZipArchiveEntry e = zipArc.CreateEntryFromFile(filePath, Path.GetFileName(filePath));
            }

            //もとファイルの削除
            File.Delete(filePath);
            //拡張子の変更
            File.Move(zipFilePath, filePath);
        }
    }
}
