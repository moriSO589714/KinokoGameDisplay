using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CommandNode : MonoBehaviour
{
    public List<CommandNode> ChildrenNodes = new List<CommandNode>();

    public UnityEvent OnAction;

    private void Awake()
    {
        CheckGameObjectName();
    }

    private void CheckGameObjectName()
    {
        string gameObjectName = gameObject.name;
        if(gameObjectName.Contains(" ") || gameObjectName.Contains("　"))
        {
            throw new Exception("CommandNodeがアタッチされたゲームオブジェクトに半角/全角スペースの入った名前は設定できません");
        }
    }
}
