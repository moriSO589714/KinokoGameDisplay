using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// ディレクトリー関係のアクションをまとめるクラス
/// 別スレッドで呼び出すのが望ましい
/// </summary>
public static class DirectoryActs
{
    /// <summary>
    /// 特定のパスのディレクトリが存在しているかを確認して、していない場合作成する
    /// </summary>
    public static bool CreateAndCheckDir(string path)
    {
        if (Directory.Exists(path))
        {
            return true;
        }

        Directory.CreateDirectory(path);
        return true;
    }

    /// <summary>
    /// 指定されたディレクトリを中身のファイルごと全て削除する
    /// </summary>
    public static void CompleteDirDelete(string dirPath)
    {
        if (!Directory.Exists(dirPath)) return;

        //ディレクトリ以外の全てのファイルを削除する
        string[] filePaths = Directory.GetFiles(dirPath);
        foreach(string file in filePaths)
        {
            //ファイルの属性をnormalに変更してから削除する
            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }

        //対象とするディレクトリ内にディレクトリがある場合このメソッドを再帰的に呼んで削除
        string[] inDirPaths = Directory.GetDirectories(dirPath);
        foreach(string inDirPath in inDirPaths)
        {
            CompleteDirDelete(inDirPath);
        }

        //中身に何も無い状態になった場合、自身も削除する
        //パスには直接子要素を削除する際のパスを指定できるが、今回のメソッドでは行わないためfalse
        Directory.Delete(dirPath, false);
    }

    /// <summary>
    /// 指定のディレクトリを削除後、新規に作り直す
    /// </summary>
    public static void RefleshDir(string path)
    {
        CompleteDirDelete(path);
        CreateAndCheckDir(path);
    }
}
