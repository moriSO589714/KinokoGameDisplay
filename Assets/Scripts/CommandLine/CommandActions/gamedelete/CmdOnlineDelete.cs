using Cysharp.Threading.Tasks;
using Google.Apis.Drive.v3;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class CmdOnlineDelete : CmdAct
{
    private OnNetGetParentId _onNetGetParentId;
    private OnNetDriveGetName _onNetDriveGetName;
    private OnNetDelete _onNetDelete;
    private DeleteProc _deleteProc;

    private CancellationTokenSource _forDeleteCts;

    WordEmtCell _gameIdLib;

    protected override void Init()
    {
        if (CheckInEnvironment.isOnNet)
        {
            DriveService driveService = NetworksSingleton.Instance.ReturnDriveService();
            _onNetGetParentId = new OnNetGetParentIdfromDv(driveService);
            _onNetDriveGetName = new OnNetDriveGetNamefromDv(driveService);
            _onNetDelete = new OnNetDeleteforDv(driveService);

            _deleteProc = new DeleteProc(_onNetDelete, _onNetGetParentId, _onNetDriveGetName);
        }
        else
        {
            _onNetGetParentId = new OnNetGetParentIdfromTest();
            _onNetDriveGetName = new OnNetDriveGetNamefromTest();
            _onNetDelete = new OnNetDeleteforTest();

            _deleteProc = new DeleteProc(_onNetDelete, _onNetGetParentId, _onNetDriveGetName);
        }
    }

    protected override void End()
    {
        _onNetGetParentId = null;
        _onNetDriveGetName = null;
        _onNetDelete = null;
        _forDeleteCts?.Cancel();
    }

    public override void FirstCall()
    {
        base.FirstCall();
        _cmdSceneManager.OutPutManager.ReceiveMessage("削除モードに変更します", OutPutTextLogColorSets.SystemDefault);
        //モードから出る時にトークンをキャンセルするようにしておく
        _cmdSceneManager.InputFieldManager._endModeAction += () => { _forDeleteCts?.Cancel(); };
        //スプシのロード中でコマンドの受付を行わないようにしておく
        _cmdSceneManager.InputFieldManager.ChangeAction(new CmdNothing().MessageGird);

        CancellationTokenSource cts = new CancellationTokenSource();
        _cmdSceneManager.InputFieldManager._endModeAction += () => { cts.Cancel(); };
        LoadSpreadSheet(cts.Token);
    }

    public async UniTask LoadSpreadSheet(CancellationToken token)
    {
        GameDataManager gameDataManager = new GameDataManager();

        string connectLogStr = "インターネットに接続して、ゲーム情報を取得しています";
        string messageId = _cmdSceneManager.OutPutManager.ReceiveMessage(connectLogStr, OutPutTextLogColorSets.SystemDefault);
        CancellationTokenSource ctsForLogAnim = new CancellationTokenSource();
        new CmdWaitingAnimInLog().LoopWaitingLog(connectLogStr, OutPutTextLogColorSets.SystemDefault, messageId, ctsForLogAnim.Token);

        try
        {
            await UniTask.RunOnThreadPool(gameDataManager.LoadGameDataFromSpSt);
        }
        catch(System.Exception e)
        {
            ctsForLogAnim.Cancel();
            _cmdSceneManager.OutPutManager.ReceiveMessage("ゲーム情報の取得に失敗しました", OutPutTextLogColorSets.AccentDefault);
            ReturnCmdReceiveMode();
            return;
        }
        ctsForLogAnim.Cancel();

        //wecとして取得する
        GameDatasSingleton gameDatasSingleton = GameDatasSingleton.Instance;
        _gameIdLib = gameDatasSingleton.ReturnGameIdLib();

        //処理にキャンセルが入っていた場合
        if (token.IsCancellationRequested)
        {
            return;
        }

        _cmdSceneManager.OutPutManager.ReceiveMessage("接続成功。初期処理を実行中", OutPutTextLogColorSets.SystemDefault);
        CmdDeleteEntrance();
    }

    private void CmdDeleteEntrance()
    {
        _cmdSceneManager.InputFieldManager.ChangeAction(ReceiveGameId, _gameIdLib);
        _cmdSceneManager.OutPutManager.ReceiveMessage("削除したいゲームのゲームIDを送信してください", OutPutTextLogColorSets.SystemDefault);
    }

    private void ReceiveGameId(string message)
    {
        //送信されたゲームidを持つGameDataクラスを取得してくる
        GameDatasSingleton gameDatasSingleton = GameDatasSingleton.Instance;
        List<GameData> allGameDataList = new List<GameData>(gameDatasSingleton.AllGameDatas);
        List<GameData> matchGameData = allGameDataList.Where(x => x.GameID == message).ToList();

        if(matchGameData.Count < 1)
        {
            _cmdSceneManager.OutPutManager.ReceiveMessage("合致するIDを持つゲームが存在しません", OutPutTextLogColorSets.AccentDefault);
            CmdDeleteEntrance();
            return;
        }

        if(matchGameData.Count > 1)
        {
            _cmdSceneManager.OutPutManager.ReceiveMessage("合致するIDを持つゲームが複数存在します", OutPutTextLogColorSets.AccentDefault);
            CmdDeleteEntrance();
            return;
        }

        GameData targetGameData = matchGameData[0];

        //ローカルで追加されたゲームである場合は弾く
        if(targetGameData.Status == GameStatus.ByLocal)
        {
            _cmdSceneManager.OutPutManager.ReceiveMessage("対象のゲームはローカルにしか存在しないため、このコマンドでは削除することができません", OutPutTextLogColorSets.AccentDefault);
            CmdDeleteEntrance();
            return;
        }

        CheckDelete(targetGameData);
    }

    private void CheckDelete(GameData deleteTarget)
    {
        string gameTitle = deleteTarget.GameTitle;
        string gameId = deleteTarget.GameID;
        string gameVersion = deleteTarget.GameVersion;
        string[] gameDevelopper = deleteTarget.GameDevelopper;

        string checkLog = $"削除するゲームの確認を行ってください。この操作は取り消せません、誤ったゲームを指定していないか確認をお願いします\n" +
            $"削除を行う場合はゲームのタイトル名を送信してください\n【ゲーム情報】\n・ゲームタイトル：{gameTitle}" +
            $"\n・ゲームID：{gameId}\n・ゲームのバージョン(最終更新日)：{gameVersion}\n・ゲーム開発者名：{string.Join(",", gameDevelopper)}";

        _cmdSceneManager.OutPutManager.ReceiveMessage(checkLog, OutPutTextLogColorSets.SystemDefault);
        _cmdSceneManager.InputFieldManager.ChangeAction((string val) => ReceiveTitle(val, deleteTarget));
    }

    private void ReceiveTitle(string message, GameData targetGameData)
    {
        //タイトルに一致するか確認する
        if(message == targetGameData.GameTitle)
        {
            DoDelete(targetGameData);
        }
        else
        {
            _cmdSceneManager.OutPutManager.ReceiveMessage("タイトルと一致していません", OutPutTextLogColorSets.AccentDefault);
            CmdDeleteEntrance();
            return;
        }
    }

    private async UniTask DoDelete(GameData targetGameData)
    {
        _cmdSceneManager.OutPutManager.ReceiveMessage("ゲームの削除を開始します", OutPutTextLogColorSets.SystemDefault);
        _cmdSceneManager.InputFieldManager.ChangeAction(new CmdNothing().MessageGird);
        _forDeleteCts = new CancellationTokenSource();
        try
        {
            await _deleteProc.UniDeleteDriveGame(targetGameData.GameDriveId, targetGameData.GameID, _forDeleteCts.Token);
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