using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DownloadProgressTabManager : UIPanel
{
    [SerializeField]private SlideSideUIAct _thisUIAct;
    [SerializeField]private WatchingGameDlCueForUI _watchCue;
    [SerializeField]private Text _gameTitleTxt;
    [SerializeField]private Text _progress;
    [SerializeField]private Image _progressBar;

    [SerializeField]private DlProgressPanelManager _progressPanelManager;

    private BarManager _barManager;

    protected override void Awake()
    {
        base.Awake();
        _barManager = new BarManager(_progressBar);
        _thisUIAct.ClickAct += ActivateProgressPanel;
        _watchCue.UpdateProgressInTaskNameAct += SetNewtitle;
        _watchCue.UpdateProgressInPercentageAct += SetNewProgress;
        _watchCue.ChangeTaskEmptyAct += EndAllProgress;
    }

    public override void InitPanel()
    {
    }

    public void SetNewtitle(string title)
    {
        _gameTitleTxt.text = title;
        //出現アニメーションを再度行う
        _thisUIAct.OpeningAct();
    }

    public void SetNewProgress(float percentage)
    {
        string progressTxt = "";
        if(percentage >= 100)
        {
            progressTxt = "最終処理を実行中";
        }
        else
        {
            progressTxt = $"{percentage.ToString("G3")}%まで終了";
        }
        _progress.text = progressTxt;
        _barManager.SetPercentage(percentage);
    }

    public void EndAllProgress()
    {
        _thisUIAct.HideThisTab();
    }

    private void ActivateProgressPanel()
    {
        _progressPanelManager.gameObject.SetActive(true);
    }
}
