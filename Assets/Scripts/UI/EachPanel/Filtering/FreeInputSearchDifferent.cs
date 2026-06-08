using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FreeInputSearchDifferent
{
    //前回の入力の記憶用
    private string[] _lastTxtArray = null;

    public string SearchDifferent(string inputTxt)
    {
        //改行記号をスペースに置き換え
        string targetTxt = inputTxt.Replace("\n", " ");
        //テキストをスペースで分割
        string[] targetTxtArray = targetTxt.Split(new char[2] { ' ', '　'});

        //前回の入力から増加した語
        List<int> increasedWordsIndex = CompareArray(_lastTxtArray, targetTxtArray);

        //もし1単語のみが前回から増えている場合、増えた単語を返す
        if(increasedWordsIndex.Count == 1)
        {
            return targetTxtArray[increasedWordsIndex[0]];
        }
        else
        {
            return "";
        }
    }

    /// <summary>
    /// 2つの配列を比較し、一方に含まれない要素のindex値を返す
    /// </summary>
    /// <param name="comparedArray">含んでいるか調べられる母集団の配列</param>
    /// <param name="targetArray">含まれるか調べられる配列、母集団に含まれない場合こちらの配列でのindex値が返される</param>
    private List<int> CompareArray(string[] comparedArray, string[] targetArray)
    {
        List<int> resultList = new List<int>();
        for(int i = 0; i < targetArray.Length; i++)
        {
            int lastIndex = -1;
            if(comparedArray != null)
            {
                lastIndex = Array.IndexOf(comparedArray, targetArray[i]);
            }

            if(lastIndex == -1)
            {
                resultList.Add(i);
            }
        }
        return resultList;
    }
}
