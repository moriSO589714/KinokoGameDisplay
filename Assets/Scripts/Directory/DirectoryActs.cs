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

    /// <summary>
    /// 指定のパス以下のファイルを丸ごとコピーする
    /// </summary>
    /// <param name="copyedFolderPath">コピーされる対象のフォルダパス</param>
    /// <param name="targetPath">コピー先のパス</param>
    public static void CopyDirectory(string copyedFolderPath, string targetPath)
    {
        //コピー先のディレクトリがない場合は作成
        if (!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
            //ファイル属性もコピーする
            File.SetAttributes(targetPath, File.GetAttributes(copyedFolderPath));
        }

        //コピー先のディレクトリ名の末尾に\を付ける
        if (targetPath[targetPath.Length - 1] != Path.DirectorySeparatorChar)
        {
            targetPath = targetPath + Path.DirectorySeparatorChar;
        }

        //copyedFolderPath以下のファイルをコピー
        string[] files = Directory.GetFiles(copyedFolderPath);
        foreach(string file in files)
        {
            File.Copy(file, targetPath + Path.GetFileName(file), true);
        }

        //ディレクトリがある場合はこのメソッドを再帰的に呼び出してコピーする
        string[] dirs = Directory.GetDirectories(copyedFolderPath);
        foreach(string dir in dirs)
        {
            CopyDirectory(dir, targetPath + Path.GetFileName(dir));
        }
    }
}
