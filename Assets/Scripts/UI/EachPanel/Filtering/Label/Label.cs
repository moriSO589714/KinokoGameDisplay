using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Label : MonoBehaviour
{
    public string MyLabelName { get; private set; } = "";
    public RectTransform MyRect { get; private set; }

    [SerializeField] private float _thisSpacingAreaWidth;
    [SerializeField] private GameObject _labelTxtObj;
    [SerializeField] private int _displayLabelMaxLength;
    [SerializeField] private UIActBase _deleteLabelButton;

    private Action<Label> _deleteThisLabelAct;
    private Text _labelTxt;

    private void Awake()
    {
        MyRect = this.GetComponent<RectTransform>();
        _labelTxt = _labelTxtObj.GetComponent<Text>();
        _deleteLabelButton.ClickAct += DeleteThis;
    }

    private void DeleteThis()
    {
        _deleteThisLabelAct?.Invoke(this);
        _deleteThisLabelAct = null;
    }

    public void ActivateLabel(string labelName, Action<Label> whenDeleteAct)
    {
        MyLabelName = labelName;
        SetLabelTxt(labelName);
        _deleteThisLabelAct = whenDeleteAct;
        FlexibleMyWidth();
    }

    private void SetLabelTxt(string labelName)
    {
        string displayTxt = labelName;
        if(labelName.Count() > _displayLabelMaxLength)
        {
            displayTxt = StrTools.ReplaceOverWords(displayTxt, _displayLabelMaxLength);
        }

        _labelTxt.text = displayTxt;
    }

    /// <summary>
    /// 現在のラベルテキストに表示されている文字数から自身の横幅を調整する
    /// </summary>
    private void FlexibleMyWidth()
    {
        //テキストオブジェクトが占める横幅を取得する
        float txtWidth = _labelTxt.preferredWidth * _labelTxtObj.GetComponent<RectTransform>().localScale.x;
        //テキストに利用しない部分を足す(バツボタンの部分等)
        float labelWidth = txtWidth + _thisSpacingAreaWidth;

        //反映
        MyRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, labelWidth);
    }
}
