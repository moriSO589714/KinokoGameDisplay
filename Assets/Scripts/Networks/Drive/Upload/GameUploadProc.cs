using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;

public class GameUploadProc
{
    private readonly int goalDividePiecies = 50;
    //アップロード時の最大データサイズ(圧縮時)。単位はbyte
    private readonly long limitUploadSize = 3221225472;

    private AllDirs _allDirs = null;
    private OnNetCreateFolder _onNetCreateFolder = null;
    private OnNetDriveUploadFile _onNetDriveUploadFile = null;
    private OnNetAppEndGameInfo _onNetAppEndGameInfo = null;
    private OnNetGetParentId _onNetGetParentId = null;
    private OnNetDriveGetName _onNetDriveGetName = null;
    private OnNetDelete _onNetDelete = null;

    private CancellationToken _ct = new CancellationToken();

    private string _gameOriginalId = "";
    private string _driveId = "";

    public GameUploadProc
        (OnNetCreateFolder onNetCreateFolder, OnNetDriveUploadFile onNetDriveUploadFile, OnNetAppEndGameInfo onNetAppEndGameInfo, OnNetGetParentId onNetGetParentId, OnNetDriveGetName onNetDriveGetName, OnNetDelete onNetDelete)
    {
        _allDirs = AllDirs.GetInstance();

        _onNetCreateFolder = onNetCreateFolder;
        _onNetDriveUploadFile = onNetDriveUploadFile;
        _onNetAppEndGameInfo = onNetAppEndGameInfo;
        _onNetGetParentId = onNetGetParentId;
        _onNetDriveGetName = onNetDriveGetName;
        _onNetDelete = onNetDelete;
    }

    public async UniTask UploadGameInUniTask(CancellationToken ct, GameData gameData, bool forceUpload = false, GameUpProgress gameUpProgress = null)
    {
        _ct = ct;

        try
        {
            await UniTask.RunOnThreadPool(() => UploadAll(forceUpload, gameData, gameUpProgress), cancellationToken: ct);
        }
        catch (System.Exception e)
        {
            //エラーで終了した場合はアップロード途中の部分の削除を行う
            if(_driveId != "")
            {

                //アップロード済みのデータやスプシデータの削除
                try
                {
                    //tmpフォルダの削除
                    string tempDirPath = _allDirs.TmpUpPath;
                    string tempGamePath = CreateDirPath.TempGamePathForUpload(tempDirPath, _gameOriginalId);
                    
                    DirectoryActs.CompleteDirDelete(tempGamePath);
                    DeleteProc deleteProc = new DeleteProc(_onNetDelete, _onNetGetParentId, _onNetDriveGetName);
                    await deleteProc.UniDeleteDriveGame(_driveId, _gameOriginalId, new CancellationTokenSource().Token);
                }
                catch(System.Exception e2)
                {
                    Debug.LogException(e2);
                }
            }

            throw e;
        }
    }

    private void UploadAll(bool forceUpload, GameData gameData, GameUpProgress gameUpProgress)
    {
        string localGameDir = gameData.GameDriveId;
        string localImageDir = gameData.GameImageId;

        string createGameId = CreateGameId(gameUpProgress);
        string idDriveFolderId = CreateIdDriveFolder(createGameId);
        string driveId = UploadGame(forceUpload, gameData, createGameId, idDriveFolderId, gameUpProgress);
        string imageDriveId = UploadImage(idDriveFolderId, createGameId, gameData, gameUpProgress);
        WriteSheetForUpload(localGameDir, createGameId, driveId, imageDriveId, gameData, gameUpProgress);

        gameUpProgress?.ChangeState("アップロード処理が完了しました");
    }

    /// <summary>
    /// GameIdを作成する
    /// </summary>
    /// <param name="progress"></param>
    /// <returns></returns>
    private string CreateGameId(GameUpProgress progress)
    {
        //ゲームを判別するための固有IDの生成
        string gameId = UUIDGenerator.GenerateUUID();
        _gameOriginalId = gameId;
        Debug.Log("UploadGameId>>>" + gameId);
        progress?.ChangeState($"ゲームIDの作成完了。ID>>>{gameId}");

        return gameId;
    }

    /// <summary>
    /// GameIDが名前になっているドライブフォルダを作成して、そのDriveIdを返す
    /// </summary>
    private string CreateIdDriveFolder(string gameId)
    {
        string idDriveFolder = _onNetCreateFolder.CreateFolder(_allDirs.GameSavedDriveID, gameId);
        return idDriveFolder;
    }

    /// <summary>
    /// ゲームをアップロードする
    /// GameDataクラスのGameDriveIdとGameImageIdにはそれぞれのローカルパスを入れる
    /// </summary>
    /// <returns>アップロードしたゲームフォルダのDriveID</returns>
    /// <exception cref="System.Exception"></exception>
    public string UploadGame(bool forceUpload, GameData gameData,string gameId ,string idDriveFolderId, GameUpProgress gameUpProgress)
    {
        string localGameDir = gameData.GameDriveId;
        string localImagePath = gameData.GameImageId;

        //使用するパスの定義
        string tempDirPath = _allDirs.TmpUpPath;
        string tempGamePath = CreateDirPath.TempGamePathForUpload(tempDirPath, gameId);
        string tempSlicedGamePath = CreateDirPath.SlicedFilesPathForUpload(tempGamePath);

        gameUpProgress?.ChangeState("ゲームデータの圧縮と分割を実行中");
        //一時保存用ディレクトリを作成する
        DirectoryActs.CreateAndCheckDir(tempSlicedGamePath);
        FileSpliting fileSpliting = new FileSpliting();
        //ゲームが入っているフォルダを圧縮する
        string zipFilePath = fileSpliting.PackagingFile(localGameDir, tempGamePath);
        string gameFolderName = Path.GetFileName(localGameDir);

        //ファイルの容量から目標分割数を参考に分割するバイト数を算出する
        long zipFileByte = new System.IO.FileInfo(zipFilePath).Length;
        long splicedBite = (long)Mathf.Floor(zipFileByte / goalDividePiecies);

        //制限容量を超えている場合は処理を終了する
        if(zipFileByte >= limitUploadSize)
        {
            if (!forceUpload)//強制アップロードのフラグがtrueの場合はアップロードを行う
            {
                throw new System.Exception("アップロードできる最大サイズを超えています");
            }
        }

        //ゲームデータの分割を行う
        fileSpliting.DivideZipFile(splicedBite, zipFilePath, tempSlicedGamePath);

        //分割したゲームデータのパスをリストで取得
        string[] uploadFilesPaths = Directory.GetFiles(tempSlicedGamePath);

        gameUpProgress?.ChangeState("インターネット上にアップロード用フォルダを作成");
        //GoogleDrive上のフォルダを作成する     
        string uploadTargetFolderDriveId = _onNetCreateFolder.CreateFolder(idDriveFolderId, gameFolderName);

        _driveId = uploadTargetFolderDriveId;

        int counter = 0;
        //順番にアップロードを行う
        foreach (string uploadFilePath in uploadFilesPaths)
        {
            try
            {
                _onNetDriveUploadFile.UploadFile(uploadTargetFolderDriveId, uploadFilePath);
            }
            catch (System.Exception ex) 
            {
                UnityEngine.Debug.LogException(ex);
                throw ex;
            }

            //トークンがキャンセルされていれば例外を投げて処理を中断
            _ct.ThrowIfCancellationRequested();

            gameUpProgress?.ChangeState($"{counter++}/{uploadFilesPaths.Count()}をアップロード済み");
        }

        //一時データの削除
        gameUpProgress?.ChangeState("ローカルの一時ファイル削除を実行中");
        DirectoryActs.CompleteDirDelete(tempGamePath);

        return uploadTargetFolderDriveId;
    }

    /// <summary>
    /// サムネイル画像のアップロード
    /// </summary>
    public string UploadImage(string idFolderDriveId, string gameId, GameData gameData, GameUpProgress progress)
    {
        //サムネ画像のアップロード
        progress?.ChangeState("サムネイル画像のアップロードを開始");
        string localImagePath = gameData.GameImageId;

        string imageDriveId = "";
        if (localImagePath != null && localImagePath != "")
        {
            NetworkThumbnailManager networkThumbnailManager = new NetworkThumbnailManager();

            //トークンがキャンセルされていれば例外を投げて処理を中断
            _ct.ThrowIfCancellationRequested();

            imageDriveId = networkThumbnailManager.UploadThumbnail(_onNetDriveUploadFile, idFolderDriveId, localImagePath, gameId);
        }

        return imageDriveId;
    }

    /// <summary>
    /// データのスプレッドシートへの書き込み
    /// </summary>
    private void WriteSheetForUpload(string localGameDir, string gameId, string driveId, string imageDriveId, GameData gameData, GameUpProgress progress)
    {
        progress?.ChangeState("スプレッドシートへゲーム情報を追加中");
        //スプレッドシートへの保存
        gameData.GameDirName = Path.GetFileName(localGameDir);
        gameData.GameID = gameId;
        gameData.GameVersion = NetworkGameVersionManager.CreateGameVersion();
        gameData.GameDriveId = driveId;
        gameData.GameImageId = imageDriveId;
        NetworksSingleton networksSingleton = NetworksSingleton.Instance;
        List<string> sheetElementOrder = networksSingleton.ReturnElementOrder(false);
        //リスト形式に変換
        List<string> registerSheetFormat = ElementOrderManager.GameDataToSheetFormat(sheetElementOrder, gameData);

        //トークンがキャンセルされていれば例外を投げて処理を中断
        _ct.ThrowIfCancellationRequested();

        //スプレッドシートの新規行に追加
        _onNetAppEndGameInfo.AppEndGameInfo(registerSheetFormat);
    }
}