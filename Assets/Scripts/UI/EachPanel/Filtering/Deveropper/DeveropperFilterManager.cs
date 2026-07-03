using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeveropperFilterManager : FilterManagerFreeInput
{
    public List<string> DeveropperFiltering => _labelFieldManager.ReturnActiveLabelTxts();
}
