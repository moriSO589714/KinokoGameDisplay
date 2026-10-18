using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLibraryPanelManager : UIPanel
{

    [SerializeField] private MonitorPlayerInput _monitorPlayerInput;
    [SerializeField] private  LibraryCellsManager _libraryCellsManager;
    [SerializeField] private  GameLibraryMainPanelManager _mainPanelManager;
    [SerializeField] private GameLibraryMoveMainPanel _moveMainPanel;
    [SerializeField] private UIActBase _closeMrk;


    public override void InitPanel()
    {
        //マウススクロールの割り当て
        _monitorPlayerInput.OnMouseScroll += _libraryCellsManager.OnScroll;
        _monitorPlayerInput.OnMouseScroll += _moveMainPanel.OnScroll;
        base.InitPanel();


        _closeMrk.ClickAct = OnCloseProc;
    }
    public void SetData(int _firstSelectIndex = 0)
    {
        GameDatasSingleton gameDatasSingleton = GameDatasSingleton.Instance;
        List<GameData> currentAllGameDatas = gameDatasSingleton.AllGameDatas;
        _libraryCellsManager.GenerateBoxs(currentAllGameDatas, _firstSelectIndex);
    }

    protected override void OnCloseProc()
    {
        //マウススクロール割り当てを外す
        _monitorPlayerInput.OnMouseScroll -= _libraryCellsManager.OnScroll;
        _monitorPlayerInput.OnMouseScroll -= _moveMainPanel.OnScroll;
        base.OnCloseProc();
    }
}
