using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class FileCombine : MonoBehaviour
{
    /// <summary>
    /// 分割されたZIPファイルを結合するメソッド
    /// </summary>
    /// <param name="mergedFilesPath">結合されるファイル群が置かれているディレクトリのパス</param>
    public void MergeSplitedFile(string mergedFilesPath)
    {
        //対象ディレクトリ内のファイルのパスを取得してくる
        string[] splitedFiles = Directory.GetFiles(mergedFilesPath);
        //取得したファイルパス群から、語尾の数値でソートを行う(数値以外の部分は共通しているため、単純にソートを行うことが可能)
        //ToArray()はキャッシュ化してLINQの遅延実行を無視するため
        string[] sortedFiles = splitedFiles.OrderBy(f => f).ToArray();


    }
}
