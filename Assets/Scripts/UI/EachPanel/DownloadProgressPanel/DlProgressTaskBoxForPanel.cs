using System;
using UnityEngine;

public class DlProgressTaskBoxForPanel : DlProgressTaskBox
{

    [SerializeField] private UIActBase _downArrowButton;
    [SerializeField] private UIActBase _cancellTaskButton;

    public void SetButtonAct(Action<string> deleteTaskAct, Action transferTaskAct)
    {
        _downArrowButton.ClickAct += transferTaskAct;
        _cancellTaskButton.ClickAct += () => deleteTaskAct(_currentTask.TaskName);
    }
}
