using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Services;
using System;
using System.IO;
using UnityEngine;
using Google.Apis.Sheets.v4.Data;
using System.Collections.Generic;
using Unity.VisualScripting;

public class SpreadSheetBased
{
    /// <summary>
    /// スプレッドシート接続用のAPIを作成する。
    /// </summary>
    /// <returns></returns>
    public SheetsService CreateSpStAPI(string jsonKeyPath)
    {
        GoogleCredential credential;
        using (var stream = new FileStream(jsonKeyPath, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream).CreateScoped(SheetsService.ScopeConstants.Spreadsheets);
        }

        SheetsService sheetService = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "Spread Sheet",
        });

        return sheetService;
    }

    /// <summary>
    /// 指定したセルの値を取得して返す
    /// </summary>
    /// <param name="startCellPosition">値を返す範囲の始めの(列,行)</param>
    /// <param name="endCellPosition">値を返す範囲の終わりの(列,行)</param>
    /// <returns>セルの値[0]A1,A2,A3...
    ///                 [1]B2,B2,B3...
    ///                 ※空セルの場合nullを返す！注意！</returns>
    public List<List<string>> ReturnSSValue(SheetsService sheetService, string sheetID, Vector2 startCellPosition, Vector2 endCellPosition)
    {
        if (sheetID == null) throw new Exception("failed to get cell value. dont load ssid");
        //リクエストのシート名は省略することで、1番始めのシートが自動で選択される(qiita行き？)
        var request = sheetService.Spreadsheets.Values.Get(sheetID, ChangeToR1C1(startCellPosition) + ":" + ChangeToR1C1(endCellPosition));
        List<List<string>> returnValues = new List<List<string>>();
        try
        {
            ValueRange response = request.Execute();
            var values = response.Values;

            if (values == null) return null;
            foreach(IList<object> list in values)
            {
                List<string> strings = new List<string>();
                foreach(object value in list)
                {
                    strings.Add(value.ToString());
                }
                returnValues.Add(strings);
            }

            //spread sheet APIで無視される末尾の空セルを埋める(行方向のみ)
            int columnLength = ((int)endCellPosition.x - (int)startCellPosition.x) + 1;
            foreach(List<string> value in returnValues)
            {
                if(value.Count < columnLength)
                {
                    for(int i = 0; i < columnLength - value.Count; i++)
                    {
                        value.Add("");
                    }
                }
            }
        }
        catch(Exception e)
        {
            Debug.Log(e);
            throw new Exception("failed to connect with SpreadSheet. Log>>>" + e);
        }

        return returnValues;
    }

    /// <summary>
    /// ２次元配列のリスト(ReturnSSValueで取得したセルバリュー)を座標をKeyにしたDictionaryに変換して返す
    /// </summary>
    /// <param name="wList"></param>
    /// <returns>[セル座標(列,行),セルの値]のDictionary</returns>
    public Dictionary<Vector2,string> ConvertWListintoDictionary(List<List<string>> wList)
    {
        Dictionary<Vector2,string> convertedDictionary = new Dictionary<Vector2,string>();
        SpreadSheetTools spreadSheetTools = new SpreadSheetTools();
        for(int i = 0; i < wList.Count; i++)
        {
            for(int i2 = 0; i2 < wList[i].Count; i2++)
            {
                //定数1を足しているのは、リストのindexが0で始まるため
                Vector2 cellPos = new Vector2(spreadSheetTools.IndextoSSColumn(i2), spreadSheetTools.IndextoSSRow(i));
                convertedDictionary[cellPos] = wList[i][i2];
            }
        }

        return convertedDictionary;
    }

    /// <summary>
    /// 特定の行または列から、垂直または水平方向に値が設定されていない場所まで値を取得し、文字列の配列として返す
    /// </summary>
    /// <param name="startPos">検索を開始するセルの場所(列,行)</param>
    /// <param name="isRow">true =　列方向への検索, false = 行方向への検索</param>
    /// <returns></returns>
    public Dictionary<Vector2, string> ScrollCellValueSearch(SheetsService sheetService, string sheetID, Vector2 startPos, bool isRow)
    {
        Dictionary<Vector2, string> getValueDic = new Dictionary<Vector2, string>();
        Vector2 targetPos = startPos;

        bool flag = true;
        while (flag)
        {
            List<List<string>> returnValues = ReturnSSValue(sheetService, sheetID, targetPos, targetPos);
            if(returnValues == null)
            {
                flag = false; break;
            }
            else
            {
                getValueDic[targetPos] = returnValues[0][0];
                if (isRow) targetPos.y += 1;
                else targetPos.x += 1;
            }
        }

        if(getValueDic.Count == 0)
        {
            throw new System.Exception("empty or failed to get elementTyoe from SpreadSheet.");
        }

        return new Dictionary<Vector2, string>(getValueDic);
    }

    /// <summary>
    /// Vector2でのセル表記からR1C1表記に変更する
    /// </summary>
    /// <param name="cellValue">(列,行)</param>
    /// <returns></returns>
    public string ChangeToR1C1(Vector2 cellValue)
    {
        string returnValue = "R" + cellValue.y.ToString() + "C" + cellValue.x.ToString();
        return returnValue;
    }
}
