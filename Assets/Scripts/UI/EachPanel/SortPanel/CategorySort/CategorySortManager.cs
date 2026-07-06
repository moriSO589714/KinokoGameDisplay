using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CategorySortManager : SortElementManager
{
    protected override List<string> SetPullDownList()
    {
        return new SortLibrary().CategoryNames;
    }
}
