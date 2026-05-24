using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EachDataLoad : MonoBehaviour
{
    [SerializeField] GameBoxsManager gameBoxsManager;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {

    }

    /// <summary>
    /// ソフトウェア起動時に必要とするロード
    /// </summary>
    public void InitLoad()
    {
        //ディレクトリの場所を読み込み
        new LoadFlexibleDir().SetFlexibleDirByJson();
    }

    /// <summary>
    /// ローカルのみでmain画面を構成するためのデータをロードする
    /// </summary>
    public void LoadLocalData()
    {
        new GameDataManager().LoadGameDataFromJsons();
        List<GameData> gameDatas = GameDatasSingleton.Instance.GameDatas;
        gameBoxsManager.SetGameBoxsByGameDataList(gameDatas);
    }

    public void LoadInternetGameDatas()
    {
        new GameDataManager().LoadGameDataFromSpSt(null);
        List<GameData> gameDatas = GameDatasSingleton.Instance.GameDatas;
        gameBoxsManager.SetGameBoxsByGameDataList(gameDatas);
    }
}
