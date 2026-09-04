using Cysharp.Threading.Tasks;
using Google.Apis.Drive.v3;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class CmdOnlineDelete : CmdDelete
{
    private OnNetGetParentId _onNetGetParentId;
    private OnNetDriveGetName _onNetDriveGetName;
    private OnNetDelete _onNetDelete;
    private DeleteProc _deleteProc;

    private CancellationTokenSource _forDeleteCts;

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
        _cmdSceneManager.OutPutManager.ReceiveMessage("オンラインゲーム削除モードに変更します", OutPutTextLogColorSets.SystemDefault);
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
        List<GameData> gameDatas = gameDatasSingleton.AllGameDatas;
        //ローカルのみに保存されているものを除く
        gameDatas.RemoveAll(x => x.Status == GameStatus.ByLocal);
        //idを取り出してwecにする
        _gameIdLib = CreateLibFromGameDatas.CreateGameIdLib(gameDatas);

        //処理にキャンセルが入っていた場合
        if (token.IsCancellationRequested)
        {
            return;
        }

        _cmdSceneManager.OutPutManager.ReceiveMessage("接続成功。初期処理を実行中", OutPutTextLogColorSets.SystemDefault);
        CmdDeleteEntrance();
    }

    protected override bool CheckGameStatus(GameStatus status)
    {
        if (status == GameStatus.ByLocal) return false;
        return true;
    }

    protected override async UniTask DoDelete(GameData targetGameData)
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