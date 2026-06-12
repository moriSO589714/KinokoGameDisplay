using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 基本的な機能(「はい」、「いいえ」のボタンなど)のみの使いまわし用パネル
/// </summary>
public class GeneralPanel : UIPanel
{
    [SerializeField] private Text _titleTxt;
    [SerializeField] private Text _descriptionTxt;
    [SerializeField] private UIActBase _yesButton;
    [SerializeField] private UIActBase _noButton;
    [SerializeField] private UIActBase _closeButton;

    public override void InitPanel()
    {
        base.InitPanel();
        _closeButton.ClickAct += OnCloseProc;
    }

    /// <summary>
    /// 生成時にパネルの情報をセットするメソッド
    /// </summary>
    public void SetPanelData(string titleTxt = "", string descriptionTxt = "", Action YesButtonAct = null, Action NoButtonAct = null)
    {
        _titleTxt.text = titleTxt;
        _descriptionTxt.text = descriptionTxt;

        if (YesButtonAct == null)
        {
            _yesButton.gameObject.SetActive(false);
        }
        else
        {
            _yesButton.ClickAct = YesButtonAct;
        }

        if(NoButtonAct == null)
        {
            _noButton.gameObject.SetActive(false);
        }
        else
        {
            _noButton.ClickAct = NoButtonAct;
        }
    }
}
