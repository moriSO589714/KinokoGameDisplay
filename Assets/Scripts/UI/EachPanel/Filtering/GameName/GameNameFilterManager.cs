using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameNameFilterManager : FilterManagerFreeInput
{
    public List<string> GameNameFiltering => _labelFieldManager.ReturnActiveLabelTxts();

    protected override void Init()
    {
        base.Init();
        FilterCondition condition = _gameDatasSingleton.CurrentFilterCondition;
        if (condition != null)
        {
            List<List<string>> gameNamesConditions = condition.GameNames;
            AddSetedConditions(gameNamesConditions);
        }
    }
}
