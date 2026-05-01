using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class SpStTools
{
    public static int IndextoSpStColumn(int indexNum)
    {
        return indexNum + (int)AllDirs.GetInstance().SpreadSheetStartCellPos.x;
    }

    public static int IndextoSpStRow(int indexNum)
    {
        return indexNum + (int)AllDirs.GetInstance().SpreadSheetStartCellPos.y;
    }

    /// <summary>
    /// 配列・リストの要素数から最後のインデックス値にして返す
    /// </summary>
    public static int LengthToLastIndex(int length)
    {
        return length - 1;
    }

    /// <summary>
    /// スプレッドシート上の2点間の座標から、その間のセルの数を計算する
    /// </summary>
    public static int CalcCellsLength(int startPos, int endPos)
    {
        return (endPos - startPos) + 1;
    }

    /// <summary>
    /// 任意の2点が一直線上に並んでいるかを調べる
    /// </summary>
    public static bool isInLine(Vector2 startPos, Vector2 endPos)
    {
        if (startPos.x != endPos.x && startPos.y != endPos.y) return false;
        return true;
    }

    /// <summary>
    /// Vector2でのセル表記からR1C1表記に変更する
    /// </summary>
    /// <param name="cellValue">(列,行)</param>
    /// <returns></returns>
    public static string ChangeToR1C1(Vector2 cellValue)
    {
        string returnValue = "R" + cellValue.y.ToString() + "C" + cellValue.x.ToString();
        return returnValue;
    }

    /// <summary>
    /// 範囲に対して、不足しているIndexに ("") を追加する
    /// </summary>
    /// <param name="fillMode">列方向に満たすか、行方向に満たすか。trueだと行方向に満たす</param>
    public static List<List<string>> FillInEmptyIndex(List<List<string>> targetList, Vector2 startPos, Vector2 endPos, DirectionOnSpSt direction)
    {
        List<List<string>> filledList = targetList.Select(x => new List<string>(x)).ToList();

        if (direction == DirectionOnSpSt.column)
        {
            //必要な長さを取得
            int needlyLengthOfColumn = CalcCellsLength((int)startPos.x, (int)endPos.x);
            for(int i = 0; i < targetList.Count; i++)
            {
                if (targetList[i].Count < needlyLengthOfColumn)
                {
                    for (int i2 = 0; i2 < needlyLengthOfColumn - targetList[i].Count; i2++)
                    {
                        filledList[i].Add("");
                    }
                }
            }
        }
        else if(direction == DirectionOnSpSt.row)
        {
            int needlyLengthOfRow = CalcCellsLength((int)startPos.y, (int)endPos.y);
            int columnLength = CalcCellsLength((int)startPos.x, (int)endPos.x);
            if (targetList.Count < needlyLengthOfRow)
            {
                for(int i = 0; i < needlyLengthOfRow - targetList.Count; i++)
                {
                    //空のstringが入ったリストで不足ぶんを満たす
                    filledList.Add(new string[columnLength].Select(x => x = "").ToList());
                }
            }
        }

        return new List<List<string>>(filledList);
    }

    /// <summary>
    /// 範囲指定(Vector2)を行い２次元配列のリストから値を切り出す
    /// 範囲指定は0,0で始まる配列のindex値での指定
    /// </summar>
    public static List<List<string>> SearchLocalSpStDataByPos(List<List<string>> allData, Vector2 startPos, Vector2 endPos)
    {
        int lowestColumnNum = Math.Min((int)startPos.x, (int)endPos.x);
        int lowestRowNum = Math.Min((int)startPos.y, (int)endPos.y);
        int highestColumnNum = Math.Max((int)startPos.x, (int)endPos.x);
        int highestRowNum = Math.Max((int)startPos.y, (int)endPos.y);
        int allDataLastIndex = LengthToLastIndex(allData.Count);
        //指定された範囲のデータがテスト用のデータに無かった場合nullを返す
        if (allDataLastIndex < lowestRowNum) return null;
        int maxListsLength = 0;
        foreach (List<string> list in allData)
        {
            maxListsLength = maxListsLength < list.Count ? list.Count : maxListsLength;
        }
        if (LengthToLastIndex(maxListsLength) < lowestColumnNum) return null;

        List<List<string>> UsedDataList = new List<List<string>>();
        //リストの２次元配列から指定範囲を切り取る
        for (int i = lowestRowNum; i <= highestRowNum; i++)
        {
            //i列のデータが存在しない場合
            if (allDataLastIndex < i || i == -1)
            {
                break;
            }
            List<string> addDataList = new List<string>();
            for (int i2 = lowestColumnNum; i2 <= highestColumnNum; i2++)
            {
                if (LengthToLastIndex(allData[i].Count) < i2)
                {
                    continue;
                }
                else
                {
                    addDataList.Add(allData[i][i2]);
                }
            }
            UsedDataList.Add(addDataList);
        }

        return UsedDataList;
    }
}

public enum DirectionOnSpSt 
{
    column,
    row,
}

