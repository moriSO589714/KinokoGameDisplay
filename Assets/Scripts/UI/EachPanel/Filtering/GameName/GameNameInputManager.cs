using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameNameInputManager : FreeInputManager
{
    protected override void ActivatePickUpCandidateProc()
    {
        GameDatasSingleton gameDatasSingleton = GameDatasSingleton.Instance;
        List<string> titlesLib = gameDatasSingleton.ReturnTitlesLib();
        _pickUpCandidateElementProc = new PickUpCandidateElementForContains(titlesLib);
    }
}
