
using UnityEngine;

/// <summary>
/// ゲームボックスのスタートボタンが押されたときの挙動
/// GameBoxManagerにインスタンスが存在する
/// </summary>
public class GameBoxButtonClick
{
    private UIPanel _checkDownloadPanel;
    private UIPanel _checkUpdatePanel;
    private GameDlCue _gameDlCue;
    private WatchingGameDlCueForUI _wathingGameDlCue;

    public GameBoxButtonClick(UIPanel checkDlPanel, UIPanel checkUpdatePanel, GameDlCue gameDlCue, WatchingGameDlCueForUI watchingGameDlCueForUI)
    {
        _checkDownloadPanel = checkDlPanel;
        _checkUpdatePanel = checkUpdatePanel;
        _gameDlCue = gameDlCue;
        _wathingGameDlCue = watchingGameDlCueForUI;
    }
    
    /// <summary>
    /// ボタンが押された際の処理
    /// </summary>
    public void OnClickAction(GameBox targetGameBox)
    {
        GameData targetGameData = targetGameBox._myGameData;
        //ゲームの状態ごとに処理を分ける
        if (targetGameData.Status == GameStatus.Downloaded || targetGameData.Status == GameStatus.ByLocal)
        {
            //ゲームを実行
            new PlayingGame().OnPlayGame(targetGameData);
        }
        else if (targetGameData.Status == GameStatus.NotDownloaded)
        {
            //ダウンロードを確認するパネルを有効化。残りの処理はそっちに任せる
            _checkDownloadPanel.gameObject.SetActive(true);
            _checkDownloadPanel.GetComponent<CheckDownloadPanel>().SetGameData(targetGameData, _gameDlCue, _wathingGameDlCue, targetGameBox);
        }
        else if (targetGameData.Status == GameStatus.UpdateAvailable)
        {
            //アップデートするか確認するパネルを有効化。残りの処理はそっちに任せる
            _checkUpdatePanel.gameObject.SetActive(true);
            _checkUpdatePanel.GetComponent<CheckUpdatePanel>().SetGameData(targetGameData, _gameDlCue, _wathingGameDlCue, targetGameBox, () => { new PlayingGame().OnPlayGame(targetGameData); });
        }
    }
}
