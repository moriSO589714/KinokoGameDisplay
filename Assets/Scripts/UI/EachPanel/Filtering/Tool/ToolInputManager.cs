using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolInputManager : FreeInputManager
{
    protected override void ActivatePickUpCandidateProc()
    {
        GameDatasSingleton gameDatasSingleton = GameDatasSingleton.Instance;
        List<GameData> gameDatas = gameDatasSingleton.AllGameDatas;
        WordEmtCell wecLib = CreateLibFromGameDatas.CreateToolsLib(gameDatas);
        _pickUpCandidateElementProc = new PickUpCandidateElementForWE(wecLib);
    }
}
