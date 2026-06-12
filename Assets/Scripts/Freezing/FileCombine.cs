using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public class FileCombine
{
    /// <summary>
    /// 分割されたZIPファイルを結合して解凍するメソッド
    /// </summary>
    /// <param name="splitedFilesPath">結合されるファイル群が置かれているディレクトリのパス</param>
    /// <param name="gameDataPath">完成したフォルダを置くパス</param>
    public void MergeSplitedFile(string splitedFilesPath, string gameDataPath)
    {
        //対象ディレクトリ内のファイルのパスを取得してくる
        string[] splitedFiles = Directory.GetFiles(splitedFilesPath);

        MistakeFiles mistakeFiles = new FreezingTools().hasAllRequiredData(splitedFilesPath);
        if(mistakeFiles.LackFiles.Count() != 0)
        {
            throw new Exception("ファイルの欠損を確認しました");
        }
        else if (mistakeFiles.ErrorFilePathes.Count() != 0)
        {
            throw new Exception("ファイル群のあるフォルダに問題のあるファイルがあります。");
        }

        //データ群を拡張子でソート
        string[] sortedFiles = new FreezingTools().sortingFilesByPath(splitedFilesPath);
        DLData targetDLData = new DLData();
        //DLDataクラスにソート後の一番始めに来るファイル(.00)をデシリアライズさせて、データを格納
        targetDLData.DeserializeDataByFilePath(sortedFiles[0]);

        //targetDLDataから結合するゲームの詳細情報を取得する
        string dlFileName = targetDLData.FileName;
        long splitedFileNum = targetDLData.SplitFileNum;

        //結合後にファイルを置くパスを生成
        string zipFileDirPath = Path.Combine(splitedFilesPath, "zipFile");
        DirectoryActs.CreateAndCheckDir(zipFileDirPath);
        string margedFilePath = Path.Combine(zipFileDirPath, dlFileName + ".zip");
        //データを結合する処理
        using (FileStream outFs = new FileStream(margedFilePath, FileMode.Create,FileAccess.Write))
        {
            for (int i = 1; i < sortedFiles.Length; i++)
            {
                byte[] bytedatas = File.ReadAllBytes(sortedFiles[i]);
                outFs.Write(bytedatas, 0, bytedatas.Length);
            }
        }

        //既に解凍されたフォルダが指定のパスに存在した場合完全消去したうえで実行する
        if (File.Exists(gameDataPath))
        {
            DirectoryActs.CompleteDirDelete(gameDataPath);
        }

        //結合して出来たZIPファイルを解凍する
        ZipFile.ExtractToDirectory(margedFilePath, gameDataPath, Encoding.GetEncoding("shift_jis"));

        //ZIPファイルを削除する
        System.IO.File.Delete(margedFilePath);
    }
}
