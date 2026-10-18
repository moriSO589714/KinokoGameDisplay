
using System.IO;

public static class FileOpenByExplorer
{
    public static void OpenFile(string filePath)
    {
        string fullPath = Path.GetFullPath(filePath);
        if (File.Exists(fullPath))
        {
            System.Diagnostics.Process.Start("EXPLORER.EXE", @$"/select,""{fullPath}""");
        }
        else
        {
            throw new System.Exception("指定のパスは存在しません。path>>>" + fullPath);
        }
    }

    public static void OpenFolder(string folderPath)
    {
        string fullPath = Path.GetFullPath(folderPath);
        if (Directory.Exists(fullPath))
        {
            System.Diagnostics.Process.Start("EXPLORER.EXE", fullPath);
        }
        else
        {
            throw new System.Exception("指定のパスは存在しません。path>>>" + fullPath);
        }
    }
}
