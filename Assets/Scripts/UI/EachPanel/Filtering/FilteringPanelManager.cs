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
    [SerializeField] private GameBoxsManager _gameBoxsManager;

    protected override void Awake()
    {
        base.Awake();
        _confirmButton.ClickAct += ActivateFilteringProc;
    }

    public override void InitPanel()
    {
        base.InitPanel();
    }

    public void ActivateFilteringProc()
    {
        GameBoxFilter gameBoxFilter = new GameBoxFilter();
        FilterCondition currentFilterCondition = GenerateFilterConditions();
        List<GameData> filterdGameData = gameBoxFilter.FilteringGameDatas(currentFilterCondition);

        //シングルトンに登録する
        GameDatasSingleton gameDatasSingleton = GameDatasSingleton.Instance;
        gameDatasSingleton.SetCurrentDisplayGames(filterdGameData, currentFilterCondition);

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
}
