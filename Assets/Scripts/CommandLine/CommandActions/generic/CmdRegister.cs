using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public static (string replayMessage, OutPutTextLogColorSets logColor) RegisterSingleCategory(string message, Action<string> registerAct)
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

    public static (string replayMessage, OutPutTextLogColorSets logColor) RegisterArrayCategory(string message, string[] formerArray, Action<string[]> registerAct)
    {
        string errorWord = CheckErrorWordInMessage(message);
        if(errorWord != "")
        {
            return ($"不正な文字が含まれています。送信し直してください。不正文字>>>{errorWord}", OutPutTextLogColorSets.AccentDefault);
        }

        if (formerArray != null && formerArray.Contains(message))
        {
            List<string> newList = formerArray.ToList();
            newList.Remove(message);
            registerAct(newList.ToArray());
            return ($"{message}を削除しました。続けて操作可能です。項目選択に戻る場合は「{new CmdReturn(null).ReturnWord}」を送信してください", OutPutTextLogColorSets.SystemDefault);
        }

        List<string> reNewList = new List<string>();
        if(formerArray != null)
        {
            reNewList = formerArray.ToList();
        }
        reNewList.Add(message);
        registerAct(reNewList.ToArray());
        return ($"{message}を登録しました。続けて操作可能です。項目選択に戻る場合は「{new CmdReturn(null).ReturnWord}」を送信してください", OutPutTextLogColorSets.SystemDefault);
    }
}
