
using Cysharp.Threading.Tasks;
using System.IO;
using System.Threading;

public class ImageDlProc
{
    private OnNetDriveGetFile _getFile;
    private GameData _targetGameData;
    private AllDirs _allDirs;

    CancellationToken cancellDownloadImage;

    public ImageDlProc(OnNetDriveGetFile getFile, GameData gameData)
    {
        _getFile = getFile;
        _targetGameData = gameData;
        _allDirs = AllDirs.GetInstance();
    }

    public async UniTask DLImageInUniTask(CancellationToken ct)
    {
        await UniTask.RunOnThreadPool(DLImage, cancellationToken: ct);
    }

    /// <summary>
    /// コンストラクタで渡されたGameDataインスタンスを利用してサムネイル画像のDLを実行する
    /// 既にダウンロードされていた場合でも再度ダウンロードを行ってしまうため、既にダウンロードされているかは呼び出し元で確認する
    /// </summary>
    public void DLImage()
    {
        if(_targetGameData.GameImageId == null || _targetGameData.GameImageId == "")
        {
            throw new System.Exception("対象のGameDataインスタンスには画像用のDriveIdが保存されていません");
        }

        string gameId = _targetGameData.GameID;
        //画像パスの作成
        string imageExtension = "." + new CmdGenericInfoInputOfAddGame()._imageExtension;
        string imageFileName = gameId + imageExtension;
        string imagePath = Path.Combine(_allDirs.ImageFolderPath, imageFileName);

        CheckAndDeleteImageFile(imagePath);
        _getFile.GetFile(_targetGameData.GameImageId, imageFileName, _allDirs.ImageFolderPath);

        if (cancellDownloadImage.IsCancellationRequested)
        {
            CheckAndDeleteImageFile(imagePath);
        }
    }

    /// <summary>
    /// 画像保存用ディレクトリに今回対象にするIDの画像ファイルが保存されているかを確認する
    /// もし保存されているなら削除する
    /// </summary>
    private void CheckAndDeleteImageFile(string targetPath)
    {
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
    }
}
