using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CmdSceneManager : BasedSingletonInMono<CmdSceneManager>
{
    //この変数はUnityのインスペクタ上からアタッチする---------
    [SerializeField] private CmdInputFieldManager _inputFieldManager;
    [SerializeField] private OutputManager _outPutManager;

    public CmdInputFieldManager InputFieldManager { get { return _inputFieldManager; }}
    public OutputManager OutPutManager { get { return _outPutManager; }}
    //--------------------------------------------------------
}