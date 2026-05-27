using System;
using UnityEngine;

/// <summary>
/// MainUIでのデリゲート設定などの管理を行うクラス
/// </summary>
public class ManageMainUI : MonoBehaviour
{
    [SerializeField] private MonitorPlayerInput _monitorPlayerInput;

    [SerializeField] private GameBoxsManager _gameBoxManager;
    [SerializeField] private GameObject _wifiMrkObj;
    [SerializeField] private GameObject _filterMrkObj;
    [SerializeField] private GameObject _canvas;
    
    private void Awake()
    {
        InitMainUI();
    }

    public void InitMainUI()
    {
        CommonStateManager commonStateManager = CommonStateManager.Instance;

        //ソフトウェア起動時に必要なロードを行う==========================================
        new LoadFlexibleDir().SetFlexibleDirByJson();
        //================================================================================

        //Wifiボタンのデリゲート設定======================================================
        NetWorkTab netWorkTab = _wifiMrkObj.GetComponent<NetWorkTab>();
        if(netWorkTab != null)
        {
            netWorkTab.InstancePanel = CreatePanel;
        }
        //================================================================================

        //フィルターボタンのデリゲート設定================================================
        DownTab filterMrkDowntab = _filterMrkObj.GetComponent<DownTab>();
        if(filterMrkDowntab != null)
        {

        }
        //================================================================================

        //マウススクロールの割り当て======================================================
        if (_monitorPlayerInput != null)
        {
            Action<float> act = _gameBoxManager.OnScroll;
            _monitorPlayerInput.onMouseScroll += act;
            //画面変更及び、ローディング時に外す
            commonStateManager.AddOnMainLoadingFunc(() => { _monitorPlayerInput.onMouseScroll -= act; });
            commonStateManager.AddOnMiniLoadingFunc(() => { _monitorPlayerInput.onMouseScroll -= act; });
            //ローディングから戻った際にもう一度設定
            commonStateManager.AddOutLoadingFunc(() => { _monitorPlayerInput.onMouseScroll += act; });
        }
        //================================================================================

        //ローカルで保存されているゲームをロードしてgameBoxを作成する=====================
        new SetGameBoxs(_gameBoxManager).SetAllGameBoxfromLocal();
        //================================================================================
    }

    private void CreatePanel(GameObject pref)
    {
        GameObject instance = InstantiatePref(pref);
    }

    /// <summary>
    /// プレハブをcanvas下に生成する
    /// </summary>
    private GameObject InstantiatePref(GameObject pref)
    {
        return Instantiate(pref, parent:_canvas.transform);
    }
}
