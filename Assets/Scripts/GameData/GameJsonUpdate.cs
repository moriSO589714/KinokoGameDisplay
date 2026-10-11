using System.IO;

public static class GameJsonUpdate
{
    public static void UpdateJson(GameData newGameData, AllDirs allDirs)
    {
        string gameJsonPath = CreateDirPath.GameJsonPath(savedJsonsPath: allDirs.JsonsDirPath, gameId:newGameData.GameID);
        if (File.Exists(gameJsonPath))
        {
            File.Delete(gameJsonPath);
        }

        JSONTools.SerializeJson(newGameData, gameJsonPath);
    }
}
