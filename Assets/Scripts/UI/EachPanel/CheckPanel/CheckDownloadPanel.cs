using UnityEngine;

public class CheckDownloadPanel : UIPanel
{
    [SerializeField] private UIActBase _yesButton;
    [SerializeField] private UIActBase _noButton;

    private GameData _dlGameData = null;
    private GameDlCue _gameDlCue = null;
    private WatchingGameDlCueForUI _wathingGameDlCue = null;
    private GameBox _gameBox = null;

    public override void InitPanel()
    {
        base.InitPanel();

        //デリゲートの設定
        _noButton.ClickAct = OnCloseProc;
        _yesButton.ClickAct = AddGameDlTask;
    }

    //ダウンロードを実行するゲームのGameDataクラスをセットする
    public void SetGameData(GameData gameData, GameDlCue gameDlCue, WatchingGameDlCueForUI watchingGameDlCueForUI, GameBox gameBox)
    {
        _dlGameData = gameData;
        _gameDlCue = gameDlCue;
        _wathingGameDlCue = watchingGameDlCueForUI;
        _gameBox = gameBox;
    }

    private void AddGameDlTask()
    {
        //ダウンロード用のタスクを作成してキューに追加
        GameDlTask dlTask = new DownloadGame().CreateGameDlTaskAndAddCue(_dlGameData, _gameDlCue);
        GameBox localGameBox = _gameBox;
        //ダウンロード終了時にボタンのスプライトを更新するようにしておく
        _wathingGameDlCue.EndProgressTaskSendTaskName += (string endTaskName) => ChangeSpriteWhenEndDownload(dlTask.TaskName, endTaskName, localGameBox);
        //ダウンロード中であることが分かるようにゲームボックスのスプライトを差し替える
        _gameBox.ChangeButtonImage();
        OnCloseProc();
    }

    protected override void OnCloseProc()
    {
        stateManager.SetCurrentLoad(LoadStates.NoLoading);
        this.gameObject.SetActive(false);
    }

    private void ChangeSpriteWhenEndDownload(string taskName, string endTaskName, GameBox targetGameBox)
    {
        //終了したタスクが引数のIDを持つゲームのタスクか
        if(taskName == endTaskName)
        {
            targetGameBox.ChangeButtonImage();
        }
    }
}