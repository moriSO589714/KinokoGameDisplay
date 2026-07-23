using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System.Collections.Generic;
using System.Linq;

public class OnNetAppEndGameInfoToSpSt : OnNetAppEndGameInfo
{
    private readonly string _allCellsRange = "A1";

    private SheetsService _sheetsService = null;
    private string _sheetId;

    public OnNetAppEndGameInfoToSpSt(SheetsService service, string sheetId)
    {
        _sheetsService = service;
        _sheetId = sheetId;
    }

    /// <param name="appEndGameInfo">データの挿入に利用しない列もListに含める必要がある(空stringとして代入)</param>
    public void AppEndGameInfo(List<string> appEndGameInfo)
    {
        //渡された追加するゲーム情報のリストをSheetAPIで扱う型に変換する
        List<IList<object>> addValues = new List<IList<object>>()
        {
            appEndGameInfo.Cast<object>().ToList()
        };

        ValueRange requestBody = new ValueRange() { Values = addValues };
        SpreadsheetsResource.ValuesResource.AppendRequest request 
            = _sheetsService.Spreadsheets.Values.Append(requestBody, _sheetId, _allCellsRange);

        //stringで入力された数値をシート上で数値として扱うために、ValueInputOptionを指定する
        request.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

        //リクエストの実行
        request.Execute();
    }
}
