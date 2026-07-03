using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolFilterManager : FilterManagerFreeInput
{
    public List<string> ToolFiltering => _labelFieldManager.ReturnActiveLabelTxts();
}
