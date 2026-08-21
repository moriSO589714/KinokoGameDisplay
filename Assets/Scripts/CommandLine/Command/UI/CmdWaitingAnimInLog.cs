using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CmdWaitingAnimInLog
{
    int _limitDot = 3;
    float _waitSeconds = 0.4f; 

    public async UniTask LoopWaitingLog(string originMessage, OutPutTextLogColorSets outPutTextLogColorSets, string messageCode, CancellationToken token)
    {
        CmdSceneManager cmdSceneManager = CmdSceneManager.Instance;
        int dotNum = 1;
        string currentTxt = originMessage;
        while (!token.IsCancellationRequested)
        {
            if(dotNum <= _limitDot)
            {
                dotNum++;
                currentTxt += ".";
            }
            else
            {
                dotNum = 0;
                currentTxt = originMessage;
            }

            cmdSceneManager.OutPutManager.ReceiveMessage(currentTxt, outPutTextLogColorSets, specifiedUUID:messageCode);
            await UniTask.WaitForSeconds(_waitSeconds, cancellationToken: token);
        }
    }
}