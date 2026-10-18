using UnityEngine;

public class CheckDownloadPanel : UIPanel
{
    [SerializeField] private UIActBase _yesButton;
    [SerializeField] private UIActBase _noButton;

    private GameData _dlGameData = null;
    private GameDlCue _gameDlCue = null;
    private WatchingGameDlCueForUI _wathingGameDlCue = null;
    private GameStartButton _currentGameStartButton;

    public override void InitPanel()
    {
        base.InitPanel();

        //デリゲートの設定
        _noButton.ClickAct = OnCloseProc;
        _yesButton.ClickAct = AddGameDlTask;
    }

    //ダウンロードを実行するゲームのGameDataクラスをセットする
    public void SetGameData(GameData gameData, GameDlCue gameDlCue, WatchingGameDlCueForUI watchingGameDlCueForUI, GameStartButton gameStartButton)
    {
        _dlGameData = gameData;
        _gameDlCue = gameDlCue;
        _wathingGameDlCue = watchingGameDlCueForUI;
        _currentGameStartButton = gameStartButton;
    }

    private void AddGameDlTask()
    {
        //ダウンロード用のタスクを作成してキューに追加
        GameDlTask dlTask = new DownloadGame().CreateGameDlTaskAndAddCue(_dlGameData, _gameDlCue);
        GameStartButton localGameStartButton = _currentGameStartButton;
        //ダウンロード終了時にボタンのスプライトを更新するようにしておく
        _wathingGameDlCue.EndProgressTaskSendTaskName += (string endTaskName) => ChangeSpriteWhenEndDownload(dlTask, endTaskName, localGameStartButton);
        //ダウンロード中であることが分かるようにゲームボックスのスプライトを差し替える
        _currentGameStartButton.ChangeButtonImage();
        OnCloseProc();
    }

    private void ChangeSpriteWhenEndDownload(GameDlTask targetTask, string endTaskName, GameStartButton targetGameStartButton)
    {
        //終了したタスクが引数のIDを持つゲームのタスクか
        if(targetTask.TaskName == endTaskName)
        {
            //タスクと事前に渡されているスタートボタンのゲームが一致しているか？(ゲームライブラリパネルなどはインスタンスは同じで扱うゲームが変わる)
            if (targetTask.TaskInstance.GameData.GameID == targetGameStartButton.MyGameData.GameID)
            {
                targetGameStartButton.ChangeButtonImage();
            }
        }
    }
}