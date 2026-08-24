using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class GameUpProgress
{
    private string _currentProgress = "";

    //現在の進捗状況が変化した際に呼ばれる
    Action<string> _onChangeProgressAct;

    public void ChangeState(string progress)
    {
        _currentProgress = progress;
        _onChangeProgressAct?.Invoke(progress);
    }
}
