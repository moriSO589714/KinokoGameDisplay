using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CmdDelete : CmdAct
{
    protected WordEmtCell _gameIdLib;

    protected virtual void CmdDeleteEntrance()
    {
        CmdReturn cmdReturn = new CmdReturn(ReturnCmdReceiveMode);
        _cmdSceneManager.InputFieldManager.ChangeAction((string message) => ReceiveGameId(message, cmdReturn), _gameIdLib);
        _cmdSceneManager.OutPutManager.ReceiveMessage("削除したいゲームのゲームIDを送信してください", OutPutTextLogColorSets.SystemDefault);
    }

    protected virtual void ReceiveGameId(string message, CmdReturn cmdReturn)
    {
        if (cmdReturn.ReturnCheck(message)) return;

        //送信されたゲームidを持つGameDataクラスを取得してくる
        GameDatasSingleton gameDatasSingleton = GameDatasSingleton.Instance;
        List<GameData> allGameDataList = new List<GameData>(gameDatasSingleton.AllGameDatas);
        List<GameData> matchGameData = allGameDataList.Where(x => x.GameID == message).ToList();

        if (matchGameData.Count < 1)
        {
            _cmdSceneManager.OutPutManager.ReceiveMessage("合致するIDを持つゲームが存在しません", OutPutTextLogColorSets.AccentDefault);
            CmdDeleteEntrance();
            return;
        }

        if (matchGameData.Count > 1)
        {
            _cmdSceneManager.OutPutManager.ReceiveMessage("合致するIDを持つゲームが複数存在します", OutPutTextLogColorSets.AccentDefault);
            CmdDeleteEntrance();
            return;
        }

        GameData targetGameData = matchGameData[0];

        if (!CheckGameStatus(targetGameData.Status))
        {
            _cmdSceneManager.OutPutManager.ReceiveMessage($"対象のゲームはこのモードでは削除できません。対象ゲームのステータス{targetGameData.Status}>>>", OutPutTextLogColorSets.AccentDefault);
            CmdDeleteEntrance();
            return;
        }

        CheckDelete(targetGameData);
    }

    protected virtual bool CheckGameStatus(GameStatus status)
    {
        return true;
    }

    protected virtual void CheckDelete(GameData deleteTarget)
    {
        string gameTitle = "\n・ゲームタイトル：";
        string gameId = "\n・ゲームID：";
        string gameVersion = "\n・ゲームのバージョン(最終更新日)：";
        string gameDevelopper = "\n・ゲーム開発者名：";

        if (deleteTarget.GameTitle != null) gameTitle += deleteTarget.GameTitle;
        if(deleteTarget.GameID != null) gameId += deleteTarget.GameID;
        if(deleteTarget.GameVersion != null) gameVersion += deleteTarget.GameVersion;
        if (deleteTarget.GameDevelopper != null && deleteTarget.GameDevelopper.Count() != 0) gameDevelopper += string.Join(",", gameDevelopper);

        string checkLog = $"削除するゲームの確認を行ってください。この操作は取り消せません、誤ったゲームを指定していないか確認してください\n" +
            $"削除を行う場合はゲームのタイトル名を送信してください\n【ゲーム情報】" + gameTitle + gameId + gameVersion + gameDevelopper;

        CmdReturn cmdReturn = new CmdReturn(() => CheckDelete(deleteTarget));

        _cmdSceneManager.OutPutManager.ReceiveMessage(checkLog, OutPutTextLogColorSets.SystemDefault);
        _cmdSceneManager.InputFieldManager.ChangeAction((string val) => ReceiveTitle(val, deleteTarget, cmdReturn));
    }

    protected virtual void ReceiveTitle(string message, GameData targetGameData, CmdReturn cmdReturn)
    {
        if (cmdReturn.ReturnCheck(message)) return;

        //タイトルに一致するか確認する
        if (message == targetGameData.GameTitle)
        {
            DoDelete(targetGameData);
        }
        else
        {
            _cmdSceneManager.OutPutManager.ReceiveMessage("タイトルと一致していません", OutPutTextLogColorSets.AccentDefault);
            CmdDeleteEntrance();
            return;
        }
    }

    protected virtual async UniTask DoDelete(GameData targetGameData)
    {

    }
}
