using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolFilterManager : FilterManagerFreeInput
{
    public List<string> ToolFiltering => _labelFieldManager.ReturnActiveLabelTxts();

    protected override void Init()
    {
        base.Init();
        FilterCondition condition = _gameDatasSingleton.CurrentFilterCondition;
        if (condition != null)
        {
            List<string> softsConditions = condition.Softs;
            AddSetedConditions(softsConditions);
        }
    }
}
