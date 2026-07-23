using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DeleteProc
{
    OnNetDelete _onNetDelete;
    OnNetGetParentId _onNetGetParentId;
    OnNetDriveGetName _onNetDriveGetName;

    public DeleteProc(OnNetDelete onNetDelete, OnNetGetParentId onNetGetParentId, OnNetDriveGetName onNetDriveGetName)
    {
        _onNetDelete = onNetDelete;
        _onNetGetParentId = onNetGetParentId;
        _onNetDriveGetName = onNetDriveGetName;
    }

    public async UniTask UniDeleteDriveGame(string gameDriveId, string gameOriginalId, CancellationToken ct)
    {
        await UniTask.RunOnThreadPool(() => DeleteDriveGameData(gameDriveId, gameOriginalId), cancellationToken: ct);
    }

    /// <summary>
    /// ドライブにあるゲームデータを削除する
    /// </summary>
    /// <param name="gameDriveId">スライスされたゲームデータが入っているフォルダのDriveID</param>
    private void DeleteDriveGameData(string gameDriveId, string gameOriginalId)
    {
        //一つ上のフォルダに移動する
        string parentId = _onNetGetParentId.GetParentId(gameDriveId);

        //一つ上のフォルダ名の取得
        string parentFolderName = _onNetDriveGetName.GetFolderName(parentId);

        //フォルダ名がゲームIDと一致している場合削除を実行する
        if(parentFolderName == gameOriginalId)
        {
            _onNetDelete.DeleteFolder(parentId);
        }
        else
        {
            throw new System.Exception("削除するゲームの親Driveフォルダの名前がゲームの固有IDと一致しません。親フォルダ名＞＞" + parentFolderName);
        }

        //スプレッドシート上の対象ゲームに関する情報を削除する
        DeleteGameAllInfoFromSpSt deleteGameAllInfoFromSpSt = new DeleteGameAllInfoFromSpSt();
        deleteGameAllInfoFromSpSt.DeleteGameInfo(gameOriginalId);

        Debug.Log("END_DELETE");
    }
}
