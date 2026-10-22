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
        Debug.Log("flexibleデータの取得に成功しました(cmd)");
        gameDataManager.LoadGameDataFromJsons();
        Debug.Log("ゲームデータの取得に成功しました(cmd)");
        _cmdLib = _commandManager.GetCmdLib();
        Debug.Log("コマンドライブラリの取得に成功しました(cmd)");
        SetCommandReceiver();
        _cmdSceneManager.InputFieldManager._setCommandReceiver = SetCommandReceiver;
        Debug.Log("コマンドシーンのロードに成功しました(cmd)");
    }

    private void SetCommandReceiver()
    {        
        _cmdSceneManager.InputFieldManager.ChangeAction(_commandManager.ReceiveCommand, _cmdLib);             
    }
}