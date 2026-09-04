using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeveropperInputManager : FreeInputManager
{
    protected override void ActivatePickUpCandidateProc()
    {
        GameDatasSingleton gameDatasSingleton = GameDatasSingleton.Instance;
        List<GameData> gameDatas = gameDatasSingleton.AllGameDatas;
        WordEmtCell wecLib = CreateLibFromGameDatas.CreateDeveropperLib(gameDatas);
        _pickUpCandidateElementProc = new PickUpCandidateElementForWE(wecLib);
    }
}
