using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitAndErrorBoxsManager : BoxManager
{
    [SerializeField] private GameObject _waitBoxPref;
    [SerializeField] private GameObject _errorBoxPref;

    protected override void Awake()
    {
        base.Awake();
    }

    public void GenerateTaskBoxs(List<GameDlTask> gameDlTasksList, Action<string> immediatelyDlAct, Action<string, int> transferWaitBoxAct, Action<string> deleteTask)
    {
        foreach(GameDlTask gameDlTask in gameDlTasksList)
        {
            GameObject instancedBox = InstanceBox(gameDlTask, _lastBoxYPos, _waitBoxPref);
            _lastBoxYPos = instancedBox.GetComponent<RectTransform>().anchoredPosition.y;
            instancedBox.GetComponent<WaitBox>().SetButtonActs(immediatelyDlAct, transferWaitBoxAct, deleteTask);
        }
    }

    /// <summary>
    /// エラーボックスの生成。こちらはフィールドを消さず追加で生成する
    /// </summary>
    public void GenerateErrorBoxs(List<GameDlError> gameDlErrorList, Action<string> recoveryAct, Action<string> errorDeleteAct)
    {
        foreach(GameDlError gameDlError in gameDlErrorList)
        {
            GameObject instancedBox = InstanceBox(gameDlError, _lastBoxYPos, _errorBoxPref);
            _lastBoxYPos = instancedBox.GetComponent<RectTransform>().anchoredPosition.y;
            instancedBox.GetComponent<ErrorBox>().SetButtonActs(recoveryAct, errorDeleteAct);
        }
    }
}