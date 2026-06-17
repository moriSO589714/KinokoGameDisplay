
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

    public GameBoxButtonClick(UIPanel checkDlPanel, UIPanel checkUpdatePanel, GameDlCue gameDlCue)
    {
        _checkDownloadPanel = checkDlPanel;
        _checkUpdatePanel = checkUpdatePanel;
        _gameDlCue = gameDlCue;
    }

    /// <summary>
    /// ボタンが押された際の処理
    /// </summary>
    public void OnClickAction(GameBox targetGameBox)
    {
        GameData targetGameData = targetGameBox._thisGameData;
        //ゲームの状態ごとに処理を分ける
        if(targetGameData.Status == GameStatus.Downloaded || targetGameData.Status == GameStatus.ByLocal)
        {
            //ゲームを実行
            new PlayingGame().OnPlayGame(targetGameData);
        }
        else if(targetGameData.Status == GameStatus.NotDownloaded)
        {
            //ダウンロードを確認するパネルを有効化。残りの処理はそっちに任せる
            _checkDownloadPanel.gameObject.SetActive(true);
            _checkDownloadPanel.GetComponent<CheckDownloadPanel>().SetGameData(targetGameData, _gameDlCue, targetGameBox);
        }
        else if(targetGameData.Status == GameStatus.UpdateAvailable)
        {
            //アップデートするか確認するパネルを有効化。残りの処理はそっちに任せる
            _checkUpdatePanel.gameObject.SetActive(true);
        }

    }


}
