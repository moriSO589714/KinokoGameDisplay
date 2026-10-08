using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OnNetUpdateGameInfoToSpSt : OnNetUpdateGameInfo
{

    private SheetsService _sheetsService = null;
    private string _sheetId;

    public OnNetUpdateGameInfoToSpSt(SheetsService service, string sheetId)
    {
        _sheetsService = service;
        _sheetId = sheetId;
    }

    public void UpdateGameInfo(List<string> newGameInfo, int targetRow)
    {
        //追加するゲーム情報のリストをSheetAPIで扱う型に変換
        List<IList<object>> addValues = new List<IList<object>>()
        {
            newGameInfo.Cast<object>().ToList()
        };

        ValueRange requestBody = new ValueRange() { Values = addValues};

        //値範囲の作成
        int maxColumns = new AZLibrary().AlphabetLibrary[newGameInfo.Count - 1];
        string range = $"A{targetRow}:{maxColumns}{targetRow}";

        SpreadsheetsResource.ValuesResource.UpdateRequest request
            = _sheetsService.Spreadsheets.Values.Update(requestBody, _sheetId, range);

        request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

        request.Execute();
    }
}
