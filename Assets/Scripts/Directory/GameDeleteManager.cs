using System.IO;

public class GameDeleteManager
{
    public void UninstallGame(GameData gameData)
    {
        AllDirs allDirs = AllDirs.GetInstance();

        //ゲームデータ本体の削除処理
        string gameDirPath = CreateDirPath.GameDataPathId
            (saveGamesDirName: allDirs.GameFilePath, gameId: gameData.GameID);
        DirectoryActs.CompleteDirDelete(gameDirPath);

        //画像の削除処理
        string gameImagePath = Path.Combine(allDirs.ImageFolderPath, gameData.GameID + ".png");
        if (File.Exists(gameImagePath))
        {
            File.SetAttributes(gameImagePath, FileAttributes.Normal);
            File.Delete(gameImagePath);
        }

        //jsonファイルの削除
        string gameJsonPath = CreateDirPath.GameJsonPath(savedJsonsPath: allDirs.JsonsDirPath, gameId: gameData.GameID);
        if (File.Exists(gameJsonPath))
        {
            File.SetAttributes(gameJsonPath, FileAttributes.Normal);
            File.Delete(gameJsonPath);
        }

        //ステータスの変更
        if(gameData.Status == GameStatus.ByLocal)
        {
            GameDatasSingleton gameDatasSingleton = GameDatasSingleton.Instance;
            gameDatasSingleton.RemoveGameData(gameData);
        }
        else
        {
            gameData.Status = GameStatus.NotDownloaded;
        }
    }
}
