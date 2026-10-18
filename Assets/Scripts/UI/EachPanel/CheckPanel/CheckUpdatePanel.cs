using System;
using UnityEngine;

public class CheckUpdatePanel : UIPanel
{
    [SerializeField] private UIActBase _yesButton;
    [SerializeField] private UIActBase _noButton;
    [SerializeField] private UIActBase _closeMrk;

    private GameData _updateGameData = null;
    private GameDlCue _gameDlCue = null;
    private WatchingGameDlCueForUI _watchingGameDlCueForUI = null;
    private GameStartButton _currentGameStartuButton;
    private Action _startGameAct;

    public override void InitPanel()
    {
        base.InitPanel();

        //デリゲートの設定
        _closeMrk.ClickAct = OnCloseProc;
        _yesButton.ClickAct = AddGameUpdateTask;
        _noButton.ClickAct = _startGameAct;
    }

    public void SetGameData(GameData gameData, GameDlCue gameDlCue, WatchingGameDlCueForUI watchingGameDlCueForUI, GameStartButton gameStartButton, Action startGameAct)
    {
        _updateGameData = gameData;
        _gameDlCue = gameDlCue;
        _watchingGameDlCueForUI = watchingGameDlCueForUI;
        _currentGameStartuButton = gameStartButton;
        _startGameAct = startGameAct;
    }

    private void AddGameUpdateTask()
    {
        //タスク作成とキュー追加
        GameDlTask dlTask = new DownloadNewVersionGame().CreateGameDlTaskAndAddCue(_updateGameData, _gameDlCue);
        GameStartButton localGameStartButton = _currentGameStartuButton;
        _watchingGameDlCueForUI.EndProgressTaskSendTaskName += (string endTaskName) => UpdateStartSpriteWhenEndUpdateTask(dlTask, endTaskName, localGameStartButton);
        //スタートボタンのスプライト差し替え
        _currentGameStartuButton.ChangeButtonImage();
        OnCloseProc();
    }

    //スタートボタンのスプライト更新用、WathingGameDlCueForUIのEndProgressTaskSendTaskNameに突っ込む
    private void UpdateStartSpriteWhenEndUpdateTask(GameDlTask targetTask, string endTaskName, GameStartButton gameStartButton)
    {
        //終了したタスク名が引数のタスクと同じか
        if(targetTask.TaskName == endTaskName)
        {
            if(targetTask.TaskInstance.GameData.GameID == gameStartButton.MyGameData.GameID)
            {
                gameStartButton.ChangeButtonImage();
            }
        }
    }
}