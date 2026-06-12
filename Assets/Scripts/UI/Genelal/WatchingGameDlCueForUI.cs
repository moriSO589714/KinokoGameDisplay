using System;
using UnityEngine;

/// <summary>
/// ダウンロードキューの進捗をUI側で視覚化・操作するためのクラス
/// </summary>
public class WatchingGameDlCueForUI : MonoBehaviour
{
    [SerializeField] GameDlCue _gameDlCue;

    private string _lastProgressTaskName = "";
    private float _lastProgressPercentage = -1;

    public Action<string> UpdateProgressInTaskNameAct = null;
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
                UpdateProgressInTaskName(gameDlProgress.TaskName, gameDlProgress.GameName);
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

    private void UpdateProgressInTaskName(string taskName, string gameName)
    {
        _lastProgressTaskName = taskName;
        UpdateProgressInTaskNameAct?.Invoke(gameName);
    }

    private void UpdateProgressInPercentage(float percentage)
    {
        UpdateProgressInPercentageAct?.Invoke(percentage);
    }

    /// <summary>
    /// ダウンロードタスクが存在した状態から存在しない状態になった瞬間呼ばれる
    /// </summary>
    private void ChangeTaskEmptyState()
    {
        _lastProgressTaskName = "";
        _lastProgressPercentage = 0;
        ChangeTaskEmptyAct?.Invoke();
    }
}
