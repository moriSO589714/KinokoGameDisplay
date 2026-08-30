using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnNetDeleteRowToSpSt : OnNetDeleteRow
{
    private SheetsService _sheetsService;
    private string _sheetId;

    public OnNetDeleteRowToSpSt(SheetsService sheetsService, string sheetId)
    {
        _sheetsService = sheetsService;
        _sheetId = sheetId;
    }

    public void DeleteRow(int deleteRow)
    {
        AllDirs allDirs = AllDirs.GetInstance();
        int startColumn = (int)allDirs.SpreadSheetStartCellPos.x;
        char alphabetOfSheet = new AZLibrary().AlphabetLibrary[startColumn - 1];
        //Zはスプレッドシートの最後の列数を意図したもの。AAなど続くようになった場合は変更する必要がある
        string clearRange = alphabetOfSheet + deleteRow.ToString() + ":" + "Z" + deleteRow.ToString();
        //ClearValuesRequestはAPIの設計上必要なもの。インスタンスしただけのもので問題ない。
        var request = _sheetsService.Spreadsheets.Values
                        .Clear(new ClearValuesRequest() ,_sheetId, clearRange);

        var response = request.Execute();       
    }
}
