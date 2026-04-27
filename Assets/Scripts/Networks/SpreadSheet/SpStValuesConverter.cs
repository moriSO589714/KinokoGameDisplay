using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpStValuesConverter
{
    /// <summary>
    /// スプレッドシートから取得したリストを<セル座標,セルの値>のディクショナリにして返す
    /// </summary>
    public static Dictionary<Vector2,string> ConvertSpStIntoDictionary(List<List<string>> spStList)
    {
        Dictionary<Vector2, string> convertedDictionary = new Dictionary<Vector2, string>();
        for (int i = 0; i < spStList.Count; i++)
        {
            for (int i2 = 0; i2 < spStList[i].Count; i2++)
            {
                Vector2 cellPos = new Vector2(SpStTools.IndextoSSColumn(i2), SpStTools.IndextoSSRow(i));
                convertedDictionary[cellPos] = spStList[i][i2];
            }
        }

        return new Dictionary<Vector2, string>(convertedDictionary);
    }


}
