using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// コマンド受付を行いたくない場合に使用する
/// </summary>
public class CmdNothing
{
    /// <summary>
    /// コマンドが送信されても何も行わないメソッド
    /// </summary>
    public void MessageGird(string message)
    {
        CmdSceneManager cmdSceneManager = CmdSceneManager.Instance;
        cmdSceneManager.OutPutManager.ReceiveMessage("現在コマンドの受付はできません。", OutPutTextLogColorSets.AccentDefault);
    }
}
