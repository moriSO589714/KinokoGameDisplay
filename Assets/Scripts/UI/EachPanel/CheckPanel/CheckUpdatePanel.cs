using System;
using UnityEngine;

public class CheckUpdatePanel : UIPanel
{
    [SerializeField] private UIActBase _yesButton;
    [SerializeField] private UIActBase _noButton;
    [SerializeField] private UIActBase _closeMrk;

    private GameData _updateGameData = null;
    private GameDlCue _gameDlCue = null;
    private GameBox _gameBox = null;
    private Action _startGameAct;

    public override void InitPanel()
    {
        base.InitPanel();

        //デリゲートの設定
        _closeMrk.ClickAct = OnCloseProc;
        _yesButton.ClickAct = AddGameUpdateTask;
        _noButton.ClickAct = _startGameAct;
    }

    public void SetGameData(GameData gameData, GameDlCue gameDlCue, GameBox gameBox, Action startGameAct)
    {
        _updateGameData = gameData;
        _gameDlCue = gameDlCue;
        _gameBox = gameBox;
        _startGameAct = startGameAct;
    }

    private void AddGameUpdateTask()
    {
        //タスク作成とキュー追加
        GameDlTask dlTask = new DownloadNewVersionGame().CreateGameDlTaskAndAddCue(_updateGameData, _gameDlCue);
        //スタートボタンのスプライト差し替え
        _gameBox.ChangeButtonImage();
        OnCloseProc();
    }

    protected override void OnCloseProc()
    {
        stateManager.SetCurrentLoad(LoadStates.NoLoading);
        this.gameObject.SetActive(false);
    }
}
