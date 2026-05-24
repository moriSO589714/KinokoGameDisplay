using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WifiPanelManager : UIPanel
{
    [SerializeField] SimpleUI CloseMark;

    public override void InitPanel()
    {
        base.InitPanel();
        //停止ボタンを押したときの処理をボタンオブジェクトのデリゲートに設定
        CloseMark.ClickAct = OnCloseProc;
    }

    protected override void OnCloseProc()
    {
        base.OnCloseProc();
    }
}
