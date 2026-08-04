using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdSceneManager : BasedSingletonInMono<CmdSceneManager>
{
    //この変数はUnityのインスペクタ上からアタッチする---------
    public CmdInputFieldManager InputFieldManager;
    public OutputManager OutPutManager;
    //--------------------------------------------------------
}
