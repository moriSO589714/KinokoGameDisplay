using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdGameInfoInputerForUpdate : CmdGameInfoInputer
{
    protected readonly new string _decisionWord = "update";

    public CmdGameInfoInputerForUpdate(Action<GameData, string, string> sendGameDataAct, GameData originGameData): base(sendGameDataAct)
    {
        _currentGameData = originGameData;
    }

    public CmdGameInfoInputerForUpdate() { }

    public override void StartInputData(CmdReturn returnCmdReceive)
    {
        base.StartInputData(returnCmdReceive);
        _allowDecision = true;

        _sceneManager.OutPutManager.SendLogMessage($"※決定してアップデートをする場合は「{_decisionWord}」を送信してください", OutPutTextLogColorSets.Blue);
    }

    protected override bool CheckDecision(string message)
    {
        if(message == _decisionWord && _allowDecision)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
