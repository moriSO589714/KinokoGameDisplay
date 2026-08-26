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
            await UniTask.RunOnThreadPool(() => UploadGame(forceUpload, gameData, gameUpProgress), cancellationToken: ct);
        }
        catch (System.Exception e)
        {
            //エラーで終了した場合はアップロード途中の部分の削除を行う
            if(_driveId != "")
            {
                //tmpフォルダの削除
                string tempDirPath = _allDirs.TmpUpPath;
                string tempGamePath = CreateDirPath.TempGamePathForUpload(tempDirPath, _gameOriginalId);
                DirectoryActs.CompleteDirDelete(tempGamePath);

                //アップロード済みのデータやスプシデータの削除
                DeleteProc deleteProc = new DeleteProc(_onNetDelete, _onNetGetParentId, _onNetDriveGetName);
                await deleteProc.UniDeleteDriveGame(_driveId, _gameOriginalId, new CancellationTokenSource().Token);
            }

            throw e;
        }
    }

    /// <summary>
    /// ゲームをアップロードする
    /// GameDataクラスのGameDriveIdとGameImageIdにはそれぞれのローカルパスを入れる
    /// </summary>
    /// <exception cref="System.Exception"></exception>
    private void UploadGame(bool forceUpload, GameData gameData, GameUpProgress gameUpProgress)
    {
        //ゲームを判別するための固有IDの生成
        string gameId = UUIDGenerator.GenerateUUID();
        _gameOriginalId = gameId;
        Debug.Log("UploadGameId>>>" + gameId);
        gameUpProgress?.ChangeState($"ゲームIDの作成完了。ID>>>{gameId}");
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
        string gameSavedDriveId = _allDirs.GameSavedDriveID; //ゲーム保存ドライブフォルダの最も上層フォルダ
        string gameIdFolderDriveId = _onNetCreateFolder.CreateFolder(gameSavedDriveId, gameId);
        string uploadTargetFolderDriveId = _onNetCreateFolder.CreateFolder(gameIdFolderDriveId, gameFolderName);

        _driveId = uploadTargetFolderDriveId;

        int counter = 0;
        //順番にアップロードを行う
        foreach (string uploadFilePath in uploadFilesPaths)
        {
            _onNetDriveUploadFile.UploadFile(uploadTargetFolderDriveId, uploadFilePath);

            //トークンがキャンセルされていれば例外を投げて処理を中断
            _ct.ThrowIfCancellationRequested();

            gameUpProgress?.ChangeState($"{counter++}/{uploadFilesPaths.Count()}をアップロード済み");
        }

        //サムネ画像のアップロード
        gameUpProgress?.ChangeState("サムネイル画像のアップロードを開始");
        string imageDriveId = "";
        if(localImagePath != null && localImagePath != "")
        {
            NetworkThumbnailManager networkThumbnailManager = new NetworkThumbnailManager();

            //トークンがキャンセルされていれば例外を投げて処理を中断
            _ct.ThrowIfCancellationRequested();

            imageDriveId = networkThumbnailManager.UploadThumbnail(_onNetDriveUploadFile, gameIdFolderDriveId, localImagePath, tempGamePath, gameId);
        }

        //一時データの削除
        gameUpProgress?.ChangeState("ローカルの一時ファイル削除を実行中");
        DirectoryActs.CompleteDirDelete(tempGamePath);

        gameUpProgress?.ChangeState("スプレッドシートへゲーム情報を追加中");
        //スプレッドシートへの保存
        gameData.GameDirName = Path.GetFileName(localGameDir);
        gameData.GameID = gameId;
        gameData.GameVersion = NetworkGameVersionManager.CreateGameVersion();
        gameData.GameDriveId = uploadTargetFolderDriveId;
        gameData.GameImageId = imageDriveId;
        NetworksSingleton networksSingleton = NetworksSingleton.Instance;
        List<string> sheetElementOrder = networksSingleton.ReturnElementOrder(false);
        //リスト形式に変換
        List<string> registerSheetFormat = ElementOrderManager.GameDataToSheetFormat(sheetElementOrder, gameData);

        //トークンがキャンセルされていれば例外を投げて処理を中断
        _ct.ThrowIfCancellationRequested();

        //スプレッドシートの新規行に追加
        _onNetAppEndGameInfo.AppEndGameInfo(registerSheetFormat);
        gameUpProgress?.ChangeState("アップロード処理が完了しました");
    }
}