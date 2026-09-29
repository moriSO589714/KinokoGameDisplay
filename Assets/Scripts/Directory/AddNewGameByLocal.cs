using System.IO;

/// <summary>
/// ローカル上で新しいゲームを追加するクラス
/// </summary>
public class AddNewGameByLocal
{
    public void AddNewGame(GameData gameInfo, string currentGamePath, string thumbnailImagePath = "")
    {
        //最低限必要なデータがGameDataインスタンスに格納されているか
        if (!GameDataForUpload.QualityCheck(gameInfo, currentGamePath))
        {
            throw new System.Exception("ゲーム情報に不足があります");
        }

        AllDirs allDirs = AllDirs.GetInstance();
        //固有IDの生成
        string gameId = UUIDGenerator.GenerateUUID();
        gameInfo.GameID = gameId;

        gameInfo.Status = GameStatus.ByLocal;

        //ローカルパスのゲームをゲーム保存に利用しているディレクトリにコピーする
        string gameDirName = Path.GetFileName(currentGamePath);
        string newPath = CreateDirPath.GameDataPath(allDirs.GameFilePath, gameId, gameDirName);
        DirectoryActs.CopyDirectory(currentGamePath, newPath);

        if(thumbnailImagePath != "")
        {
            string newImagePath = Path.Combine(allDirs.ImageFolderPath, gameId + Path.GetExtension(thumbnailImagePath));
            File.Copy(thumbnailImagePath, newImagePath);
        }

        //ゲーム情報をjsonデータとして保存
        string gameJsonPath = CreateDirPath.GameJsonPath(savedJsonsPath: allDirs.JsonsDirPath, gameId: gameId);
        JSONTools.SerializeJson(gameInfo, gameJsonPath);
    }
}