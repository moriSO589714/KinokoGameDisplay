using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagFilterManager : FilterManagerFreeInput
{
    public List<string> TagFiltering => _labelFieldManager.ReturnActiveLabelTxts();

    protected override void Init()
    {
        base.Init();
        FilterCondition condition = _gameDatasSingleton.CurrentFilterCondition;
        if (condition != null)
        {
            List<List<string>> tagsConditions = condition.GameTags;
            AddSetedConditions(tagsConditions);
        }
    }
}