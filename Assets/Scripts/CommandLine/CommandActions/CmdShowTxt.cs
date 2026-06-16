using UnityEngine;

public class CmdShowTxt : MonoBehaviour
{
    [SerializeField] CmdInputManager _cmdInputManager;
    [SerializeField] CmdOutPutManager cmdOutPutManager;

    public void ShowInputWord()
    {
        _cmdInputManager.ChangeAction(cmdOutPutManager.ReceiveMessage);
    }
}
