using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEngine;

/// <summary>
/// フォルダをZIP圧縮して、チャンクで分割していくクラス
/// </summary>
public class FileSpliting
{
    /// <summary>
    /// フォルダを圧縮するメソッド
    /// </summary>
    /// <param name="gameDataPath">ゲームデータが入ったフォルダのパス</param>
    /// <param name="toFilePath">zip化したファイルのパス</param>
    public string PackagingFile(string gameDataPath,string toFilePath)
    {
        //.zipの拡張子を付けて、圧縮ファイルのファイル名のみを作る（※パスではないため注意！！）
        string zipFileName = Path.GetFileName(gameDataPath) + ".zip";
        //toFilePathで指定された場所に圧縮ファイルが置かれるようなパスを生成する。
        string tempFilePath = Path.Combine(toFilePath, zipFileName);

        try
        {
            //すでに圧縮された同名のファイルが存在した場合、そのファイルを削除する
            if(File.Exists(tempFilePath)) File.Delete(tempFilePath);

            //shift_jisを指定して圧縮
            ZipFile.CreateFromDirectory(gameDataPath, tempFilePath, System.IO.Compression.CompressionLevel.Optimal, false, System.Text.Encoding.GetEncoding("shift_jis"));
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
    /// <param name="splicedBite">分割する要領の指定(単位はバイト)</param>
    /// <param name="splicedFilePath">分割されるファイルのパス</param>
    /// <param name="saveinPath">分割後のファイル群が保存されるフォルダのパス</param>
    public void DivideZipFile(int splicedBite ,string splicedFilePath, string saveinPath)
    {
        //ファイルサイズ
        FileInfo fileInfo = new FileInfo(splicedFilePath);
        long fileSize = fileInfo.Length;
        //ファイル名
        string fileName = Path.GetFileNameWithoutExtension(splicedFilePath);

        long chunkIndex = 1;
        //ファイルストリームを開く
        using (FileStream fs = new FileStream(splicedFilePath, FileMode.Open, FileAccess.Read))
        {
            int bytesRead;

            byte[] buffer = new byte[splicedBite];
            //FileSteam.Read()には自動で読み取った部分をシークしていく機能がついてる。返り値として、今回読み取ったバイト数が返される。0になった際に全ての読み込みが完了したことを表す
            //だからずっと同じFileStreamを使ってる
            //https://learn.microsoft.com/ja-jp/dotnet/api/system.io.filestream.read?view=net-8.0#system-io-filestream-read(system-byte()-system-int32-system-int32)
            while ((bytesRead = fs.Read(buffer, 0,buffer.Length)) > 0)
            {
                //保存先のパス string.Format()の使い方に注意
                string chunkFileName = string.Format("{0}.{1:D3}", fileName, chunkIndex);
                string chunkPath = Path.Combine(saveinPath, chunkFileName);

                //分割ファイルを書き出し
                using (FileStream outFs = new FileStream(chunkPath, FileMode.Create, FileAccess.Write))
                {
                    outFs.Write(buffer, 0, bytesRead);
                }

                chunkIndex++;
            }
        }

        //チャンクインデックス00(.00)にDL情報のファイル（容量、ZIPファイル名等）を置いておく
        DLData createdDLData = new DLData(fileSize, fileName, chunkIndex);
        string dlDataFileName = $"{fileName}.00";
        string dlDataPath = Path.Combine(saveinPath, dlDataFileName);
        using (FileStream dlDataFs = new FileStream(dlDataPath, FileMode.Create, FileAccess.Write))
        {
            dlDataFs.Write(createdDLData.ReturnByteData());
        }

        //zipファイルを削除する
        File.Delete(splicedFilePath);
    }
}
