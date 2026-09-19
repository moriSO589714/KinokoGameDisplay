using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CmdDownloadGame : CmdActForUseNetwork
{
    private FilterCondition _currentFilterCondition = new FilterCondition();
    private CmdFiltering _filtering;
    private List<GameData> _candidateGameDatas = new List<GameData>();

    [SerializeField] private GameDlCue _gameDlCue;
    [SerializeField] private WatchingGameDlCueForUI _watchingGameDlCue;    

    public override void FirstCall()
    {
        base.FirstCall();
        _cmdSceneManager.OutPutManager.ReceiveMessage("ダウンロードモードに変更します", OutPutTextLogColorSets.SystemDefault);
        _filtering = new CmdFiltering(ReceiveCmdFiltering);

        try
        {
            PrepareUsingNetwork();
        }
        catch (Exception e)
        {
            if (_ctsForLoadSpreadSheet.IsCancellationRequested) return;

            _cmdSceneManager.OutPutManager.ReceiveMessage("ゲーム情報の取得に失敗しました。モードを終了します。", OutPutTextLogColorSets.AccentDefault);
            ReturnCmdReceiveMode();
            Debug.LogException(e);
            return;
        }
    }

    protected override void End()
    {
        base.End();

        _gameDlCue.DeleteAllTasks();
        _gameDlCue.DeleteAllErrorTasks();
        _candidateGameDatas = new List<GameData>();
    }

    protected override async UniTask LoadSpreadSheetData()
    {
        await base.LoadSpreadSheetData();
        if (_ctsForLoadSpreadSheet.IsCancellationRequested) return;

        DoFiltering();
    }

    private void DoFiltering()
    {
        _cmdSceneManager.OutPutManager.ReceiveMessage("ダウンロードするゲームのフィルタリング情報を送信してください", OutPutTextLogColorSets.SystemDefault);
        _filtering.WaitSendCategory();
    }

    private void ReceiveCmdFiltering(FilterCondition sendedFilterCondition)
    {
        _currentFilterCondition = sendedFilterCondition;

        _cmdSceneManager.OutPutManager.ReceiveMessage("当てはまるゲームを検索中", OutPutTextLogColorSets.SystemDefault);
        //一致するGameDataクラスを取得してくる
        List<GameData> matchGameDatas = PickUpMatchGameData(sendedFilterCondition);

        if(matchGameDatas != null && matchGameDatas.Count > 0)
        {
            string checkMessage = $"当てはまるゲームが{matchGameDatas.Count}個存在します。";
            foreach(GameData gameData in matchGameDatas)
            {
                checkMessage += $"\n・{gameData.GameTitle}| (id){gameData.GameID}";
            }
            _cmdSceneManager.OutPutManager.ReceiveMessage(checkMessage, OutPutTextLogColorSets.SystemDefault);

            _cmdSceneManager.OutPutManager.ReceiveMessage($"ダウンロードを実行しますか？", OutPutTextLogColorSets.SystemDefault);
            _cmdSceneManager.InputFieldManager.ChangeAction(CheckDownload);
        }
        else
        {
            _cmdSceneManager.OutPutManager.ReceiveMessage("当てはまるゲームが存在しません。フィルタリング設定を修正してください", OutPutTextLogColorSets.AccentDefault);
            DoFiltering();
        }
    }

    private void CheckDownload(bool isDownload)
    {
        if (!isDownload)
        {
            DoFiltering();
            return;
        }
        _cmdSceneManager.OutPutManager.ReceiveMessage("ゲームのダウンロードを開始します。", OutPutTextLogColorSets.SystemDefault);
        //コマンドを受け付けないようにしておく
        _cmdSceneManager.InputFieldManager.ChangeAction(new CmdNothing().MessageGird);
        DoDownload();
    }

    private List<GameData> PickUpMatchGameData(FilterCondition filterCondition)
    {
        GameDatasSingleton gameDatasSingleton = GameDatasSingleton.Instance;
        List<GameData> matchGameDatas = GameBoxFilter.FilteringGameDatas(_currentFilterCondition, gameDatasSingleton.AllGameDatas);
        //現在のダウンロード候補としてフィールドに保存しておく
        _candidateGameDatas = new List<GameData>(matchGameDatas);

        return matchGameDatas;
    }

    /// <summary>
    /// 実際にダウンロードを実行
    /// </summary>
    private void DoDownload()
    {
        DownloadGame downloadGame = new DownloadGame();
        List<GameDlTask> gameDlTasks = new List<GameDlTask>();

        foreach(GameData gameData in _candidateGameDatas)
        {
            GameDlTask dlTask = downloadGame.CreateGameDlTaskAndAddCue(gameData, _gameDlCue);
            gameDlTasks.Add(dlTask);
        }

        _watchingGameDlCue.ChangeTaskEmptyAct = ()=> EndDownload(_watchingGameDlCue);
        LoggerDuaringDownload();
    }

    /// <summary>
    /// ダウンロード中にコマンドラインに進捗ログを流すメソッド
    /// </summary>
    private void LoggerDuaringDownload()
    {
        string currentDlGameTitle = _gameDlCue.GameDlTasksList[0].TaskInstance.GameData.GameTitle;
        string lastPercentage = _watchingGameDlCue.ReturnLastPercentage();

        CmdDownloadProgressLog cmdDlProgressLog = new CmdDownloadProgressLog();
        string logText = cmdDlProgressLog.UpdateTitle(currentDlGameTitle, lastPercentage);
        string logId = _cmdSceneManager.OutPutManager.ReceiveMessage(logText, OutPutTextLogColorSets.SystemDefault);

        _watchingGameDlCue.UpdateProgressTaskActForTaskAct = 
            (GameDlTask task) =>  _cmdSceneManager.OutPutManager.ReceiveMessage(cmdDlProgressLog.UpdateTitle(task.TaskInstance.GameData.GameTitle), OutPutTextLogColorSets.SystemDefault, specifiedUUID: logId);
        _watchingGameDlCue.UpdateProgressInPercentageAct =
            (float progress) => _cmdSceneManager.OutPutManager.ReceiveMessage(cmdDlProgressLog.UpdatePercentage(progress.ToString()), OutPutTextLogColorSets.SystemDefault, specifiedUUID: logId);            
    }

    /// <summary>
    /// 予定されたダウンロードが全て終了した際に呼ばれる
    /// </summary>
    private void EndDownload(WatchingGameDlCueForUI watchingGameDlCueForUI)
    {
        _cmdSceneManager.OutPutManager.ReceiveMessage("全てのダウンロードが終了しました。", OutPutTextLogColorSets.SystemDefault);

        //エラーが発生したタスクが無いかの確認
        List<GameDlError> errorTasks = watchingGameDlCueForUI.CheckErrorTasks();
        if(errorTasks != null && errorTasks.Count > 0)
        {
            //エラーが発生した場合
            HappenError(errorTasks);
            return;
        }

        ReturnCmdReceiveMode();
    }

    private void HappenError(List<GameDlError> errorTasks)
    {
        string errorLog = "エラー終了したゲームが存在します!!\n【一覧】";
        foreach (GameDlError gameDlError in errorTasks)
        {
            errorLog += $"\n・ゲームタイトル：{gameDlError.Task.TaskInstance.GameData.GameTitle},エラー分類：{gameDlError.DlException.GameDlErrorType.ToString()}";
        }
        _cmdSceneManager.OutPutManager.ReceiveMessage(errorLog, OutPutTextLogColorSets.AccentDefault);
        CmdReturn endThisMode = new CmdReturn(ReturnCmdReceiveMode);
        //エラーが発生したゲームタイトルのwecを作成
        WordEmtCell errorTitleWEC = CreateLibFromGameDatas.CreateGameIdLib(errorTasks.Select(errorTask => errorTask.Task.TaskInstance.GameData).ToList());
        _cmdSceneManager.InputFieldManager.ChangeAction
            ((string message) => RecoveryError(message, errorTasks, endThisMode), errorTitleWEC);
        _cmdSceneManager.OutPutManager.ReceiveMessage($"エラーが発生したゲームのIDを送信することで回復処理を実行できます。\n「{endThisMode.ReturnWord}」を送信することでコマンド受信モードに戻ります。", OutPutTextLogColorSets.AccentDefault);
        return;
    }

    private void RecoveryError(string message, List<GameDlError> errorTasks, CmdReturn cmdReturn)
    {
        //コマンド受信に戻る(ダウンロードモードの終了)
        if (cmdReturn.ReturnCheck(message)) return;

        //送信されたIDに合致するGameDlErrorクラスを取ってくる
        int targetErrorIndex = errorTasks.FindIndex(errorTask => errorTask.Task.TaskInstance.GameData.GameID == message);
        if(targetErrorIndex == -1)
        {
            _cmdSceneManager.OutPutManager.ReceiveMessage("送信されたIDはエラーが発生したゲームリストに含まれていません。送信し直してください", OutPutTextLogColorSets.AccentDefault);
            return;
        }
        GameDlError targetErrorTask = errorTasks[targetErrorIndex];
        //エラーの種別を解析。回復処理が可能なら実行する。不可能なら詳細のログを流して終了
        switch (targetErrorTask.DlException.GameDlErrorType) 
        {
            case GameDlErrorType.NeedCleanDirectory:
                DoRecovery(targetErrorTask.Task.TaskName);
                break;
            case GameDlErrorType.NeedRetryAccessDrive:
                DoRecovery(targetErrorTask.Task.TaskName);
                break;
            default:
                _cmdSceneManager.OutPutManager.ReceiveMessage($"このモードで回復不可能なエラーです。管理者にお問い合わせ下さい\nエラー詳細：{targetErrorTask}", OutPutTextLogColorSets.AccentDefault);                
                ReturnCmdReceiveMode();
                return;
        }
    }

    private async UniTask DoRecovery(string taskName)
    {
        //リカバリ中はメッセージ送信を受け付けない
        _cmdSceneManager.InputFieldManager.ChangeAction(new CmdNothing().MessageGird);
        _cmdSceneManager.OutPutManager.ReceiveMessage("回復処理を実行します", OutPutTextLogColorSets.SystemDefault);
        List<GameDlError> errorTasks = _watchingGameDlCue.CheckErrorTasks();

        //タスク名からerrorListのインデックスを取得
        int index = errorTasks.FindIndex(x => x.Task.TaskName == taskName);
        try
        {
            await _gameDlCue.RecoveryAndDlInErrorTasksList(index);
            LoggerDuaringDownload();
        }
        catch (Exception e)
        {
            throw;
        }
    }
}