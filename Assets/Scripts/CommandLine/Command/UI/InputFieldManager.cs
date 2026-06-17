using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// コマンドラインのインプットマネージャーから入力が来た際に入力を送る
/// </summary>
public class InputFieldManager : MonoBehaviour
{
    [SerializeField]private InputField _myInputField;
    private Action<string> _throwMessageMethod;

    [SerializeField] private string _resetWord;
    //コマンド受信に切り替えるメソッド
    public Action _setCommandReceiver;
    //現在登録されているアクションが切り替えられる際に実行されるアクション
    public Action _whenEndCurrentAction;

    public void ChangeAction(Action<string> newAct)
    {
        _whenEndCurrentAction?.Invoke();
        _whenEndCurrentAction = null;
        _throwMessageMethod = newAct;
    }

    public void OnTryAction()
    {
        string inputFieldTxt = _myInputField.text;

        //強制終了時用(強制的にデフォルトに戻る)
        if(inputFieldTxt == _resetWord)
        {
            _setCommandReceiver?.Invoke();
        }
        else//現在の受信メソッドへ入力内容を送る
        {
            _throwMessageMethod?.Invoke(_myInputField.text);
        }

        ClearInputField();
    }

    private void ClearInputField()
    {
        _myInputField.text = "";
    }
}