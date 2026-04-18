using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// ネットワーク関係の値を保持しておくシングルトンクラス
/// API等などにより取得に時間がかかるものは基本的にこのクラスに入れておく
/// </summary>
public class NetworksSingleton : BasedSingleton<NetworksSingleton>
{
    private List<string> SpreadSheetElementOrder = new List<string>();

    /// <summary>
    /// スプレッドシートの要素の順番を格納するメソッド
    /// </summary>
    /// <param name="elementOrder">値渡しでリストを渡す</param>
    public void SetElemetOrder(List<string> elementOrder)
    {
        if(elementOrder == null || elementOrder.Count == 0)
        {
            return;
        }

        SpreadSheetElementOrder = elementOrder;
    }

    public List<string> GetElementOrder()
    {
        if(SpreadSheetElementOrder.Count != 0)
        {
            return new List<string>(SpreadSheetElementOrder);
        }
        else
        {
            AllDirs allDirs = AllDirs.GetInstance();
            List<string> sheetElementOrder = new SpreadSheetDataGet().GetElementTypeArray(allDirs.JsonPathKey, allDirs.SpreadSheetID);
            if (sheetElementOrder == null || sheetElementOrder.Count <= 0) throw new System.Exception("failed to get sheetElement");
            
            SpreadSheetElementOrder = new List<string>(sheetElementOrder);
            return sheetElementOrder;
        }
    }
}
