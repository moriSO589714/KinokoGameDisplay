using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdDownloadProgressLog
{
    private string _baseParts = "ゲームをダウンロードしています。";
    private string _titleParts = "・現在ダウンロード中のタイトル名：";
    private string _percentageParts = "進捗率：";

    private string _titleName = "";
    private string _percentage = "";

    public string MergeLog()
    {
        string merge = $"{_baseParts}\n{_titleParts}{_titleName}\n{_percentageParts}{_percentage}";
        return merge;
    }

    public string UpdateTitle(string title, string customePercentate = "0")
    {
        _titleName = title;
        _percentageParts = customePercentate;
        string all = MergeLog();
        return all;
    }

    public string UpdatePercentage(string percentage)
    {
        _percentage = percentage;
        string all = MergeLog();
        return all;
    }
}
