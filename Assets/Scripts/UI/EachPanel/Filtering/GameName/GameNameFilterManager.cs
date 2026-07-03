using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameNameFilterManager : FilterManagerFreeInput
{
    public List<string> GameNameFiltering => _labelFieldManager.ReturnActiveLabelTxts();
}
