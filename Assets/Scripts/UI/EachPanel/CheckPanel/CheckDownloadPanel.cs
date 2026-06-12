using UnityEngine;

public class CheckDownloadPanel : UIPanel
{
    [SerializeField] private UIActBase _yesButton;
    [SerializeField] private UIActBase _noButton;

    private GameData _dlGameData = null;
    private GameDlCue _gameDlCue = null;

    public override void InitPanel()
    {
        base.InitPanel();

        //デリゲートの設定
        _noButton.ClickAct = OnCloseProc;
        _yesButton.ClickAct = AddGameDlTask;
    }

    //ダウンロードを実行するゲームのGameDataクラスをセットする
    public void SetGameData(GameData gameData, GameDlCue gameDlCue)
    {
        _dlGameData = gameData;
        _gameDlCue = gameDlCue;
    }

    public void AddGameDlTask()
    {
        //ダウンロード用のタスクを作成してキューに追加
        GameDlTask dlTask = new DownloadGame().CreateGameDlTaskAndAddCue(_dlGameData, _gameDlCue);
        OnCloseProc();
    }

    protected override void OnCloseProc()
    {
        stateManager.SetCurrentLoad(LoadStates.NoLoading);
        this.gameObject.SetActive(false);
    }


}
