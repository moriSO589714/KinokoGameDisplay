using UnityEngine;

public class CmdShowTxt : MonoBehaviour
{
    CmdSceneManager _cmdSceneManager;

    private void Awake()
    {
        _cmdSceneManager = CmdSceneManager.Instance;
    }

    public void SayHelloWorld()
    {
        _cmdSceneManager.OutPutManager.ReceiveMessage("HelloWorld");
    }

    /// <summary>
    /// オウム返しするメソッド
    /// </summary>
    public void SayReceiveMessage()
    {
        //inputFieldのメッセージ送信先を切り替える
        _cmdSceneManager.InputFieldManager.ChangeAction(_cmdSceneManager.OutPutManager.ReceiveMessage);

        //終了時のアクションを登録する
        _cmdSceneManager.InputFieldManager._whenEndCurrentAction += () => { Debug.Log("オウム返しを終了します"); };



    }
}
