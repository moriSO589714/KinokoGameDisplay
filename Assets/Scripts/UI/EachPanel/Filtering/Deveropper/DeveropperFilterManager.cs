using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeveropperFilterManager : FilterManagerFreeInput
{
    public List<string> DeveropperFiltering => _labelFieldManager.ReturnActiveLabelTxts();

    protected override void Init()
    {
        base.Init();
        FilterCondition condition = _gameDatasSingleton.CurrentFilterCondition;
        if(condition != null)
        {
            List<List<string>> devsConditions = condition.GameDevs;
            AddSetedConditions(devsConditions);
        }
    }
}
