using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TagInputManager : FreeInputManager
{
    protected override void ActivatePickUpCandidateProc()
    {
        GameDatasSingleton gameDatasSingleton = GameDatasSingleton.Instance;
        WordEmtCell wecLib = gameDatasSingleton.ReturnTagsLib();
        _pickUpCandidateElementProc = new PickUpCandidateElementForWE(wecLib);
    }
}
