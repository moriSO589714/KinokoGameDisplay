using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdOfflineDelete : CmdDelete
{
    protected override void Init()
    {
        
    }

    public override void FirstCall()
    {
        base.FirstCall();
        _cmdSceneManager.OutPutManager.ReceiveMessage("オフラインゲーム削除モードに変更します", OutPutTextLogColorSets.SystemDefault);

        GameDatasSingleton gameDatasSingleton = GameDatasSingleton.Instance;
        List<GameData> gameDatas = gameDatasSingleton.AllGameDatas;
        //オンライン上のみに存在するものを除く
        gameDatas.RemoveAll(x => x.Status == GameStatus.NotDownloaded);
        _gameIdLib = CreateLibFromGameDatas.CreateGameIdLib(gameDatas);

        CmdDeleteEntrance();
    }

    protected override bool CheckGameStatus(GameStatus status)
    {
        switch (status)
        {
            case GameStatus.NotDownloaded:
                return false;
            default:
                return true;
        }
    }

    protected override async UniTask DoDelete(GameData targetGameData)
    {
        _cmdSceneManager.OutPutManager.ReceiveMessage("ゲームの削除を開始します", OutPutTextLogColorSets.SystemDefault);
        try
        {
            await UniTask.RunOnThreadPool(() => new GameDeleteManager().UninstallGame(targetGameData));
        }
        catch(System.Exception e)
        {
            _cmdSceneManager.OutPutManager.ReceiveMessage($"エラーが発生しました。エラー内容>>{e}", OutPutTextLogColorSets.AccentDefault);
            CmdDeleteEntrance();
            return;
        }

        _cmdSceneManager.OutPutManager.ReceiveMessage("ゲームの削除が完了しました", OutPutTextLogColorSets.SystemDefault);

        ReturnCmdReceiveMode();
    }
}
