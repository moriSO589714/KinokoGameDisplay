using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// InputFieldでのEnter入力を状況に応じて発火させるメソッドを変化させる
/// </summary>
public class FreeInputEnterController
{
    private Action _registerLabelFieldAct;
    private Action _reflectCandidateValue;

    private CandidateBoxManager _candidateBoxManager;

    public FreeInputEnterController(Action registerLabelFieldAct, Action reflectCandidateValue, CandidateBoxManager candidateBoxManager)
    {
        _registerLabelFieldAct = registerLabelFieldAct;
        _reflectCandidateValue = reflectCandidateValue;
        _candidateBoxManager = candidateBoxManager;
    }

    /// <summary>
    /// InputFieldコンポーネント側がsubmitを検知した場合に発火させるメソッド
    /// FreeInputManager側でInputFieldに登録する
    /// </summary>
    public void WhenSubmitInputField()
    {
        int currentSelectBox = _candidateBoxManager._currentSelectBoxIndex;
        if(currentSelectBox == -1)
        {
            _registerLabelFieldAct?.Invoke();
        }
        else
        {
            _reflectCandidateValue?.Invoke();
        }
    }
}
