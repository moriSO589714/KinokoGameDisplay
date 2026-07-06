using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SortElementManager : MonoBehaviour
{
    [SerializeField] PullDown _pullDown;
    [SerializeField] UIActBase _myUIAct;
    [SerializeField] Text _curretnSelectedTxt;
    [SerializeField] RotateUIAct _arrowUI;

    public string _currentSelectElement { get; private set; } = "";
    private bool _isActivePullDown = false;
    private List<string> _sortCategory = new List<string>();

    private void Awake()
    {
        _sortCategory = SetPullDownList();
        _myUIAct.ClickAct += OpenPullDownList;
        _pullDown.OnClikedCellAct += OnClickedPullDownCell;
    }

    /// <summary>
    /// 初期状態に戻す処理
    /// </summary>
    public void InitSortElement()
    {
        _currentSelectElement = "";
        _curretnSelectedTxt.text = "選択されていません";
        ClosePullDownList();
    }

    private void OpenPullDownList()
    {
        //既にプルダウンリストが展開されている場合はプルダウンを閉じる
        if (_isActivePullDown)
        {
            ClosePullDownList();
            return;
        }

        _pullDown.CreatePullDownList(_sortCategory);
        _arrowUI.SwitchRotate();
        _isActivePullDown = true;
    }

    private void ClosePullDownList()
    {
        _pullDown.ClosePullDownList();
        _arrowUI.SwitchRotate();
        _isActivePullDown = false;
    }

    /// <summary>
    /// セルがクリックされた際の処理
    /// </summary>
    private void OnClickedPullDownCell(string clickedCellTxt)
    {
        _currentSelectElement = clickedCellTxt;
        _curretnSelectedTxt.text = clickedCellTxt;
        ClosePullDownList();
    }

    protected virtual List<string> SetPullDownList()
    {
        return null;
    }
}