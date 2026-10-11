using UnityEngine;

public class CmdAct : MonoBehaviour
{
    protected CmdSceneManager _cmdSceneManager = null;
    protected virtual string _thisMode => "";

    /// <summary>
    /// コマンドラインでコマンドを送信した際に初めに呼ばれるようにするメソッド
    /// </summary>
    public virtual void FirstCall()
    {
        if (_cmdSceneManager == null) _cmdSceneManager = CmdSceneManager.Instance;
        _cmdSceneManager.InputFieldManager._endModeAction += End;
        Init();
        _cmdSceneManager.OutPutManager.SendLogMessage($"{_thisMode}モードに変更します", OutPutTextLogColorSets.SystemDefault);
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
    protected virtual void ReturnCmdReceiveMode()
    {
        _cmdSceneManager.OutPutManager.SendLogMessage("コマンド受付モードに戻ります", OutPutTextLogColorSets.SystemDefault);
        //コマンド受付に戻す
        _cmdSceneManager.InputFieldManager.ReturnCommandReceive();
    }

    private void OnDestroy()
    {
        End();
    }
}
