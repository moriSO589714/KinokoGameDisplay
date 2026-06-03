public static class StrTools
{
    /// <summary>
    /// 文字数以上の部分を削除して...に置き換える。wordLimitsは...分の3文字も含まれる
    /// </summary>
    public static string ReplaceOverWords(string targetStr, int wordLimits)
    {
        string returnStr = targetStr;
        //制限された文字数以上であった場合以下の処理を実行する。
        if (targetStr.Length > wordLimits)
        {
            //余剰分以外を抽出
            string splited = targetStr.Substring(0, wordLimits - 3);
            //...を文章に付け加える
            returnStr = splited + "...";
        }

        return returnStr;
    }
}
