using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GameDlTask
{
    //タスクの名前
    public string TaskName { get; private set; } = "";
    //ダウンロードを実行するインスタンス
    public GameDLProc TaskInstance { get; private set; } = null;

    public GameDlTask(GameDLProc instance)
    {
        TaskInstance = instance;
        TaskName = instance.GameData.GameID;
    }
}
