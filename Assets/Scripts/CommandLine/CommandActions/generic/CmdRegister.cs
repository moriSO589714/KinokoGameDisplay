using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro.EditorUtilities;
using UnityEngine;

public static class CmdRegister
{
    public static string CheckErrorWordInMessage(string message)
    {        
        ForceReplaceWord forceReplaceWord = new ForceReplaceWord();
        string containsErrorWord = "";
        foreach (string word in forceReplaceWord.UnAvailableWordsList)
        {
            if (message.Contains(word))
            {
                containsErrorWord = word;
                break;
            }
        }

        if (containsErrorWord != "")
        {
            return containsErrorWord;
        }

        return "";
    }

    public static (string replyMessage, OutPutTextLogColorSets logColor) RegisterSingleCategory(string message, Action<string> registerAct)
    {
        string errorWord = CheckErrorWordInMessage(message);
        if (errorWord != "")
        {
            return ($"不正な文字が含まれています。送信し直してください。不正文字>>>{errorWord}", OutPutTextLogColorSets.AccentDefault);            
        }

        //GameDataインスタンスの特定のフィールドに登録
        registerAct(message);
        return ($"{message}を登録しました", OutPutTextLogColorSets.SystemDefault);
    }

    public static (string replyMessage, OutPutTextLogColorSets logColor) RegisterArrayCategory(string message, List<string> formerList, Action<List<string>> registerAct)
    {
        string errorWord = CheckErrorWordInMessage(message);
        if(errorWord != "")
        {
            return ($"不正な文字が含まれています。送信し直してください。不正文字>>>{errorWord}", OutPutTextLogColorSets.AccentDefault);
        }

        if (formerList != null && formerList.Contains(message))
        {
            List<string> newList = new List<string>(formerList);
            newList.Remove(message);
            registerAct(newList);
            return ($"{message}を削除しました。続けて操作可能です。項目選択に戻る場合は「{new CmdReturn(null).ReturnWord}」を送信してください", OutPutTextLogColorSets.SystemDefault);
        }

        List<string> reNewList = new List<string>();
        if(formerList != null)
        {
            reNewList = new List<string>(formerList);
        }
        reNewList.Add(message);
        registerAct(reNewList);
        return ($"{message}を登録しました。続けて操作可能です。項目選択に戻る場合は「{new CmdReturn(null).ReturnWord}」を送信してください", OutPutTextLogColorSets.SystemDefault);
    }

    /// <summary>
    /// 多次元リストにstring型の変数を1リスト1つずつ登録していくメソッド
    /// GameIdの登録用に利用
    /// </summary>
    public static (string replyMessage, OutPutTextLogColorSets logColor) RegisterSingleValInWList(string message, List<List<string>> formerWList, Action<List<List<string>>> registerAct)
    {
        string errorWord = CheckErrorWordInMessage(message);
        if(errorWord != "")
        {
            return ($"不正な文字が含まれています。送信し直してください。不正文字>>>{errorWord}", OutPutTextLogColorSets.AccentDefault);
        }

        //各Listに代入されている値をまとめて1次元のリストにする
        List<string> singleList = new List<string>();
        bool isRemoveValue = false;
        string systemMessage = "";
        foreach(List<string> formerList in formerWList)
        {
            if(formerList == null || formerList.Count != 1)
            {
                return ($"事前に登録されたリストの形が正しくありません", OutPutTextLogColorSets.AccentDefault);
            }

            string targetVal = formerList[0];
            if(targetVal == message)
            {
                isRemoveValue = true;
                systemMessage = $"{message}を削除しました。続けて操作可能です。項目選択に戻る場合は「{new CmdReturn(null).ReturnWord}」を送信してください";
                continue;
            }

            singleList.Add(formerList[0]);
        }

        //値の新規登録
        if (!isRemoveValue)
        {
            singleList.Add(message);
            systemMessage = $"{message}を登録しました。続けて操作可能です。項目選択に戻る場合は「{new CmdReturn(null).ReturnWord}」を送信してください";
        }

        //多次元リストに再形成
        List<List<string>> resultVal = new List<List<string>>();
        foreach(string str in singleList)
        {
            resultVal.Add(new List<string>{ str });
        }
        registerAct.Invoke(resultVal);

        return (systemMessage, OutPutTextLogColorSets.SystemDefault);
    }

    public static (string replyMessage, OutPutTextLogColorSets logColor) RegisterPath(string message, Action<string> registerAct)
    {
        if (Directory.Exists(message))
        {
            registerAct(message);
            string systemMessage = "登録完了しました。項目選択に戻ります";
            return (systemMessage, OutPutTextLogColorSets.SystemDefault);
        }
        else
        {
            string errorMessage = "送信されたパスは存在しません。";
            return (errorMessage, OutPutTextLogColorSets.AccentDefault);
        }
    }

    public static (string replyMessage, OutPutTextLogColorSets logColor) RegisterExePath(string message, string userGameDirPath, Action<string> registerAct)
    {
        //パスの存在確認
        if (!File.Exists(message))
        {
            string errorMessage = "送信されたパスのファイルは存在しません";
            return (errorMessage, OutPutTextLogColorSets.AccentDefault);
        }

        //実行ファイルが入っているフォルダが入力されているかの確認
        if(userGameDirPath == null || userGameDirPath == "")
        {
            string errorMessage = "ゲームが入ったフォルダのパスが登録されていません。先にフォルダパスを登録する必要があります。";
            return (errorMessage, OutPutTextLogColorSets.AccentDefault);
        }

        //実行ファイルが指定されたゲームフォルダの下にあるかを確認して、相対パスに編集
        if (message.StartsWith(userGameDirPath)) //親子関係の確認部分。パスの先頭からの一致で判定
        {
            //相対パスに編集
            string parentPath = userGameDirPath + "\\";
            string childPath = message.Replace(parentPath, "");

            registerAct(childPath);
            string systemMessage = "登録完了しました。項目選択に戻ります";
            return (systemMessage, OutPutTextLogColorSets.SystemDefault);
        }
        else
        {
            string errorMessage = "指定された実行ファイルはゲームフォルダの中に存在しません";
            return (errorMessage, OutPutTextLogColorSets.AccentDefault);
        }
    }
}
