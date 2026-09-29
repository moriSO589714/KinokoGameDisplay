using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdAddGameByLocal : CmdAct
{
    protected override void Init()
    {
        base.Init();
    }

    public override void FirstCall()
    {
        base.FirstCall();
        _cmdSceneManager.OutPutManager.SendMessage("ローカルアップロードモードに変更します", OutPutTextLogColorSets.SystemDefault);
        CmdGenericInfoInputOfAddGame cmdGenericInfoInputOfAddGame = new CmdGenericInfoInputOfAddGame(ReceiveInputedGameData);
        CmdReturn returnCmdReceiveMode = new CmdReturn(ReturnCmdReceiveMode);
        cmdGenericInfoInputOfAddGame.StartInputData(returnCmdReceiveMode);
    }

    private void ReceiveInputedGameData(GameData addGameData, string localGamePath, string localImagePath)
    {
        _cmdSceneManager.InputFieldManager.ChangeAction(new CmdNothing().MessageGird);
        _cmdSceneManager.OutPutManager.SendMessage("ゲームフォルダのコピー中です", OutPutTextLogColorSets.SystemDefault);
        AddNewGameByLocal addNewGameByLocal = new AddNewGameByLocal();
        try
        {
            addNewGameByLocal.AddNewGame(addGameData, localGamePath, localImagePath);
        }
        catch (Exception e)
        {
            _cmdSceneManager.OutPutManager.SendMessage(e.ToString(), OutPutTextLogColorSets.AccentDefault);
            throw e;
        }

        _cmdSceneManager.OutPutManager.SendMessage("アップロードが終了しました。コマンド受付モードに移行します", OutPutTextLogColorSets.SystemDefault);
        ReturnCmdReceiveMode();
    }
}