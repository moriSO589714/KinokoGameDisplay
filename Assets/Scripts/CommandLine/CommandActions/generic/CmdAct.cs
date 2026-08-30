using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdAct : MonoBehaviour
{
    protected CmdSceneManager _cmdSceneManager = null;

    /// <summary>
    /// コマンドラインでコマンドを送信した際に初めに呼ばれるようにするメソッド
    /// </summary>
    public virtual void FirstCall()
    {
        if (_cmdSceneManager == null) _cmdSceneManager = CmdSceneManager.Instance;
        _cmdSceneManager.InputFieldManager._endModeAction += End;
        Init();
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    protected virtual void Init()
    {

    }

    /// <summary>
    /// 終了時の処理
    /// </summary>
    protected virtual void End()
    {

    }

    /// <summary>
    /// コマンド受付モードに戻るメソッド
    /// </summary>
    protected void ReturnCmdReceiveMode()
    {
        _cmdSceneManager.OutPutManager.ReceiveMessage("コマンド受付モードに戻ります", OutPutTextLogColorSets.SystemDefault);
        //コマンド受付に戻す
        _cmdSceneManager.InputFieldManager.ReturnCommandReceive();
    }

    private void OnDestroy()
    {
        End();
    }
}
