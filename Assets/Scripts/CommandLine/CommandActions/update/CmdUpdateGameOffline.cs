using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public class CmdUpdateGameOffline : CmdAct
{
    protected override string _thisMode => "オフラインアップデート";

    List<GameData> _allGameDatas = new List<GameData>();
    WordEmtCell _idLib;
    CancellationTokenSource _ctsForUpdate;

    public override void FirstCall()
    {
        base.FirstCall();

        GameDatasSingleton gameDatasSingleton = GameDatasSingleton.Instance;
        _allGameDatas = gameDatasSingleton.AllGameDatas;
        _idLib = CreateLibFromGameDatas.CreateGameIdLib(_allGameDatas);        
        _cmdSceneManager.InputFieldManager.ChangeAction(SelectGame);
        _cmdSceneManager.OutPutManager.SendLogMessage("アップデートするゲームのIDを送信してください", OutPutTextLogColorSets.SystemDefault);
        _ctsForUpdate = new CancellationTokenSource();
    }

    private void SelectGame(string message)
    {
        List<GameData> matchGameDatas = new List<GameData>();
        matchGameDatas = _allGameDatas.Where(x => x.GameID == message).ToList();

        GameData matchGameData = new GameData();
        if(matchGameDatas.Count == -1)
        {
            _cmdSceneManager.OutPutManager.SendLogMessage("送信されたIDのゲームは存在しません。IDを送信し直してください", OutPutTextLogColorSets.AccentDefault);
            return;
        }
        else if(matchGameDatas.Count > 1)
        {
            _cmdSceneManager.OutPutManager.SendLogMessage("該当するIDのゲームが複数存在します。IDを送信し直してください", OutPutTextLogColorSets.AccentDefault);
            return;
        }
        else if(matchGameDatas.Count == 1)
        {
            matchGameData = matchGameDatas[0];
        }

        //ゲームがローカルから追加されたかを確認する
        if(matchGameData.Status != GameStatus.ByLocal)
        {
            _cmdSceneManager.OutPutManager.SendMessage("送信されたゲームはオンラインで追加されているため、このモードからはアップデートできません。IDを送信し直してください", OutPutTextLogColorSets.AccentDefault);
            return;
        }

        StartDataInput(new GameData(matchGameData), matchGameData);
    }

    private void StartDataInput(GameData newGameData, GameData originGameData)
    {
        CmdGameInfoInputerForUpdate cmdGameInfoInputerForUpdate = new CmdGameInfoInputerForUpdate
            ((GameData newdata, string gamePath, string imagePath) => ReceiveInputedGameData(newdata, gamePath, imagePath, originGameData), originGameData);
        CmdReturn cmdReturn = new CmdReturn(ReturnCmdReceiveMode);
        cmdGameInfoInputerForUpdate.StartInputData(cmdReturn);
    }

    private void ReceiveInputedGameData(GameData updatedGameData, string updateLocalGamePath, string updateLocalImagePath, GameData originGameData)
    {
        //ユーザー入力のものから文字の置き換えなど最適化を行う
        GameData useGameData = GameDataForUpload.CreateGameDataForUpload(updatedGameData, updateLocalGamePath, updateLocalImagePath);
        UpdateGame(useGameData);
    }

    private async UniTask UpdateGame(GameData updateGameData)
    {
        string logId = _cmdSceneManager.OutPutManager.SendLogMessage("アップデートを開始します", OutPutTextLogColorSets.SystemDefault);

        UpdateGameByLocal updateGameByLocal = new UpdateGameByLocal();
        GameUpProgress gameUpProgress = new GameUpProgress();
        gameUpProgress.OnChangeProgressAct = ((string message) => _cmdSceneManager.OutPutManager.SendLogMessage(message, OutPutTextLogColorSets.SystemDefault, specifiedUUID: logId));

        await updateGameByLocal.UpdateGameInUniTask(_ctsForUpdate.Token, updateGameData, gameUpProgress);
        _cmdSceneManager.OutPutManager.SendLogMessage("アップデートが終了しました", OutPutTextLogColorSets.SystemDefault);

        ReturnCmdReceiveMode();
    }

    protected override void ReturnCmdReceiveMode()
    {
        _ctsForUpdate?.Cancel();
        base.ReturnCmdReceiveMode();
    }
}