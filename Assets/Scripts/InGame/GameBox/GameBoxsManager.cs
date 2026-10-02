using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
    private GameBoxImageClick _gameBoxImageClick;
    [SerializeField] private GameObject _loadingAnimPref;

    private CancellationTokenSource _ctsForImageDownload = new CancellationTokenSource();

    protected override void Awake()
    {
        base.Awake();
        _gameBoxButtonClick = new GameBoxButtonClick(_checkDlPanel, _checkUpdataPanel, _gameDlCue);
        _gameBoxImageClick = new GameBoxImageClick();
    }

    /// <summary>
    /// 既存に生成しているボックスを消して新しく生成
    /// </summary>
    public void GenerateBoxs(List<GameData> gameDataList)
    {
        ClearField();
        _ctsForImageDownload = new CancellationTokenSource();

        foreach(GameData gameData in gameDataList)
        {
            GameObject instancedBox = InstanceBox(gameData, _lastBoxYPos, _gameBoxPref);
            _lastBoxYPos = instancedBox.GetComponent<RectTransform>().anchoredPosition.y;
            GameBox instancedGameBox = instancedBox.GetComponent<GameBox>();
            instancedGameBox.SetClickButtonAct(_gameBoxButtonClick.OnClickAction);

            instancedGameBox.SetClickImageAct((gameBox) => _gameBoxImageClick.OnClickAction(gameBox, _ctsForImageDownload.Token));
        }
    }

    public override void ClearField()
    {
        base.ClearField();
        _ctsForImageDownload.Cancel();
    }
}