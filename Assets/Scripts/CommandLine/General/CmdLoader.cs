using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdLoader : MonoBehaviour
{
    CmdSceneManager _cmdSceneManager;
    WordEmtCell _cmdLib;
    [SerializeField] CommandManager _commandManager;
    private void Awake()
    {
        InitLoad();
    }

    private void InitLoad()
    {
        _cmdSceneManager = CmdSceneManager.Instance;
        _cmdLib = _commandManager.GetCmdLib();
        SetCommandReceiver();
        _cmdSceneManager.InputFieldManager._setCommandReceiver = SetCommandReceiver;
        new LoadFlexibleDir().SetFlexibleDirByJson();
    }

    private void SetCommandReceiver()
    {
        _cmdSceneManager.InputFieldManager.ChangeAction(_commandManager.ReceiveCommand, _cmdLib);             
    }
}