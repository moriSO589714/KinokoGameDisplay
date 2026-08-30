using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdWrapBool
{
    public readonly string trueWord = "yes";
    public readonly string falseWord = "no";

    /// <summary>
    /// コマンドラインから送信されたstring形式のものをbool型に変換して返すメソッド
    /// </summary>
    public bool CmdMessageConvertBool(string message)
    {
        if(message == trueWord)
        {
            return true;
        }
        else if(message == falseWord)
        {
            return false;
        }
        else
        {
            throw new System.Exception($"{trueWord}と{falseWord}以外の単語の受け取りは不可能です");
        }
    }
}
