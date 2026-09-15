using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CmdActForUseNetwork : CmdAct
{
    protected CancellationTokenSource _ctsForLoadSpreadSheet;

    protected virtual async UniTask PrepareUsingNetwork()
    {
        //スプシのロード中にコマンドの受付を行わないようにしておく
        _cmdSceneManager.InputFieldManager.ChangeAction(new CmdNothing().MessageGird);
        _ctsForLoadSpreadSheet = new CancellationTokenSource();
        //exitコマンドなどが入力された場合はunitaskの処理をキャンセルさせる
        _cmdSceneManager.InputFieldManager._endModeAction += () => { _ctsForLoadSpreadSheet.Cancel(); };

        await LoadSpreadSheetData();
    }

    protected virtual async UniTask LoadSpreadSheetData()
    {
        GameDataManager gameDataManager = new GameDataManager();

        string connectInternetLog = "インターネットに接続して、現在登録されているゲーム情報を取得しています";
        string messageId = _cmdSceneManager.OutPutManager.ReceiveMessage(connectInternetLog, OutPutTextLogColorSets.SystemDefault);
        
        CancellationTokenSource ctsForLogAnim = new CancellationTokenSource();
        new CmdWaitingAnimInLog().LoopWaitingLog(connectInternetLog, OutPutTextLogColorSets.SystemDefault, messageId, ctsForLogAnim.Token);

        try
        {
            await UniTask.RunOnThreadPool(gameDataManager.LoadGameDataFromSpSt);
        }
        catch (Exception e)
        {
            ctsForLogAnim.Cancel();
            if (_ctsForLoadSpreadSheet.IsCancellationRequested)
            {
                return;
            }
            throw e;
        }
        ctsForLogAnim.Cancel();

        //処理にキャンセルが入っていた場合
        if (_ctsForLoadSpreadSheet.IsCancellationRequested)
        {
            return;
        }

        _cmdSceneManager.OutPutManager.ReceiveMessage("接続成功。初期処理を実行中", OutPutTextLogColorSets.SystemDefault);
    }
}
