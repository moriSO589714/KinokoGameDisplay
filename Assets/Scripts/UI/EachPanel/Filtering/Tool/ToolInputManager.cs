using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolInputManager : FreeInputManager
{
    protected override void ActivatePickUpCandidateProc()
    {
        GameDatasSingleton gameDatasSingleton = GameDatasSingleton.Instance;
        WordEmtCell wecLib = gameDatasSingleton.ReturnToolsLib();
        _pickUpCandidateElementProc = new PickUpCandidateElementForWE(wecLib);
    }
}
