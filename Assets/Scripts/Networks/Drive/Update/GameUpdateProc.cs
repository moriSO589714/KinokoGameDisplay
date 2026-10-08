using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

public class GameUpdateProc
{
    CancellationToken _ctForUpdate;
    AllDirs _allDirs;

    private readonly string _gameIdVariableName = "GameID";

    OnNetDelete _onNetDelete;
    OnNetGetParentId _onNetGetParentId;
    OnNetCreateFolder _onNetCreateFolder;
    OnNetDriveUploadFile _onNetDriveUploadFile;
    OnNetAppEndGameInfo _onNetAppEndGameInfo;
    OnNetUpdateGameInfo _onNetUpdateGameInfo;
    OnNetDriveGetName _onNetDriveGetName;

    public GameUpdateProc
        (OnNetDelete onNetDelete, OnNetGetParentId onNetGetParentId, OnNetCreateFolder onNetCreateFolder
        , OnNetDriveUploadFile onNetDriveUploadFile, OnNetAppEndGameInfo onNetAppEndGameInfo, OnNetUpdateGameInfo onNetUpdateGameInfo, OnNetDriveGetName onNetDriveGetName)
    {
        _allDirs = AllDirs.GetInstance();

        _onNetDelete = onNetDelete;
        _onNetGetParentId = onNetGetParentId;
        _onNetCreateFolder = onNetCreateFolder;
        _onNetDriveUploadFile = onNetDriveUploadFile;
        _onNetAppEndGameInfo = onNetAppEndGameInfo;
        _onNetUpdateGameInfo = onNetUpdateGameInfo;
        _onNetDriveGetName = onNetDriveGetName;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ct"></param>
    /// <param name="originGameData">GameDriveIdにアップロードするローカルフォルダのパス、GameImageIdにはサムネイル画像のローカルパス</param>
    /// <param name="newGameData"></param>
    /// <param name="forceUpdate"></param>
    /// <param name="gameUpProgress"></param>
    /// <returns></returns>
    public async UniTask UpdateGameInUniTask(CancellationToken ct, GameData newGameData, GameData originGameData, bool forceUpdate = false, GameUpProgress gameUpProgress = null)
    {
        _ctForUpdate = ct;

        //ゲーム本体に更新が行われている場合。localPathがGameDriveIdに仮で入っている(仕様はGameDataForUploadを参照のこと)
        if(newGameData.GameDriveId != "")
        {
            await UniTask.RunOnThreadPool(() => UploadGame(forceUpdate, originGameData.GameDriveId, newGameData, originGameData, gameUpProgress));
        }
        else
        {
            //GameDriveIdとGameImageId以外のフィールドはコピーにより、元のデータが入っているが、
            //この2つはGameDataForUploadによりローカルディレクトリパスに置き換えられている為、以下の処理が必要
            newGameData.GameDriveId = originGameData.GameDriveId;

            //ゲーム本体には更新が行われないが、サムネイル画像には更新が行われている場合
            if(newGameData.GameImageId != "")
            {
                //既に画像がアップロードされている場合削除する
                if (originGameData.GameImageId != "")
                {
                    await UniTask.RunOnThreadPool( () => _onNetDelete.DeleteFolder(originGameData.GameImageId));
                }

                GameUploadProc gameUploadProc = new GameUploadProc(_onNetCreateFolder, _onNetDriveUploadFile, _onNetAppEndGameInfo, _onNetGetParentId, _onNetDriveGetName, _onNetDelete);
                string parentFolderId = await UniTask.RunOnThreadPool( () => _onNetGetParentId.GetParentId(originGameData.GameDriveId));
                string imageDriveId = await UniTask.RunOnThreadPool( () => { return gameUploadProc.UploadImage(parentFolderId, newGameData.GameID, newGameData, gameUpProgress); });
                newGameData.GameImageId = imageDriveId;
            }
            else
            {
                newGameData.GameImageId = originGameData.GameImageId;
            }

            //スプシへの書き込み
            await UniTask.RunOnThreadPool(() => WriteSheetForUpdate(newGameData.GameID, newGameData));
        }
    }

    private void UploadGame(bool forceUpload, string driveId, GameData uploadGameData, GameData originGameData, GameUpProgress progress)
    {
        //idがフォルダ名になっているドライブフォルダのDriveIDを取得する
        string parentFolderId = _onNetGetParentId.GetParentId(driveId);

        //ドライブに保存されているアップデート前のデータを削除する
        progress?.ChangeState("アップデート前データの削除を実行中");
        _onNetDelete.DeleteFolder(driveId);

        //ゲームデータのアップロード
        GameUploadProc gameUploadProc = new GameUploadProc(_onNetCreateFolder, _onNetDriveUploadFile, _onNetAppEndGameInfo, _onNetGetParentId, _onNetDriveGetName, _onNetDelete);
        string gameDriveId = gameUploadProc.UploadGame(forceUpload, uploadGameData, uploadGameData.GameID, parentFolderId, progress);
        uploadGameData.GameDirName = Path.GetFileName(uploadGameData.GameDriveId);
        uploadGameData.GameDriveId = gameDriveId;
        uploadGameData.GameVersion = NetworkGameVersionManager.CreateGameVersion();

        //サムネイル画像が新しく指定されていればアップロードする
        if(uploadGameData.GameImageId != null && uploadGameData.GameImageId != "")
        {
            progress?.ChangeState("画像データのアップロード中");
            //もし既に画像がアップロードされているなら削除する
            if(originGameData.GameImageId != "")
            {
                _onNetDelete.DeleteFolder(originGameData.GameImageId);
            }

            string imageDriveId = gameUploadProc.UploadImage(parentFolderId, uploadGameData.GameID, uploadGameData, progress);
            uploadGameData.GameImageId = imageDriveId;
        }

        //スプレッドシートへの書き込み
        WriteSheetForUpdate(uploadGameData.GameID, uploadGameData);
    }

    private void WriteSheetForUpdate(string targetGameId, GameData registerGameData)
    {
        NetworksSingleton networksSingleton = NetworksSingleton.Instance;

        List<string> elementOrder = networksSingleton.ReturnElementOrder(true);
        //スプレッドシートのデータを取得し直す
        List<List<string>> sheetDatas = networksSingleton.ReturnGameInfoAllData(true);

        //対象のGameIdのスプレッドシート上の行数を取得
        int sheetRow = SpStTools.SearchSheetForRowFromGameId(AllDirs.GetInstance(), elementOrder, _gameIdVariableName, sheetDatas, targetGameId);

        List<string> registerSheetFormat = ElementOrderManager.GameDataToSheetFormat(networksSingleton.ReturnElementOrder(false), registerGameData);
        _onNetUpdateGameInfo.UpdateGameInfo(registerSheetFormat, sheetRow);
    }
}