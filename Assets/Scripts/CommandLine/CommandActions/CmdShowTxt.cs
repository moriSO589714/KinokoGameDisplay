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
}
