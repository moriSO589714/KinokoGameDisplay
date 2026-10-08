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
        _cmdSceneManager.OutPutManager.SendLogMessage("ローカルアップロードモードに変更します", OutPutTextLogColorSets.SystemDefault);
        CmdGameInfoInputerForAdd cmdGameInfoInputerForAdd = new CmdGameInfoInputerForAdd(ReceiveInputedGameData);
        CmdReturn returnCmdReceiveMode = new CmdReturn(ReturnCmdReceiveMode);
        cmdGameInfoInputerForAdd.StartInputData(returnCmdReceiveMode);
    }

    private void ReceiveInputedGameData(GameData addGameData, string localGamePath, string localImagePath)
    {
        _cmdSceneManager.InputFieldManager.ChangeAction(new CmdNothing().MessageGird);
        _cmdSceneManager.OutPutManager.SendLogMessage("ゲームフォルダのコピー中です", OutPutTextLogColorSets.SystemDefault);
        AddNewGameByLocal addNewGameByLocal = new AddNewGameByLocal();
        try
        {
            addNewGameByLocal.AddNewGame(addGameData, localGamePath, localImagePath);
        }
        catch (Exception e)
        {
            _cmdSceneManager.OutPutManager.SendLogMessage(e.ToString(), OutPutTextLogColorSets.AccentDefault);
            throw e;
        }

        _cmdSceneManager.OutPutManager.SendLogMessage("アップロードが終了しました。コマンド受付モードに移行します", OutPutTextLogColorSets.SystemDefault);
        ReturnCmdReceiveMode();
    }
}