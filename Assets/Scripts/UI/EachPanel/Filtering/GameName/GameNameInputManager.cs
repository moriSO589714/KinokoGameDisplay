using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameNameInputManager : FreeInputManager
{
    protected override void ActivatePickUpCandidateProc()
    {
        GameDatasSingleton gameDatasSingleton = GameDatasSingleton.Instance;
        List<GameData> gameDatas = gameDatasSingleton.AllGameDatas;
        List<string> titlesLib = CreateLibFromGameDatas.CreateTitlesLib(gameDatas);
        _pickUpCandidateElementProc = new PickUpCandidateElementForContains(titlesLib);
    }
}