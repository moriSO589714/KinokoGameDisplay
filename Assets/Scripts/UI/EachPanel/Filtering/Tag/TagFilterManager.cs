using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagFilterManager : FilterManagerFreeInput
{
    public List<string> TagFiltering => _labelFieldManager.ReturnActiveLabelTxts();
}