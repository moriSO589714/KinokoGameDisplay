using Google.Apis.Sheets.v4;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 実際にスプレッドシートからAPIを介してデータを取得する処理
/// </summary>
public class GetDataFromSpStAPI
{
    AllDirs _allDirs = null;
    SheetsService _sheetsService = null;
    OnNetGameInfo _onNetGameInfo = null;

    public GetDataFromSpStAPI()
    {
        Init();
    }

    private void Init()
    {
        _allDirs = AllDirs.GetInstance();
        string jsonPathKey = _allDirs.JsonPathKey;
        _sheetsService = NetworksSingleton.Instance.ReturnSheetsService();
        _onNetGameInfo = createOnNetGameInfo();
    }

    private OnNetGameInfo createOnNetGameInfo()
    {
        OnNetGameInfo onNetGameInfo = null;
        if (CheckInEnvironment.CheckDoingNet())
        {
            onNetGameInfo = new OnNetGameInfoFromSpSt(_sheetsService, _allDirs.SpreadSheetID);
        }
        else
        {
            onNetGameInfo = new OnNetGameInfoFromTest();
        }
        return onNetGameInfo;
    }

    public List<string> GetElementOrder()
    {
        //スプレッドシートで項目名がどの位置で始まっているかの位置をスプシ上の座標(1から始まる)で取得
        Vector2 elementStartPos = _allDirs.SpStElementStartCellPos;

        List<string> elementOrderList = new List<string>();
        //項目名が書かれた行の値を全て取得
        new LastCellManager().ReturnLastCellPos(_onNetGameInfo, elementStartPos, SearchUnit.NarrowRange, DirectionOnSpSt.column, elementOrderList);

        if (elementOrderList == null || elementOrderList.Count == 0)
        {
            throw new System.Exception("failed to get SpSt element order");
        }

        return elementOrderList;
    }

    /// <summary>
    /// スプレッドシートの最終行の行番号(1から始まる)を取得する
    /// 判定方法はGameIDの項目が書かれているセルがどこまであるか
    /// </summary>
    public int GetLiminalRow(List<string> elementOrder)
    {
        //GameIDが記載されているIndex値を取得
        int gameIdColumnIndex = elementOrder.IndexOf("GameID");
        //実際のスプレッドシートの座標に変換
        int gameIdColumnNum = SpStTools.IndextoSpStColumn(gameIdColumnIndex);
        Vector2 firstGameIdCellPos = new Vector2(gameIdColumnNum, _allDirs.SpStElementStartCellPos.y);

        int liminalRow = new LastCellManager().ReturnLastCellPos(_onNetGameInfo, firstGameIdCellPos, SearchUnit.LargeRange, DirectionOnSpSt.row, null);
        return liminalRow;
    }

    public List<List<string>> GetAllGameData(int liminalRow, List<string> elementOrder)
    {
        int searchEndX = SpStTools.IndextoSpStColumn(SpStTools.LengthToLastIndex(elementOrder.Count));
        //ゲームデータが書かれているテーブルの一番最後の座標(スプレッドシートでの座標)
        Vector2 searchEndPos = new Vector2(searchEndX, liminalRow);
        //値を取得
        List<List<string>> spStValue = _onNetGameInfo.GetGameInfo(_allDirs.SpreadSheetStartCellPos, searchEndPos);
        //空セルを埋める
        List<List<string>> filledList = SpStTools.FillInEmptyIndex(spStValue, _allDirs.SpreadSheetStartCellPos, searchEndPos, DirectionOnSpSt.column);

        return filledList;
    }
}
