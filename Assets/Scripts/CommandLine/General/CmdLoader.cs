using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdLoader : MonoBehaviour
{
    WordEmtCell _cmdLib;
    [SerializeField] CommandManager _commandManager;
    //AwakeでSceneManagerのInstanceから取得したくないため、このクラスではインスペクタでアタッチして参照する
    [SerializeField] CmdSceneManager _cmdSceneManager;
    private void Awake()
    {
        InitLoad();
    }

    private void InitLoad()
    {
        GameDataManager gameDataManager = new GameDataManager();
        new LoadFlexibleDir().SetFlexibleDirByJson();
        gameDataManager.LoadGameDataFromJsons();
        _cmdLib = _commandManager.GetCmdLib();
        SetCommandReceiver();
        _cmdSceneManager.InputFieldManager._setCommandReceiver = SetCommandReceiver;
    }

    private void SetCommandReceiver()
    {        
        _cmdSceneManager.InputFieldManager.ChangeAction(_commandManager.ReceiveCommand, _cmdLib);             
    }
}