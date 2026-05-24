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

    public void InitMainUI()
    {
        CommonStateManager commonStateManager = CommonStateManager.Instance;

        //Wifiボタンのデリゲート設定======================================================
        NetWorkTab netWorkTab = _wifiMrkObj.GetComponent<NetWorkTab>();
        if(netWorkTab != null)
        {
            Vector2 moveDistance = netWorkTab.ReturnMoveDis();
            float moveSeconds = netWorkTab.ReturnMoveSeconds();
            float removeSeconds = netWorkTab.ReturnRemoveSeconds();
            SimpleDownAndUp simpleDownAndUp = new SimpleDownAndUp(_wifiMrkObj, moveDistance, moveSeconds, removeSeconds);
            netWorkTab.PointerEnterAct = simpleDownAndUp.MoveObject;
            netWorkTab.PointerExitAct = simpleDownAndUp.RemoveObject;
            netWorkTab.InstancePanel = CreatePanel;
        }
        //================================================================================

        //フィルターボタンのデリゲート設定================================================
        DownTab filterMrkDowntab = _filterMrkObj.GetComponent<DownTab>();
        if(filterMrkDowntab != null)
        {
            Vector2 moveDistance = filterMrkDowntab.ReturnMoveDis();
            float moveSeconds = filterMrkDowntab.ReturnMoveSeconds();
            float removeSeconds = filterMrkDowntab.ReturnRemoveSeconds();
            SimpleDownAndUp simpleDownAndUp = new SimpleDownAndUp(_filterMrkObj, moveDistance, moveSeconds, removeSeconds);
            filterMrkDowntab.PointerEnterAct = simpleDownAndUp.MoveObject;
            filterMrkDowntab.PointerExitAct = simpleDownAndUp.RemoveObject;
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
    }

    private void CreatePanel(GameObject pref)
    {
        GameObject instance = InstantiatePref(pref);
        instance.GetComponent<UIPanel>()?.InitPanel();
    }

    /// <summary>
    /// プレハブをcanvas下に生成する
    /// </summary>
    private GameObject InstantiatePref(GameObject pref)
    {
        return Instantiate(pref, parent:_canvas.transform);
    }

}
