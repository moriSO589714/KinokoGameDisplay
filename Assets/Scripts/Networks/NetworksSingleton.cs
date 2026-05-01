using Google.Apis.Sheets.v4;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// ネットワーク関係の値を保持しておくシングルトンクラス
/// API等などにより取得に時間がかかるものは基本的にこのクラスに入れておく
/// </summary>
public class NetworksSingleton : BasedSingleton<NetworksSingleton>
{
    private List<string> SpreadSheetElementOrder = null;
    private List<List<string>> GameInfoAllData = null;
    private int LiminalRow = -1;

    public List<string> ReturnElementOrder(bool forceLoad)
    {
        if (SpreadSheetElementOrder != null && !forceLoad)
        {
            return new List<string>(SpreadSheetElementOrder);
        }
        else
        {
            GetElementOrder();
            return new List<string>(SpreadSheetElementOrder);
        }
    }

    private void GetElementOrder()
    {
        AllDirs allDirs = AllDirs.GetInstance();
        SheetsService sheetsService = new CreateAPIService(allDirs.JsonPathKey).CreateSheetAPIService();
        OnNetGameInfo onNetGameInfo = null;
        if (!CheckInEnvironment.CheckInEditor() || CheckInEnvironment.CheckDoingNet())
        {
            onNetGameInfo = new OnNetGameInfoFromSpSt(sheetsService, allDirs.SpreadSheetID);
        }
        else if (CheckInEnvironment.CheckInEditor() && !CheckInEnvironment.CheckDoingNet())
        {
            onNetGameInfo = new OnNetGameInfoFromTest();
        }
        List<string> sheetElementOrder = new CollectivelyGetFromSpSt().GetElementTypeArray(onNetGameInfo);
        if (sheetElementOrder == null || sheetElementOrder.Count <= 0) throw new System.Exception("failed to get sheetElement");

        SpreadSheetElementOrder = new List<string>(sheetElementOrder);
    }

    public int ReturnLiminalRow(bool forceLoad)
    {
        if(LiminalRow != -1 && !forceLoad)
        {
            return LiminalRow;
        }
        else
        {
            GetLiminalRow();
            return LiminalRow;
        }
    }

    private void GetLiminalRow()
    {
        List<string> spreadSheetElementOrder = ReturnElementOrder(false);
        AllDirs allDirs = AllDirs.GetInstance();

        //GameIDが記載されている列数を取得する
        int num = spreadSheetElementOrder.IndexOf("GameID");
        int numberofColumns = SpStTools.IndextoSpStColumn(spreadSheetElementOrder.IndexOf("GameID"));
        Vector2 gameIDStartCell = new Vector2(numberofColumns, allDirs.SpreadSheetStartCellPos.y);
        string jsonPathKey = allDirs.JsonPathKey;
        SheetsService sheetsService = new CreateAPIService(jsonPathKey).CreateSheetAPIService();
        OnNetGameInfo onNetGameInfo = null;

        if (CheckInEnvironment.CheckDoingNet())
        {
            onNetGameInfo = new OnNetGameInfoFromSpSt(sheetsService, allDirs.SpreadSheetID);
        }
        else
        {
            onNetGameInfo = new OnNetGameInfoFromTest();
        }
        LastCellManager lastCellManager = new LastCellManager();
        //第二引数のマジックナンバー"3"は調べるセル(GameID)を指定してる。要修正
        int liminalRow = lastCellManager.ReturnLastCellPos(onNetGameInfo, gameIDStartCell, SearchUnit.LargeRange, DirectionOnSpSt.row, null);
        LiminalRow = liminalRow;
    }


    public List<List<string>> ReturnGameInfoAllData(bool forceLoad)
    {
        if(GameInfoAllData != null && !forceLoad)
        {
            return GameInfoAllData;
        }
        else
        {
            GetGameInfoAllData();
            return GameInfoAllData;
        }
    }

    private void GetGameInfoAllData()
    {
        int liminalRow = ReturnLiminalRow(true);
        List<string> elementOrder = ReturnElementOrder(false);
        AllDirs allDirs = AllDirs.GetInstance();

        string jsonPathKey = allDirs.JsonPathKey;
        SheetsService sheetsService = new CreateAPIService(jsonPathKey).CreateSheetAPIService();
        OnNetGameInfo onNetGameInfo = null;
        if (CheckInEnvironment.CheckDoingNet())
        {
            onNetGameInfo = new OnNetGameInfoFromSpSt(sheetsService, allDirs.SpreadSheetID);
        }
        else
        {
            onNetGameInfo = new OnNetGameInfoFromTest();
        }
        Vector2 searchEndPos = new Vector2(SpStTools.IndextoSpStColumn(SpStTools.LengthToLastIndex(elementOrder.Count)), liminalRow);
        List<List<string>> spStValue = onNetGameInfo.GetGameInfo(allDirs.SpreadSheetStartCellPos, searchEndPos);
        //空セルを埋める
        List<List<string>> filledList = SpStTools.FillInEmptyIndex(spStValue, allDirs.SpreadSheetStartCellPos, searchEndPos, DirectionOnSpSt.column);
        GameInfoAllData = filledList;
    }
}
