using UnityEngine;
using UnityEngine.UI;

public class DlProgressTabManager : UIPanel
{
    [SerializeField] private SlideSideUIAct _thisUIAct;
    [SerializeField] private WatchingGameDlCueForUI _watchCue;
    [SerializeField] private DlProgressPanelManager _progressPanelManager;

    [SerializeField] private DlProgressTaskBox _dlProgressTaskBox;

    protected override void Awake()
    {
        base.Awake();
        _thisUIAct.ClickAct += ActivateProgressPanel;
        _watchCue.UpdateProgressTaskActForTaskAct += UpdateTask;
        _watchCue.UpdateProgressInPercentageAct += UpdateProgress;
        _watchCue.ChangeTaskEmptyAct += EndAllProgress;
    }

    public override void InitPanel()
    {
    }

    public void UpdateTask(GameDlTask newTask)
    {
        _dlProgressTaskBox.SetNewTask(newTask);
        //出現アニメーションを再度行う
        _thisUIAct.OpeningAct();
    }

    public void UpdateProgress(float percentage)
    {
        _dlProgressTaskBox.SetProgress(percentage);
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
