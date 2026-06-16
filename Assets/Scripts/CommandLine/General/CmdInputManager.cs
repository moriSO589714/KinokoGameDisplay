using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// コマンドラインのインプットマネージャーから入力が来た際に入力を送る
/// </summary>
public class CmdInputManager : MonoBehaviour
{
    [SerializeField]private InputField _myInputField;
    private Action<string> _throwMessageMethod;

    public void ChangeAction(Action<string> newAct)
    {
        _throwMessageMethod = newAct;
    }

    public void OnTryAction()
    {
        _throwMessageMethod?.Invoke(_myInputField.text);
    }
}