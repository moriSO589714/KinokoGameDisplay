using System;
using UnityEngine;
using UnityEngine.UI;

public class DlProgressTaskBox : MonoBehaviour
{
    [SerializeField] private Text _gameTitleTxt;
    [SerializeField] private Text _progressDescription;
    [SerializeField] private BarManager _barManager;

    protected GameDlTask _currentTask;

    private void Init()
    {
        _gameTitleTxt.text = "ダウンロードが実行されていません";
        _progressDescription.text = "";
        _barManager.InitFromOther();
        _barManager.SetPercentage(0f);
    }

    public void InitBox()
    {
        Init();
    }

    public void SetNewTask(GameDlTask newTask)
    {
        _currentTask = newTask;
        _gameTitleTxt.text = newTask.TaskInstance.GameData.GameTitle;
    }

    public void SetProgress(float percentage)
    {
        string progressTxt = "";
        if(percentage >= 100)
        {
            progressTxt = "最終処理を実行中";
        }
        else
        {
            progressTxt = $"{percentage.ToString("F1")}%までダウンロード済";
        }

        _progressDescription.text = progressTxt;
        _barManager.SetPercentage(percentage);
    }
}
