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

    public void SetData(GameData firstSelectGameData = null)
    {
        GameDatasSingleton gameDatasSingleton = GameDatasSingleton.Instance;
        List<GameData> currentAllGameDatas = gameDatasSingleton.AllGameDatas;
        int firstSelectIndex = currentAllGameDatas.IndexOf(firstSelectGameData);
        if(firstSelectIndex == -1 || currentAllGameDatas[firstSelectIndex] == null)
        {
            firstSelectIndex = 0;
        } 
        _libraryCellsManager.GenerateBoxs(currentAllGameDatas, firstSelectIndex);
    }

    protected override void OnCloseProc()
    {
        //マウススクロール割り当てを外す
        _monitorPlayerInput.OnMouseScroll -= _libraryCellsManager.OnScroll;
        _monitorPlayerInput.OnMouseScroll -= _moveMainPanel.OnScroll;
        base.OnCloseProc();
    }
}
