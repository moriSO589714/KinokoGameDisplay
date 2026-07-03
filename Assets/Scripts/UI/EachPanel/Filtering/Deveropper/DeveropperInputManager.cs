using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeveropperInputManager : FreeInputManager
{
    protected override void ActivatePickUpCandidateProc()
    {
        GameDatasSingleton gameDatasSingleton = GameDatasSingleton.Instance;
        WordEmtCell wecLib = gameDatasSingleton.ReturnDeveroppersLib();
        _pickUpCandidateElementProc = new PickUpCandidateElementForWE(wecLib);
    }
}
