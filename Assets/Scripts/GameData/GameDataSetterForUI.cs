using UnityEngine;
using UnityEngine.UI;

public static class GameDataSetterForUI
{
    public static void SetPlainStrData(string setStr ,int wordLimit, Text targetTextObj)
    {
        if (setStr == null || setStr == "") targetTextObj.text = "";
        //置き換え文字を元に戻す
        string strForDisplay = new ForceReplaceWord().ReturnReplacedWord(setStr);
        //文字数制限の適用
        string resultStr = StrTools.ReplaceOverWords(strForDisplay, wordLimit);
        //オブジェクトに反映
        targetTextObj.text = resultStr;
    }
}
