using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TagInputManager : FreeInputManager
{
    protected override void ActivatePickUpCandidateProc()
    {
        GameDatasSingleton gameDatasSingleton = GameDatasSingleton.Instance;
        List<GameData> gameDatas = gameDatasSingleton.AllGameDatas;
        WordEmtCell wecLib = CreateLibFromGameDatas.CreateTagsLib(gameDatas);
        _pickUpCandidateElementProc = new PickUpCandidateElementForWE(wecLib);
    }
}
