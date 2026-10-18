using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GameBoxsManager : BoxManager
{
    [SerializeField] GameObject _gameBoxPref;

    [SerializeField] UIPanel _checkDlPanel;
    [SerializeField] UIPanel _checkUpdataPanel;
    [SerializeField] GameLibraryPanelManager _gameLibraryPanelManager;
    [SerializeField] GameDlCue _gameDlCue;

    [SerializeField] WatchingGameDlCueForUI _watchingGameDlCueForUI;

    private GameStartButtonClickSwitcher _gameBoxButtonClickSwitcher;
    [SerializeField] GameImageDownloaderForUI _gameImageDownLoaderForUI;

    private CancellationTokenSource _ctsForImageDownload = new CancellationTokenSource();

    protected override void Awake()
    {
        base.Awake();
        _gameBoxButtonClickSwitcher = new GameStartButtonClickSwitcher(_checkDlPanel, _checkUpdataPanel, _gameDlCue, _watchingGameDlCueForUI);
        _gameImageDownLoaderForUI = new GameImageDownloaderForUI();
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
            instancedGameBox.SetStartButtonAct(_gameBoxButtonClickSwitcher.OnClickAction);
            instancedGameBox.SetToLibraryButtonAct(ActivateGameLibraryPanel);
            instancedGameBox.SetImageDownloader(_gameImageDownLoaderForUI);
        }
    }

    public override void ClearField()
    {
        base.ClearField();
        _ctsForImageDownload.Cancel();
    }

    private void ActivateGameLibraryPanel(GameData clickedGameData)
    {
        _gameLibraryPanelManager.gameObject.SetActive(true);
        _gameLibraryPanelManager.SetData(clickedGameData);
    }
}