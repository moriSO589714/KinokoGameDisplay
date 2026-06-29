using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameBoxsManager : BoxManager
{
    [SerializeField] GameObject _gameBoxPref;

    [SerializeField] UIPanel _checkDlPanel;
    [SerializeField] UIPanel _checkUpdataPanel;
    [SerializeField] GameDlCue _gameDlCue;

    [SerializeField] WatchingGameDlCueForUI _watchingGameDlCueForUI;

    private GameBoxButtonClick _gameBoxButtonClick;

    protected override void Awake()
    {
        base.Awake();
        _gameBoxButtonClick = new GameBoxButtonClick(_checkDlPanel, _checkUpdataPanel, _gameDlCue);
    }

    /// <summary>
    /// 既存に生成しているボックスを消して新しく生成
    /// </summary>
    public void GenerateBoxs(List<GameData> gameDataList)
    {
        ClearField();

        foreach(GameData gameData in gameDataList)
        {
            GameObject instancedBox = InstanceBox(gameData, _lastBoxYPos, _gameBoxPref);
            _lastBoxYPos = instancedBox.GetComponent<RectTransform>().anchoredPosition.y;
            instancedBox.GetComponent<GameBox>().SetClickButtonAct(_gameBoxButtonClick.OnClickAction);
        }
    }
}
