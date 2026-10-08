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
            for (int i = 0; i < targetList.Count; i++)
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
        else if (direction == DirectionOnSpSt.row)
        {
            int needlyLengthOfRow = CalcCellsLength((int)startPos.y, (int)endPos.y);
            int columnLength = CalcCellsLength((int)startPos.x, (int)endPos.x);
            if (targetList.Count < needlyLengthOfRow)
            {
                for (int i = 0; i < needlyLengthOfRow - targetList.Count; i++)
                {
                    //空のstringが入ったリストで不足ぶんを満たす
                    filledList.Add(new string[columnLength].Select(x => x = "").ToList());
                }
            }
        }

        return new List<List<string>>(filledList);
    }

    /// <summary>
    /// ゲームIDからスプレッドシートの何行目に登録されているのかを検索する
    /// </summary>   
    public static int SearchSheetForRowFromGameId(AllDirs allDirs, List<string> elementOrder, string gameIdVariableName, List<List<string>> sheetDatas, string targetGameId)
    {
        int gameIdIndexInSheet = elementOrder.IndexOf(gameIdVariableName);
        //ローカルにキャッシュされているスプシのデータ(List<List<string>>形式)から当てはまる行のインデックス値を取得
        int targetGameIndexInLocalList = -1;
        for(int i = 0; i <= sheetDatas.Count - 1; i++)
        {
            if (sheetDatas[i][gameIdIndexInSheet] == targetGameId)
            {
                targetGameIndexInLocalList = i;
                break;
            }
        }
        //指定のGameIDのゲームが見つからなかった場合
        if(targetGameIndexInLocalList == -1)
        {
            throw new Exception("スプレッドシートに対象となるGameIdを持つゲームが登録されていません。");
        }

        //実際のスプレッドシートの行に変換する(項目名の行などデータテーブルでは無い部分)
        int resultSheetRow = targetGameIndexInLocalList + (int)allDirs.SpreadSheetStartCellPos.y;
        return resultSheetRow;
    }
}

public enum DirectionOnSpSt 
{
    column,
    row,
}

