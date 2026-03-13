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
    //不足したファイルがあった際に呼ばれるデリゲート
    public Action<List<long>> IsLackFile;
    //問題のあるファイルがあった際に呼ばれるデリゲート
    public Action<List<string>> IsErrorFile;

    /// <summary>
    /// 分割されたZIPファイルを結合して解凍するメソッド
    /// </summary>
    /// <param name="splitedFilesPath">結合されるファイル群が置かれているディレクトリのパス</param>
    public void MergeSplitedFile(string splitedFilesPath, string margedFileInDirPath)
    {
        //対象ディレクトリ内のファイルのパスを取得してくる
        string[] splitedFiles = Directory.GetFiles(splitedFilesPath);

        MistakeFiles mistakeFiles = new FreezingGeneric().hasAllRequiredData(splitedFilesPath);
        if(mistakeFiles.LackFiles.Count() != 0)
        {
            //ファイル欠損時に呼ぶメソッドを呼んで処理を終了
            IsLackFile?.Invoke(mistakeFiles.LackFiles);
            Debug.Log("ファイルの欠損を確認しました");
            return;
        }
        else if (mistakeFiles.ErrorFilePathes.Count() != 0)
        {
            //ファイル欠損以外に問題があるファイルがある場合(ファイルの重複、名前の異なるファイル)
            IsErrorFile?.Invoke(mistakeFiles.ErrorFilePathes);
            Debug.Log("ファイル群のあるフォルダに問題のあるファイルがあります。");
            return;
        }

        //データ群を拡張子でソート
        string[] sortedFiles = new FreezingGeneric().sortingFilesByPath(splitedFilesPath);
        //一番初めに来るファイル(.00)のファイルをバイト配列として取得する
        byte[] dlDatabytes = File.ReadAllBytes(sortedFiles[0]);
        //DLDataクラスに上記バイト配列からデータを格納
        DLData targetDLData = new DLData(dlDatabytes);

        //targetDLDataから結合するゲームの詳細情報を取得する
        string dlFileName = targetDLData.FileName;
        long splitedFileNum = targetDLData.SplitedFileNum;

        //結合後にファイルを置くパスを生成
        string margedFilePath = Path.Combine(margedFileInDirPath, dlFileName + ".zip");
        //データを結合する処理
        using (FileStream outFs = new FileStream(margedFilePath, FileMode.Create,FileAccess.Write))
        {
            for (int i = 1; i < sortedFiles.Length; i++)
            {
                byte[] bytedatas = File.ReadAllBytes(sortedFiles[i]);
                outFs.Write(bytedatas, 0, bytedatas.Length);
            }
        }

        //結合して出来たZIPファイルを解凍する
        ZipFile.ExtractToDirectory(margedFilePath, Path.Combine(margedFileInDirPath, dlFileName), Encoding.GetEncoding("shift_jis"));

        //ZIPファイルを削除する
        System.IO.File.Delete(margedFilePath);
    }
}
