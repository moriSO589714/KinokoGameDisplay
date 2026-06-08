using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : OverrapUIPanel
{
    [SerializeField] private Text _description;
    [SerializeField] private UIActBase _closeButton;

    public override void InitPanel()
    {
        _closeButton.gameObject.SetActive(false);
    }

    public void AvailableClose()
    {
        //閉じるボタンの有効化
        _closeButton.gameObject.SetActive(true);
        _closeButton.ClickAct = OnCloseProc;
    }

    public void SetLogTxt(string log)
    {
        _description.text = log;
    }

    protected override void OnCloseProc()
    {
        //プレハブとして生成されるため、閉じる際は自身を消す
        Destroy(this.gameObject);
    }
}
