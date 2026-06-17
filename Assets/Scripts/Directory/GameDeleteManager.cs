public class GameDeleteManager
{
    public void UninstallGame(GameData gameData)
    {
        AllDirs allDirs = AllDirs.GetInstance();

        string gameDirPath = CreateDirPath.GameDataPath
            (saveGamesDirName: allDirs.GameFilePath, gameId: gameData.GameID, gameDirName: gameData.GameDirName);
        DirectoryActs.CompleteDirDelete(gameDirPath);

        string gameJsonPath = CreateDirPath.GameJsonPath(savedJsonsPath: allDirs.JsonsDirPath, gameId: gameData.GameID);
        DirectoryActs.CompleteDirDelete(gameJsonPath);

        gameData.Status = GameStatus.NotDownloaded;
    }
}
