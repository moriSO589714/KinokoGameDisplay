using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// API通信を行わず、スプレッドシートの値を仮で返す
/// </summary>
public class OnNetGameInfoFromTest : OnNetGameInfo
{
    public List<List<string>> GetGameInfo(Vector2 startPos, Vector2 endPos)
    {
        //テスト用の替えデータ
        List<List<string>> alternaDataSet = AlternaDatas.SpStAlternaDatas;
        
        //シートの座標で指定されている1から始まる座標をローカルデータから切り出すために0から始まる座標に変える
        startPos.x--;
        startPos.y--;
        endPos.x--;
        endPos.y--;
        //替えデータから取得したい範囲を切り出し
        List<List<string>> UsedDataList = UsedLocalTable.TrimValueRangeFromLocalTable(alternaDataSet, startPos, endPos);
        return UsedDataList.Select(x => new List<string>(x)).ToList();
    }
}
