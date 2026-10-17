using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibraryCellsManager : BoxManager
{
    [SerializeField] GameObject _libraryCellPref;
    [SerializeField] GameLibraryMainPanelManager _mainPanelManager;   
    [SerializeField] RayPassHitPanel _hitPanel;

    private LibraryCell _currentSelectedCell;

    protected override void Awake()
    {
        base.Awake();


    }

    public override void OnScroll(float scrollDirection)
    {
        if (!_hitPanel._isPointing) return;
        base.OnScroll(scrollDirection);
    }

    /// <summary>
    /// 既存に生成しているボックスを消して新たに生成する
    /// </summary>
    public void GenerateBoxs(List<GameData> gameDataList, int firstActivateIndex)
    {
        RectTransform thisRect = this.GetComponent<RectTransform>();
        foreach (GameData gameData in gameDataList)
        {
            GameObject instancedBox = InstanceBox(gameData, _lastBoxYPos, _libraryCellPref);
            LibraryCell instancedCell = instancedBox.GetComponent<LibraryCell>();
            instancedCell._activateAct = OnActiveCell;
            _lastBoxYPos = instancedBox.GetComponent<RectTransform>().anchoredPosition.y;
        }

        LibraryCell firstActivateCell = _boxPool[firstActivateIndex] as LibraryCell;
        firstActivateCell.ActivateThisCell();
    }

    private void OnActiveCell(LibraryCell targetCell)
    {
        //1つ前にアクティブになっていたセルの終了処理
        _currentSelectedCell?.InactiveThisCell();

        _currentSelectedCell = targetCell;
        //メインパネルに選択されたセルのGameDataを渡す
        _mainPanelManager.SetData(targetCell._myGameData);
    }
}