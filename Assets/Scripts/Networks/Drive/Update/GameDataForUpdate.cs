using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameDataForUpdate
{
    public static bool QualityCheck(GameData _checkGameData, string localGamePath)
    {
        if(localGamePath != "")
        {
            if(_checkGameData.GameExeName == "")
            {
                return false;
            }
        }

        return true;
    }
}
