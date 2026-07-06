public class GameDeleteManager
{
    public void UninstallGame(GameData gameData)
    {
        AllDirs allDirs = AllDirs.GetInstance();

        string gameDirPath = CreateDirPath.GameDataPathId
            (saveGamesDirName: allDirs.GameFilePath, gameId: gameData.GameID);
        DirectoryActs.CompleteDirDelete(gameDirPath);

        string gameJsonPath = CreateDirPath.GameJsonPath(savedJsonsPath: allDirs.JsonsDirPath, gameId: gameData.GameID);
        DirectoryActs.CompleteDirDelete(gameJsonPath);

        gameData.Status = GameStatus.NotDownloaded;
        UnityEngine.Debug.Log("changeStatus 02");
    }
}
