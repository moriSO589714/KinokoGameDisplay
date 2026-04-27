using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// テスト用のAPI通信を行わず、スプレッドシートの値を仮で返す
/// </summary>
public class TestOnNetGameInfo : OnNetGameInfo
{
    public List<List<string>> GetGameInfoFromNet(Vector2 startPos, Vector2 endPos)
    {
        return new List<List<string>>(AlternaDatas.SpStAlternaDatas);
    }
}
