using System;
using UnityEngine;
using UnityEngine.UI;

public class WaitBox : Box
{
    [SerializeField] Text _gameNameTxt;
    [SerializeField] UIActBase _upArrowButton;
    [SerializeField] UIActBase _downArrowButton;
    [SerializeField] UIActBase _immediatelyDlButton;
    [SerializeField] UIActBase _deleteTask;

    public GameDlTask _myGameDlTask { get; private set; }

    public override void SetDataMyBox<T>(T originData)
    {
        base.SetDataMyBox(originData);

        //型のキャスト
        GameDlTask thisGameDlTask = originData as GameDlTask;
        _myGameDlTask = thisGameDlTask;
        SetGameName(_myGameDlTask.TaskInstance.GameData.GameTitle);
    }

    public void SetButtonActs(Action<string> immediatelyDlAct, Action<string, int> transferWaitBoxAct, Action<string> deleteTask)
    {
        _immediatelyDlButton.ClickAct += () => { immediatelyDlAct(_myGameDlTask.TaskName); };
        _upArrowButton.ClickAct += () => { transferWaitBoxAct(_myGameDlTask.TaskName, -1); };
        _downArrowButton.ClickAct += () => { transferWaitBoxAct(_myGameDlTask.TaskName, 1); };
        _deleteTask.ClickAct += () => { deleteTask(_myGameDlTask.TaskName); };
    }

    protected void SetGameName(string gameName)
    {
        _gameNameTxt.text = gameName;
    }
}
