using System;
using System.Collections.Generic;
using UnityEngine;

public class CommandNode : MonoBehaviour
{
    public string NodeName;
    public List<CommandNode> ChildrenNodes;

    public Action OnAction { get; private set; }
}
