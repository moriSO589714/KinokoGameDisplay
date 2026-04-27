using Google.Apis.Sheets.v4;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;

/// <summary>
/// スプレッドシートから最終行のセルを探す
/// </summary>
public class LastCellManager
{
    /// <summary>
    /// 指定セルから指定方向にテーブルの終了点がどこかを検索する
    /// </summary>
    /// <param name="targetPos">始点となるセルの座標</param>
    /// <param name="oneTimeSearchUnit">一度に検索をかける範囲</param>
    /// <param name="direction">検索方向</param>
    /// <param name="returnValuesList">値を保存しておきたい場合。必要なければnull</param>
    /// <returns></returns>
    public int ReturnLastCellPos(OnNetGameInfo onNetGameInfo, Vector2 targetPos, SearchUnit oneTimeSearchUnit, DirectionOnSpSt direction, List<string> returnValuesList)
    {
        bool loopFlag = true;
        Vector2 endSearchPos = Vector2.zero;
        int addNum = (int)oneTimeSearchUnit - 1; //実際に検索する座標に足した際、1つズレが生じるため

        int lastListLength = 0;
        while (loopFlag)
        {
            if(direction == DirectionOnSpSt.row)
            {
                endSearchPos = new Vector2(targetPos.x, targetPos.y + addNum);
            }
            else if(direction == DirectionOnSpSt.column)
            {
                endSearchPos = new Vector2(targetPos.x + addNum, targetPos.y);
            }
            if (!SpStTools.isInLine(targetPos, endSearchPos)) throw new Exception("range is not in one line");

            //スプレッドシートからデータを取得
            List<List<string>> spStValues = onNetGameInfo.GetGameInfoFromNet(targetPos, endSearchPos);
            
            //セルの値を保存しておく場合
            if(direction == DirectionOnSpSt.row && returnValuesList != null)
            {
                foreach(List<string> valueList in spStValues)
                {
                    returnValuesList.Add(valueList[0]);
                }
            }
            else if(direction == DirectionOnSpSt.column && returnValuesList != null)
            {
                returnValuesList.AddRange(spStValues[0]);
            }

            //セルの範囲がstartPos～endPosの範囲に収まっているかの確認
            if (spStValues == null) //検索する範囲の倍数に要素数が重なった場合nullが帰ってくる
            {
                lastListLength = 0;
                loopFlag = false;
            }
            //収まっている場合
            if(isInRange(spStValues.Count, (int)targetPos.y, (int)endSearchPos.y))
            {
                lastListLength = spStValues.Count;
                loopFlag = false;
            }
            //収まっていない場合(まだ値が入っている範囲が続いている場合)
            else
            {
                if(direction == DirectionOnSpSt.row)
                {
                    targetPos = new Vector2(endSearchPos.x, endSearchPos.y + 1);
                }
                else if(direction == DirectionOnSpSt.column)
                {
                    targetPos = new Vector2(endSearchPos.x + 1, endSearchPos.y);
                }
            }
        }
        //スプレッドシートでの最後の座標を求める
        int lastCellPos = 0;
        if(direction == DirectionOnSpSt.row)
        {
            lastCellPos = (int)endSearchPos.y - ((int)oneTimeSearchUnit - lastListLength);
        }
        else if(direction == DirectionOnSpSt.column)
        {
            lastCellPos = (int)endSearchPos.x - ((int)oneTimeSearchUnit - lastListLength);
        }

        return lastCellPos;
    }

    private bool isInRange(int listLength, int startPos, int endPos)
    {
        //引数から必要な要素の個数を取得する
        int needLength = SpStTools.CalcCellsLength(startPos,endPos);

        if (needLength > listLength) //取得してきた値が範囲内に収まっている場合
        {
            return true;
        }
        else if (needLength <= listLength)
        {
            return false;
        }
        else
        {
            throw new Exception("range error");
        }
    }
}

/// <summary>
/// 1度に調べる範囲
/// </summary>
public enum SearchUnit
{
    NarrowRange = 10, //行での利用を想定
    LargeRange = 100, //列での利用を想定
}
