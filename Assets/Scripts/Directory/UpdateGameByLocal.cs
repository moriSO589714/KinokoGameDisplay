
using Cysharp.Threading.Tasks;
using System.IO;
using System.Threading;

public class UpdateGameByLocal
{
    CancellationToken _ctForUpdate;
    AllDirs _allDirs;
    GameUpProgress _gameUpProgress;

    public async UniTask UpdateGameInUniTask(CancellationToken ct, GameData newGameData, GameUpProgress gameUpProgress)
    {
        _ctForUpdate = ct;
        _gameUpProgress = gameUpProgress;
        _allDirs = AllDirs.GetInstance();

        //ゲームデータ本体に更新が行われている場合
        if(newGameData.GameDriveId != "")
        {
            await UniTask.RunOnThreadPool(() => CopyGame(newGameData), cancellationToken: _ctForUpdate);
        }

        //画像に更新が行われている場合
        if(newGameData.GameImageId != "")
        {
            await UniTask.RunOnThreadPool(() => CopyImage(newGameData), cancellationToken: _ctForUpdate);
        }

        //jsonファイルの上書き
        await UniTask.RunOnThreadPool(() => UpdateJson(newGameData), cancellationToken: _ctForUpdate);
    }

    private void CopyGame(GameData newGameData)
    {
        _gameUpProgress?.ChangeState("ゲームのコピーを行っています");
        //各種パス
        string gamesDirPath = _allDirs.GameFilePath;
        string gameIdFolderPath = Path.Combine(gamesDirPath, newGameData.GameID);

        //現在ゲームフォルダに入っているゲームデータを削除する
        if (Directory.Exists(gameIdFolderPath))
        {
            DirectoryActs.CompleteDirDelete(gameIdFolderPath);
        }

        //ローカルパスのゲームをゲーム保存に利用しているディレクトリにコピーする
        string gameDirName = Path.GetFileName(newGameData.GameDriveId);
        string newGamePath = CreateDirPath.GameDataPath(gamesDirPath, newGameData.GameID, gameDirName);
        DirectoryActs.CopyDirectory(newGameData.GameDriveId, newGamePath);

        newGameData.GameDriveId = "";
    }

    private void CopyImage(GameData newGameData)
    {
        _gameUpProgress?.ChangeState("サムネイル画像のコピーを行っています");
        string imageFilePath = Path.Combine(_allDirs.ImageFolderPath, newGameData.GameID + Path.GetExtension(newGameData.GameImageId));
        //現在サムネイル用画像が保存されていれば削除する
        if (File.Exists(imageFilePath))
        {
            File.Delete(imageFilePath);
        }

        File.Copy(newGameData.GameImageId, imageFilePath);
        newGameData.GameImageId = "";
    }

    private void UpdateJson(GameData newGameData)
    {
        _gameUpProgress?.ChangeState("ゲーム情報を保存しています");
        GameJsonUpdate.UpdateJson(newGameData, _allDirs);
    }
}
