using Google.Apis.Sheets.v4;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Google.Apis.Sheets.v4.Data;

public class OnNetGameInfoFromSpSt : OnNetGameInfo
{
    private SheetsService _sheetsService;
    private string _spStId;

    public OnNetGameInfoFromSpSt(SheetsService sheetsService, string spStId)
    {
        _sheetsService = sheetsService;
        _spStId = spStId;
    }

    public List<List<string>> GetGameInfoFromNet(Vector2 startPos, Vector2 endPos)
    {
        if (_spStId == null) throw new Exception("failed to get cell. sheet id is null");
        //リクエストの作成
        var request = _sheetsService.Spreadsheets.Values
            .Get(_spStId, SpStTools.ChangeToR1C1(startPos) + ":" + SpStTools.ChangeToR1C1(endPos));

        List<List<string>> getValues = new List<List<string>>();
        ValueRange response = null;
        //リクエストの送信
        response = request.Execute();
        var responseValues = response.Values;

        //受け取ったリクエストをList<List<string>>の形に整形
        foreach(var value in responseValues)
        {
            List<string> strList = new List<string>();
            foreach(var invalue in value)
            {
                strList.Add(value.ToString());
            }
            getValues.Add(strList);
        }

        return new List<List<string>>(getValues);
    }
}
