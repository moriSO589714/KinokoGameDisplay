using UnityEngine;

public class CmdShowTxt : MonoBehaviour
{
    CmdSceneManager _cmdSceneManager = null;

    public void SayHelloWorld()
    {
        if (_cmdSceneManager == null) _cmdSceneManager = CmdSceneManager.Instance;
        _cmdSceneManager.OutPutManager.ReceiveMessage("HelloWorld", OutPutTextLogColorSets.SystemDefault);
    }

    /// <summary>
    /// オウム返しするメソッド
    /// </summary>
    public void SayReceiveMessage()
    {
        if (_cmdSceneManager == null) _cmdSceneManager = CmdSceneManager.Instance;
        //inputFieldのメッセージ送信先を切り替える
        _cmdSceneManager.InputFieldManager.ChangeAction( str => _cmdSceneManager.OutPutManager.ReceiveMessage(str, OutPutTextLogColorSets.SystemDefault));

        //終了時のアクションを登録する
        _cmdSceneManager.InputFieldManager._whenEndCurrentAction += () => { Debug.Log("オウム返しを終了します"); };
    }
}
