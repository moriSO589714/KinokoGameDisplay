using System.Collections.Generic;
using System.IO;

public static class CreateDirPath
{
    private const string _jsonExtention = ".json";
    private const string _zipExtention = ".zip";
    private const string _slicedDirName = "sliced";
    private const string _inZipDirName = "inzip";

    /// <param name="saveGamesDirName">ゲームの保存に利用しているディレクトリパス</param>
    /// <param name="gameId">今回対象にするゲームのID</param>
    /// <param name="gameDirName">今回対象にするゲームのディレクトリ名(識別用)</param>
    /// <returns>特定のゲームの保存場所</returns>
    public static string GameDataPath(string saveGamesDirName, string gameId, string gameDirName)
    {
        List<string> paths = new List<string>(3) { saveGamesDirName, gameId, gameDirName};
        string gameDataPath = MergePath(paths);
        return gameDataPath;
    }

    /// <param name="savedJsonsPath">jsonファイルの保存に利用しているディレクトリパス</param>
    /// <param name="gameId">今回対象にするゲームのID</param>
    /// <returns>特定のゲームのjsonファイルのパス</returns>
    public static string GameJsonPath(string savedJsonsPath, string gameId)
    {
        List<string> paths = new List<string>(2) { savedJsonsPath, gameId + _jsonExtention };
        string gameJsonPath = MergePath(paths);
        return gameJsonPath;
    }

    /// <param name="tempDirPath">一時保存に利用しているディレクトリのパス</param>
    /// <param name="gameId">今回対象にするゲームのID</param>
    /// <returns>特定のゲームをダウンロードする際に一時保存するディレクトリのパス</returns>
    public static string TempGamePathForDl(string tempDirPath, string gameId)
    {
        List<string> paths = new List<string>(2) { tempDirPath, gameId};
        string tempGamePath = MergePath(paths);
        return tempGamePath;
    }

    /// <param name="tempDirPath">一時保存に利用しているディレクトリのパス</param>
    /// <param name="gameId">今回対象にするゲームのID</param>
    /// <returns>特定のゲームをダウンロードする際にスライスされたデータを保存するディレクトリのパス</returns>
    public static string TempSlicedGamePathForDl(string tempDirPath, string gameId)
    {
        string tempGamePath = TempGamePathForDl(tempDirPath, gameId);
        List<string> paths = new List<string>(2) { tempGamePath, _slicedDirName };
        string tempSlicedGamePath = MergePath(paths);
        return tempSlicedGamePath;
    }

    /// <param name="tempGamePathForDl">特定のゲームの一時保存に利用しているディレクトリのパス</param>
    /// <returns>特定のゲームをダウンロードする際にスライスされたデータを保存するディレクトリのパス</returns>
    public static string TempSlicedGamePathForDl(string tempGamePathForDl)
    {
        List<string> paths = new List<string>(2) { tempGamePathForDl, _slicedDirName };
        string tempSlicedGamePath = MergePath(paths);
        return tempSlicedGamePath;
    }

    /// <param name="tempGamePathForDl">ダウンロードするゲームの一時保存に利用しているディレクトリ</param>
    /// <returns>ダウンロード時にzip化したファイルを置くディレクトリのパス</returns>
    public static string InZipDirForDl(string tempGamePathForDl)
    {
        List<string> paths = new List<string>(2) { tempGamePathForDl, _inZipDirName };
        string inZipDir = MergePath(paths);
        return inZipDir;
    }

    public static string ZipFilePathForDl(string inZipDir, string gameDirName)
    {
        List<string> paths = new List<string>(2) { inZipDir, gameDirName + _zipExtention };
        string zipFilePath = MergePath(paths);
        return zipFilePath;
    }

    private static string MergePath(List<string> paths)
    {
        string stuckPath = "";
        foreach(string str in paths)
        {
            stuckPath = Path.Combine(stuckPath, str);
        }
        return stuckPath;
    }
}
