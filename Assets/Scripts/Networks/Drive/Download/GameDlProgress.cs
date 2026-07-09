using System;
using UnityEngine;

public class GameDlProgress
{
    public string TaskName { get; private set; } = "";
    public string GameName { get; private set; } = "";
    public int MaxDLfiles = 0;
    public int NowDLedFileCount = 0;
    public float NowPercentage => (CalcPercentage(NowDLedFileCount, MaxDLfiles));

    public GameDlProgress(string taskName, string gameName)
    {
        TaskName = taskName;
        GameName = gameName;
    }

    private float CalcPercentage(int nowDLedFiles, int maxDLedFiles)
    {
        if (maxDLedFiles == 0) return 0;
        if (nowDLedFiles == 0) return 0;
        //int型同士の計算での小数点以下丸め込みを防ぐためにfloatのキャストをする
        float result = Mathf.Floor(((float)nowDLedFiles / maxDLedFiles) * 1000) / 10;
        return result;
    }
}
