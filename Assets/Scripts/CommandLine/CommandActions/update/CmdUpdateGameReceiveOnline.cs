using System.Collections.Generic;

public class CmdUpdateGameReceiveOnline : CmdDownloadGame
{
    protected override string _thisMode => "アップデートダウンロード";

    protected override void DoFiltering()
    {
        _cmdSceneManager.OutPutManager.SendLogMessage("新バージョンをダウンロードするゲームのフィルタリング情報を送信してください", OutPutTextLogColorSets.SystemDefault);
        //新しいバージョンが存在するものだけをフィルタリングに含めるように調整
        _filtering.FixStatusList(new List<GameStatus>() { GameStatus.UpdateAvailable });
        //実行
        _filtering.WaitSendCategory();
    }

    //ダウンロードの実行
    protected override void DoDownload()
    {
        DownloadNewVersionGame downloadNewVersionGame = new DownloadNewVersionGame();
        List<GameDlTask> gameDlTasks = new List<GameDlTask>();

        foreach(GameData gameData in _candidateGameDatas)
        {
            GameDlTask dlTask = downloadNewVersionGame.CreateGameDlTaskAndAddCue(gameData, _gameDlCue);
            gameDlTasks.Add(dlTask);
        }

        _watchingGameDlCue.ChangeTaskEmptyAct = () => EndDownload(_watchingGameDlCue);
        LoggerDuaringDownload();
    }

    /// <summary>
    /// 予定されたダウンロードが全て終了した際に呼ばれる
    /// </summary>
    protected override void EndDownload(WatchingGameDlCueForUI watchingGameDlCueForUI)
    {
        _cmdSceneManager.OutPutManager.SendLogMessage("全てのアップデートがダウンロードできました", OutPutTextLogColorSets.SystemDefault);

        //エラーが発生したタスクが無いかの確認
        List<GameDlError> errorTasks = watchingGameDlCueForUI.CheckErrorTasks();
        if(errorTasks != null && errorTasks.Count > 0)
        {
            //エラーが発生していた場合
            HappenError(errorTasks);
            return;
        }

        ReturnCmdReceiveMode();
    }
}