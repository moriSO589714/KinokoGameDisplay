using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class GameUpProgress
{
    private string _currentProgress = "";

    //現在の進捗状況が変化した際に呼ばれる
    public Action<string> OnChangeProgressAct;

    public void ChangeState(string progress)
    {
        _currentProgress = progress;

        //Unityオブジェクトに干渉するため以降のログ出し処理はメインスレッドで実行させる。
        SendMessageOnMainThread(progress).Forget();
    }

    private async UniTaskVoid SendMessageOnMainThread(string progress)
    {
        await UniTask.SwitchToMainThread();
        OnChangeProgressAct.Invoke(progress);
    }
}
