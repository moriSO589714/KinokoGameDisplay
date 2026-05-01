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
        //SearchLocalSpStDataByPosは配列のindex値での指定なので、シートの座標で指定されている引数を修正する
        startPos.x--;
        startPos.y--;
        endPos.x--;
        endPos.y--;

        List<List<string>> alternaDataSet = AlternaDatas.SpStAlternaDatas;
        List<List<string>> UsedDataList = SpStTools.SearchLocalSpStDataByPos(alternaDataSet, startPos, endPos);
        return UsedDataList.Select(x => new List<string>(x)).ToList();
    }
}
