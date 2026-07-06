using System;
using UnityEngine;

/// <summary>
/// MainUIでのデリゲート設定などの管理を行うクラス
/// </summary>
public class MainUIManager : MonoBehaviour
{
    [SerializeField] private MonitorPlayerInput _monitorPlayerInput;
    [SerializeField] private WatchingGameDlCueForUI _watchingGameDlCueForUI;

    [SerializeField] private GameBoxsManager _gameBoxManager;
    [SerializeField] private UIActBase _networkDownTab;
    [SerializeField] private GameObject _onFilteringMrk;
    [SerializeField] private UIActBase _filterDownTab;
    [SerializeField] private UIActBase _optionDownTab;
    [SerializeField] private UIActBase _sortDownTab;
    [SerializeField] private GameObject _canvas;

    [SerializeField] private UIPanel _onNetPanel;
    [SerializeField] private UIPanel _filterPanel;
    [SerializeField] private UIPanel _optionPanel;
    [SerializeField] private UIPanel _sortPanel;
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
        _networkDownTab.ClickAct = () => CreatePanel(_onNetPanel.gameObject);
        //================================================================================

        //フィルターボタンのデリゲート設定================================================
        _filterDownTab.ClickAct = () => CreatePanel(_filterPanel.gameObject);
        //================================================================================

        //オプションボタンのデリゲート設定================================================
        _optionDownTab.ClickAct = () => CreatePanel(_optionPanel.gameObject);
        //================================================================================

        //ソートボタンのデリゲート設定====================================================
        _sortDownTab.ClickAct = () => CreatePanel(_sortPanel.gameObject);
        //================================================================================

        //マウススクロールの割り当て======================================================
        if (_monitorPlayerInput != null)
        {
            Action<float> act = _gameBoxManager.OnScroll;
            _monitorPlayerInput.OnMouseScroll += act;
            //画面変更及び、ローディング時に外す
            commonStateManager.AddOnMainLoadingFunc(() => { _monitorPlayerInput.OnMouseScroll -= act; });
            commonStateManager.AddOnMiniLoadingFunc(() => { _monitorPlayerInput.OnMouseScroll -= act; });
            //メイン画面に戻った際は再び割り当てる
            commonStateManager.AddOutLoadingFunc(() => { _monitorPlayerInput.OnMouseScroll += act; });
        }
        //================================================================================

        //etcキーの割り当て===============================================================
        if(_monitorPlayerInput != null)
        {
            Action act = () => CreatePanel(_optionPanel.gameObject);
            _monitorPlayerInput.EscapeAct += act;
            //画面変更及びローディング時に外す
            commonStateManager.AddOnMainLoadingFunc(() => { _monitorPlayerInput.EscapeAct -= act; });
            commonStateManager.AddOnMiniLoadingFunc(() => { _monitorPlayerInput.EscapeAct -= act; });
            //メイン画面に戻った際は再び割り当てる
            commonStateManager.AddOutLoadingFunc(() => { _monitorPlayerInput.EscapeAct += act; });
        }
        //================================================================================

        //ローカルで保存されているゲームをロードしてgameBoxを作成する=====================
        new SetGameBoxs(_gameBoxManager).SetAllGameBoxfromLocal();
        //================================================================================
        //ダウンロード等時リロード用のアクションを登録
        _watchingGameDlCueForUI.EndProgressTaskAct += UpdataOnGameStatus;

        //フィルタリングマークのアクティブ化確認をメイン画面に戻る度に行う
        commonStateManager.AddOutLoadingFunc(CheckFiltering);
    }

    /// <summary>
    /// ゲームのステータスに変更があった場合(ダウンロードの完了など)にゲームボックスUIの再生成を行うメソッド
    /// </summary>
    private void UpdataOnGameStatus() 
    {
        new SetGameBoxs(_gameBoxManager).NoLoadSetCurrentDisplayGameBox();
    }

    private void CreatePanel(GameObject panel)
    {
        panel.SetActive(true);
    }

    private void CheckFiltering()
    {
        GameDatasSingleton gameDatasSingleton = GameDatasSingleton.Instance;
        if(gameDatasSingleton.CurrentFilterCondition == null)
        {
            _onFilteringMrk.SetActive(false);
        }
        else
        {
            _onFilteringMrk.SetActive(true);
        }
    }
}
