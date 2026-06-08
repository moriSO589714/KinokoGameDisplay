using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionPanelManager : UIPanel
{
    [SerializeField] private UIActBase _closeMrk;
    [SerializeField] private UIActBase _toSettingMrk;
    [SerializeField] private UIActBase _toDevMrk;
    [SerializeField] private UIActBase _toReportMrk;

    [SerializeField] private OverrapUIPanel _settingPanel;
    [SerializeField] private OverrapUIPanel _reportPanel;

    public override void InitPanel()
    {
        base.InitPanel();

        //各ボタンへのデリゲート設定
        _closeMrk.ClickAct = OnCloseProc;

        _toSettingMrk.ClickAct = () => CreateOverrapPanel(_settingPanel);
        _toReportMrk.ClickAct = () => CreateOverrapPanel(_reportPanel);
    }

    public void CreateOverrapPanel(OverrapUIPanel createPanel)
    {
        createPanel.gameObject.SetActive(true);
    }
}
