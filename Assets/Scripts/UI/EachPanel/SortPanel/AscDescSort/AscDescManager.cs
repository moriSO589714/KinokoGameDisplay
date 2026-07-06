using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AscDescManager : SortElementManager
{
    protected override List<string> SetPullDownList()
    {
        return new SortLibrary().AscAndDescNames;
    }
}
