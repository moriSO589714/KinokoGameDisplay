using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// スプレッドシートで扱うために、UIに表示する文字と内部で扱う文字の置き換えを行うクラス
/// (Ex.スペースはUIでは表示されるが、内部ではスペースとして扱わない)
/// </summary>
public class ForceReplaceWord
{
    //置き換えられる語の辞書<内部で扱う語, UIで表示される語>になる。
    public readonly Dictionary<string, string> ReplacedWordDictionary = new Dictionary<string, string>() 
    {
        {"~", " "}

    };
    //GameDataクラスでは配列として格納する必要がある文字列群をスプレッドシート上で一つの文字列として扱うための区切り文字
    public readonly string ArrayWordForSheet = "#";

    //内部で扱う語として登録されているため、ユーザーが入力することができない文字
    public List<string> UnAvailableWordsList => new List<string>(ReplacedWordDictionary.Keys) { ArrayWordForSheet };

    /// <summary>
    /// 置き換えする単語の辞書に従って引数の文字から置き換えを行う
    /// </summary>
    public string ReplacedWord(string originalText)
    {
        string resultText = originalText;
        foreach(var pair in ReplacedWordDictionary)
        {
            resultText = resultText.Replace(pair.Value, pair.Key);
        }
        return resultText;
    }

    /// <summary>
    /// スプレッドシートの文字を区切り文字から配列に変換する
    /// </summary>
    public string[] DivideSheetStrToArray(string sheetValue)
    {
        return sheetValue.Split(ArrayWordForSheet, System.StringSplitOptions.RemoveEmptyEntries);
    }

    /// <summary>
    /// GameData上のstring配列としての要素を区切り文字を使い１つの文字列に結合する
    /// </summary>
    public string CombineArrayToSheetStr(string[] strArray)
    {
        //joinでは先頭に区切り文字が追加されないため代入しておく
        string result = ArrayWordForSheet;
        //区切り文字を指定して結合
        result += string.Join(ArrayWordForSheet, strArray);
        return result;
    }
}
