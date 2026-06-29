using UnityEngine;
using System.Collections.Generic;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine.UIElements;

public class DlProgressPanelManager : UIPanel
{
    [SerializeField] private UIActBase _closeButton;
    [SerializeField] private WaitAndErrorBoxsManager _waitAndErrorBoxsManager;

    [SerializeField] private MonitorPlayerInput _monitorPlayerInput;
    [SerializeField] private WatchingGameDlCueForUI _watchingGameDlCueForUI;
    [SerializeField] private GameDlCue _gameDlCue;
    [SerializeField] private DlProgressTaskBoxForPanel _dlProgressTaskBox;

    protected override void Awake()
    {
        base.Awake();
        _closeButton.ClickAct = OnCloseProc;

        //進捗度バー用デリゲート
        _watchingGameDlCueForUI.UpdateProgressInPercentageAct += _dlProgressTaskBox.SetProgress;
        _watchingGameDlCueForUI.UpdateProgressTaskAct += TriggerChangeProgressTask;
        _watchingGameDlCueForUI.ChangeTaskEmptyAct += EndAllProgress;
    }

    public override void InitPanel()
    {
        base.InitPanel();
        _dlProgressTaskBox.InitBox();
        //waitBoxsの初期化もここで行う

        UpdateDisplay();

        //マウススクロール割り当て
        if (_monitorPlayerInput != null)
        {
            Action<float> act = _waitAndErrorBoxsManager.OnScroll;
            _monitorPlayerInput.OnMouseScroll += act;

            //メイン画面に戻った際に割り当てを解除する
            CommonStateManager commonStateManager = CommonStateManager.Instance;
            commonStateManager.AddOutLoadingFunc(() => { _monitorPlayerInput.OnMouseScroll -= act; });
        }
    }

    /// <summary>
    /// ダウンロードキューの進行中タスクが更新された際に呼び出される
    /// </summary>
    private void TriggerChangeProgressTask()
    {
        UpdateDisplay();
    }

    /// <summary>
    /// 実行されているタスクが全てなくなった際に呼ばれる
    /// </summary>
    private void EndAllProgress()
    {
        //progressBoxの初期化
        _dlProgressTaskBox.InitBox();
        UpdateDisplay();
    }

    /// <summary>
    /// キューのフィールドを読み取って、表示のリロードを行う
    /// </summary>
    private void UpdateDisplay()
    {
        List<GameDlTask> inCueTasks = _watchingGameDlCueForUI.CheckTasks();
        List<GameDlError> errorTasks = _watchingGameDlCueForUI.CheckErrorTasks();

        _waitAndErrorBoxsManager.ClearField();
        if (inCueTasks == null || inCueTasks.Count == 0)
        {

        }
        else
        {
            //現在進行中のダウンロードタスク表示
            SetProgressTaskBox(inCueTasks[0]);
            
            //以下ダウンロード待ちタスクを表示する処理
            //ダウンロード待ちタスクが無い場合
            if(inCueTasks.Count == 1)
            {

            }
            else
            {
                //進行中のタスク以外のリストを作成
                List<GameDlTask> waitTasks = inCueTasks.GetRange(1, inCueTasks.Count - 1);
                SetWaitTaskBoxs(waitTasks);
            }
        }

        //エラーが起こったタスクをエラータスクボックスとして追加で生成
        if(errorTasks != null && errorTasks.Count != 0)
        {
            SetErrorTaskBoxs(errorTasks);
        }
    }

    private void SetProgressTaskBox(GameDlTask progressTask)
    {
        _dlProgressTaskBox.SetNewTask(progressTask);
        _dlProgressTaskBox.SetButtonAct(DeleteProgressTaskForUI, DownProgressTaskForUI);
    }

    /// <summary>
    /// UI上で現在実行中のタスクを停止する処理をボタンを押した際の処理
    /// </summary>
    private void DeleteProgressTaskForUI(string taskName)
    {
        _gameDlCue.DeleteTaskFromIndex(0, taskName);
        UpdateDisplay();
    }

    /// <summary>
    /// UI上で現在実行中のタスクを1つ下にずらした時(下向きの矢印ボタン押下)の処理
    /// </summary>
    private void DownProgressTaskForUI()
    {
        List<GameDlTask> tasksList = _watchingGameDlCueForUI.CheckTasks();
        //待っているタスクが無い場合は実行しない
        if(tasksList == null || tasksList.Count <= 1)
        {
            return;
        }

        _gameDlCue.ReplaceProgressTaskWithNewTask(1, tasksList[1].TaskName);
    }

    private void SetWaitTaskBoxs(List<GameDlTask> waitTasks)
    {
        //ダウンロード待ちタスクボックスの生成
        _waitAndErrorBoxsManager.GenerateTaskBoxs(waitTasks, ImmediatelyDoingTask, TransferWaitTask, DeleteTask);
    }

    private void SetErrorTaskBoxs(List<GameDlError> gameDlError)
    {
        //エラーボックスの生成
        _waitAndErrorBoxsManager.GenerateErrorBoxs(gameDlError, x => RecoveryErrorTask(x).Forget(), DeleteErrorTask);
    }

    /// <summary>
    /// waitタスクの優先度を変更する(リスト内の場所の変更)
    /// </summary>
    /// <param name="taskName">移動するタスクの名前</param>
    /// <param name="direction">方向(-1でlistのindexを-1)</param>
    private void TransferWaitTask(string taskName, int direction)
    {
        List<GameDlTask> tasksList = _watchingGameDlCueForUI.CheckTasks();
        int index = tasksList.FindIndex(x => x.TaskName == taskName);
        if (index == 0) return;

        //移動後のインデックス値
        int movedIndex = index + direction;
        if(movedIndex == 0) //実行中のタスクと入れ替え
        {
            ImmediatelyDoingTask(taskName);
        }
        else if (movedIndex <= tasksList.Count - 1 && movedIndex > 0)
        {
            _gameDlCue.ReplaceTaskIndex((index, taskName), (movedIndex, tasksList[movedIndex].TaskName));
            UpdateDisplay();
        }
    }

    private void ImmediatelyDoingTask(string taskName)
    {
        List<GameDlTask> waitTaskList = _watchingGameDlCueForUI.CheckTasks();
        int index = waitTaskList.FindIndex(x => x.TaskName == taskName);

        _gameDlCue.ReplaceProgressTaskWithNewTask(index, taskName);
    }

    private void DeleteTask(string taskName)
    {
        List<GameDlTask> waitTasksList = _watchingGameDlCueForUI.CheckTasks();
        int index = waitTasksList.FindIndex(x => x.TaskName == taskName);

        _gameDlCue.DeleteTaskFromIndex(index, taskName);

        UpdateDisplay();
    }

    private void DeleteErrorTask(string taskName)
    {
        List<GameDlError> errorTasks = _watchingGameDlCueForUI.CheckErrorTasks();

        int index = errorTasks.FindIndex(x => x.Task.TaskName == taskName);
        _gameDlCue.DeleteErrorTask(index);

        UpdateDisplay();
    }

    private async UniTask RecoveryErrorTask(string taskName)
    {
        List<GameDlError> errorTasks = _watchingGameDlCueForUI.CheckErrorTasks();

        //タスク名からerrorListのインデックスを取得
        int index = errorTasks.FindIndex(x => x.Task.TaskName == taskName);
        try
        {
            await _gameDlCue.RecoveryAndDlInErrorTasksList(index);
        }
        catch (Exception e)
        {
            throw;
        }

        UpdateDisplay();
    }
}