using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 現在時刻からゲームアップロード時のバージョンを特定の表現で生成する
/// </summary>
public static class NetworkGameVersionManager
{
    public static string CreateGameVersion()
    {
        string createVersion
            = DateTime.Now.Year.ToString("D4") + DateTime.Now.Month.ToString("D2") + DateTime.Now.Day.ToString("D2") + DateTime.Now.Hour.ToString("D2") + DateTime.Now.Minute.ToString("D2");
        return createVersion;
    }
}
