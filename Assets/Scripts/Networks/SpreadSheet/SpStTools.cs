using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class SpStTools
{
    public static int IndextoSSColumn(int indexNum)
    {
        return indexNum + (int)AllDirs.GetInstance().SpreadSheetStartCellPos.x;
    }

    public static int IndextoSSRow(int indexNum)
    {
        return indexNum + (int)AllDirs.GetInstance().SpreadSheetStartCellPos.y;
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
        if (startPos.x != endPos.x && startPos.y != endPos.y) return true;
        return false;
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
        List<List<string>> filledList = new List<List<string>>(targetList);

        if (direction == DirectionOnSpSt.row)
        {
            //必要な長さを取得
            int needlyLengthOfColumn = CalcCellsLength((int)startPos.x, (int)endPos.x);
            foreach(List<string> strsList in targetList)
            {
                if(strsList.Count < needlyLengthOfColumn)
                {
                    for(int i = 0; i < needlyLengthOfColumn - strsList.Count; i++)
                    {

                    }
                }
            }

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
        else if(direction == DirectionOnSpSt.column)
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
}

public enum DirectionOnSpSt 
{
    column,
    row,
}

