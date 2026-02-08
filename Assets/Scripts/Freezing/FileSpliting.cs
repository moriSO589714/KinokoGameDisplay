using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEngine;

/// <summary>
/// フォルダをZIP圧縮して、チャンクで分割していくクラス
/// </summary>
public class FileSpliting : MonoBehaviour
{
    /// <summary>
    /// フォルダを圧縮するメソッド
    /// </summary>
    /// <param name="folderURL">zip化されるフォルダー</param>
    /// <param name="toFilePath">zip化したファイルのパス</param>
    public string PackagingFile(string folderURL,string toFilePath)
    {
        string zipFileName = Path.GetDirectoryName(folderURL) + ".zip";
        string tempFilePath = Path.Combine(toFilePath, zipFileName);

        try
        {
            if(File.Exists(tempFilePath)) File.Delete(tempFilePath);

            ZipFile.CreateFromDirectory(folderURL, tempFilePath, System.IO.Compression.CompressionLevel.Optimal, false, System.Text.Encoding.GetEncoding("shift_jis"));
        }
        catch(System.Exception e) 
        {
            Debug.Log("ファイルの圧縮に失敗しました\rエラー内容＞＞＞" + e);
            return null;
        }

        return tempFilePath;
    }

    /// <summary>
    /// ZIPファイルを分割するメソッド
    /// </summary>
    /// <param name="zipFilePath"></param>
    /// <param name="splicedBite">分割する要領の指定(単位はバイト)</param>
    public void DivideZipFile(int splicedBite ,string zipFilePath, string saveinPath)
    {
        //ファイルストリームを開く
        using (FileStream fs = new FileStream(zipFilePath, FileMode.Open, FileAccess.Read))
        {
            int chunkIndex = 0;
            int bytesRead;

            byte[] buffer = new byte[splicedBite];
            //FileSteam.Read()には自動で読み取った部分をシークしていく機能がついてる。返り値として、今回読み取ったバイト数が返される。0になった際に全ての読み込みが完了したことを表す
            //だからずっと同じFileStreamを使ってる
            //https://learn.microsoft.com/ja-jp/dotnet/api/system.io.filestream.read?view=net-8.0#system-io-filestream-read(system-byte()-system-int32-system-int32)
            while ((bytesRead = fs.Read(buffer, 0,buffer.Length)) > 0)
            {
                //保存先のパス
                string chunkFileName = string.Format("data.{0:D3}", chunkIndex);
                string chunkPath = Path.Combine(saveinPath, chunkFileName);

                //分割ファイルを書き出し
                using (FileStream outFs = new FileStream(chunkPath, FileMode.Create, FileAccess.Write))
                {
                    outFs.Write(buffer, 0, bytesRead);
                }

                chunkIndex++;
            }
        }
    }
}
