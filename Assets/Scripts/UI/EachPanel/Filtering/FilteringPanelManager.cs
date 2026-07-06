using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FilteringPanelManager : UIPanel
{
    [SerializeField] private StatusFilterManager _statusManager;
    [SerializeField] private GameNameFilterManager _gameNameManager;
    [SerializeField] private ToolFilterManager _toolsFilterManager;
    [SerializeField] private TagFilterManager _tagFilterManager;
    [SerializeField] private DeveropperFilterManager _deveropperFilterManager;
    [SerializeField] private UIActBase _confirmButton;
    [SerializeField] private UIActBase _closeButton;
    [SerializeField] private GameBoxsManager _gameBoxsManager;

    protected override void Awake()
    {
        base.Awake();
        _confirmButton.ClickAct += ActivateFilteringProc;
        _closeButton.ClickAct += OnCloseProc;
    }

    public override void InitPanel()
    {
        base.InitPanel();
    }

    protected override void OnCloseProc()
    {
        _statusManager.PanelCloseProc();

        _gameNameManager.PanelCloseProc();
        _toolsFilterManager.PanelCloseProc();
        _tagFilterManager.PanelCloseProc();
        _deveropperFilterManager.PanelCloseProc();

        base.OnCloseProc();
    }

    public void ActivateFilteringProc()
    {
        FilterCondition currentFilterCondition = GenerateFilterConditions();

        GameDatasSingleton gameDatasSingleton = GameDatasSingleton.Instance;
        List<GameData> registerGameDatas = new List<GameData>();
        if(currentFilterCondition == null)
        {
            registerGameDatas = gameDatasSingleton.AllGameDatas;
            currentFilterCondition = null;
        }
        else
        {
            registerGameDatas = GameBoxFilter.FilteringGameDatas(currentFilterCondition, gameDatasSingleton.AllGameDatas);
        }

        //シングルトンに登録する
        gameDatasSingleton.SetCurrentDisplayGames(registerGameDatas, currentFilterCondition);
        _gameBoxsManager.GenerateBoxs(gameDatasSingleton.CurrentDisplayGames);
        OnCloseProc();
    }

    /// <summary>
    /// 各項目の設定値を取ってきてFilterConditionインスタンスを生成する
    /// </summary>
    private FilterCondition GenerateFilterConditions()
    {
        //各項目のフィルタリング設定
        List<GameStatus> filteringStatuses = _statusManager.StatusFiltering.Where(x => x.Value == true).Select(x => x.Key).ToList();
        
        List<string> filteringGameNames = _gameNameManager.GameNameFiltering;
        List<string> filteringTools = _toolsFilterManager.ToolFiltering;
        List<string> filteringTags = _tagFilterManager.TagFiltering;
        List<string> filteringDevs = _deveropperFilterManager.DeveropperFiltering;

        FilterCondition filterCondition = new FilterCondition
            (filteringStatuses,
            ConvertOrToList(filteringGameNames),
            ConvertOrToList(filteringTags),
            ConvertOrToList(filteringDevs),
            filteringTools);

        //もしフィルタリング条件が初期値と等しい場合(フィルタリングが行われず、全てのゲームを表示する場合)はnullを返す
        if (isNotFiltering(filterCondition))
        {
            return null;
        }

        return filterCondition;
    }

    /// <summary>
    /// ラベル群に含まれているorを起点にリスト分けを行う
    /// </summary>
    private List<List<string>> ConvertOrToList(List<string> plainList)
    {
        List<List<string>> ConvertedList = new List<List<string>>();

        List<string> stuckStrList = new List<string>();
        foreach (string s in plainList)
        {
            if(s != "or")
            {
                stuckStrList.Add(s);
            }
            else
            {
                ConvertedList.Add(stuckStrList);
                stuckStrList = new List<string>();
            }
        }
        if(stuckStrList.Count != 0)
        {
            ConvertedList.Add(stuckStrList);
        }

        return ConvertedList;
    }

    private bool isNotFiltering(FilterCondition conditions)
    {
        int statusKinds = Enum.GetValues(typeof(GameStatus)).Length;
        if (conditions.Statuses.Count == statusKinds)
        {
            if (conditions.GameNames.Count == 0 && conditions.GameTags.Count == 0 && conditions.GameDevs.Count == 0 && conditions.Softs.Count == 0)
            {
                return true;
            }
        }
        return false;
    }
}
