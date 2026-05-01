using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EachDataLoad : MonoBehaviour
{
    [SerializeField] GameBoxsManager gameBoxsManager;
    [SerializeField] UIActBase OnInterNetUI;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        OnInterNetUI.ClickAct = LoadInternetGameDatas;
    }
    /// <summary>
    /// ローカルのみでmain画面を構成するためのデータをロードする
    /// </summary>
    public void LocalDataLoad()
    {
        new GameDataManager().OverallGameDataLoad();
    }

    public void LoadInternetGameDatas()
    {
        new GameDataManager().LoadGameDataFromSpSt(null);
        List<GameData> gameDatas = GameDatasSingleton.Instance.GameDatas;
        gameBoxsManager.SetGameBoxsByGameDataList(gameDatas);
    }
}
