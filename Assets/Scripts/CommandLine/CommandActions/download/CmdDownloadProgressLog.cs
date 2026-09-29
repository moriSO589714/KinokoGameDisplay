using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdDownloadProgressLog
{
    private string _baseParts = "【ダウンロード状況】";
    private string _titleParts = "タイトル名：";
    private string _percentageParts = "進捗率：";

    private string _titleName = "";
    private string _percentage = "";

    public string MergeLog()
    {
        string merge = $"{_baseParts}\n{_titleParts}{_titleName}\n{_percentageParts}{_percentage}％";
        return merge;
    }

    public string UpdateTitle(string title, string customePercentate = "0")
    {
        _titleName = title;
        _percentage = customePercentate;
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
