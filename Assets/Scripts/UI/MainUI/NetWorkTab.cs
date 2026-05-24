using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetWorkTab : DownTab
{
    [SerializeField] GameObject NetWorkPanelPref;
    public Action<GameObject> InstancePanel;
    public override void OnClickAct()
    {
        InstancePanel(NetWorkPanelPref);
    }
}
