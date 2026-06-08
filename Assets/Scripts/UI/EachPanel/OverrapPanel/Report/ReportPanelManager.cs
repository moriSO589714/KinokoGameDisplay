using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReportPanelManager : OverrapUIPanel
{
    [SerializeField] private UIActBase _copyLinkButton;
    [SerializeField] private UIActBase _closeButton;
    [SerializeField] private string _formURL;

    public override void InitPanel()
    {
        base.InitPanel();

        CopyTxtForCripBoard copyTxtForCripBoard = new CopyTxtForCripBoard();
        //デリゲート設定
        _copyLinkButton.ClickAct = () => copyTxtForCripBoard.CopyTxt(_formURL);
        _closeButton.ClickAct = OnCloseProc;
    }
}
