using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EachDataLoad
{
    /// <summary>
    /// ローカルのみでmain画面を構成するためのデータをロードする
    /// </summary>
    public void LocalDataLoad()
    {
        new LoadFlexibleDir().SetFlexibleDirByJson();
        new GameDataManager().OverallGameDataLoad();
    }
}
