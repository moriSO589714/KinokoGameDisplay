using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ダウンロードキューの進捗をUI側で視覚化・操作するためのクラス
/// </summary>
public class WatchingGameDlCueForUI : MonoBehaviour
{
    [SerializeField] GameDlCue _gameDlCue;

    private string _lastProgressTaskName = "";
    private float _lastProgressPercentage = -1;

    //キューの最新タスクの更新が行われた際に発火させるメソッド
    public Action UpdateProgressTaskAct = null;
    public Action<GameDlTask> UpdateProgressTaskActForTaskAct = null;
    public Action EndProgressTaskAct;

    public Action<float> UpdateProgressInPercentageAct = null;
    public Action ChangeTaskEmptyAct = null;

    /// <summary>
    /// アップデートでキューの状態を監視する
    /// </summary>
    private void Update()
    {
        GameDlProgress gameDlProgress = _gameDlCue.CurrentGameDlProgress;
        if(gameDlProgress != null)
        {
            if(gameDlProgress.TaskName != _lastProgressTaskName) 
            {
                if(_lastProgressTaskName != "")
                {
                    CallProgressTaskEnd();
                }

                CallUpdateProgressTask(_gameDlCue.GameDlTasksList[0]);
            }

            if(gameDlProgress.NowPercentage != _lastProgressPercentage)
            {
                UpdateProgressInPercentage(gameDlProgress.NowPercentage);
            }
        }
        else
        {
            //キューのタスクが0になった瞬間
            if(_gameDlCue.GameDlTasksList.Count == 0 && _lastProgressTaskName != "")
            {
                ChangeTaskEmptyState();
            }
        }
    }

    public List<GameDlTask> CheckTasks()
    {
        return new List<GameDlTask>(_gameDlCue.GameDlTasksList);
    }

    public List<GameDlError> CheckErrorTasks()
    {
        return new List<GameDlError>(_gameDlCue.ErrorTasksList);
    }

    /// <summary>
    /// 現在実行中のタスクが更新された際に呼ばれる
    /// キューがなくなる際は呼ばれないので注意
    /// </summary>
    private void CallUpdateProgressTask(GameDlTask newTask)
    {
        _lastProgressTaskName = newTask.TaskName;

        UpdateProgressTaskAct?.Invoke();
        UpdateProgressTaskActForTaskAct?.Invoke(newTask);
    }

    private void UpdateProgressInPercentage(float percentage)
    {
        UpdateProgressInPercentageAct?.Invoke(percentage);
    }

    /// <summary>
    /// タスクが終了した際に呼ばれる
    /// </summary>
    private void CallProgressTaskEnd()
    {
        EndProgressTaskAct?.Invoke();
    }


    /// <summary>
    /// ダウンロードタスクが存在した状態から存在しない状態になった瞬間呼ばれる
    /// </summary>
    private void ChangeTaskEmptyState()
    {
        CallProgressTaskEnd();
        _lastProgressTaskName = "";
        _lastProgressPercentage = 0;
        ChangeTaskEmptyAct?.Invoke();
    }
}
