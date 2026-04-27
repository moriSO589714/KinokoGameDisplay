using Google.Apis.Sheets.v4;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// ネットワーク関係の値を保持しておくシングルトンクラス
/// API等などにより取得に時間がかかるものは基本的にこのクラスに入れておく
/// </summary>
public class NetworksSingleton : BasedSingleton<NetworksSingleton>
{
    //インターネット接続を行わずにテスト実行する
    public const bool isTestOnNet = true;

    private List<string> SpreadSheetElementOrder = new List<string>();
    private int LiminalRow = -1;

    public List<string> ReturnElementOrder()
    {
        if (SpreadSheetElementOrder.Count != 0)
        {
            return new List<string>(SpreadSheetElementOrder);
        }
        else
        {
            GetElementOrder();
            return new List<string>(SpreadSheetElementOrder);
        }
    }

    public void GetElementOrder()
    {
        AllDirs allDirs = AllDirs.GetInstance();
        SheetsService sheetsService = new CreateAPIService(allDirs.JsonPathKey).CreateSheetAPIService();
        OnNetGameInfo onNetGameInfo = null;
        if (!CheckInEnvironment.CheckInEditor() || CheckInEnvironment.CheckDoingNet())
        {
            onNetGameInfo = new OnNetGameInfoFromSpSt(sheetsService, allDirs.SpreadSheetID);
        }
        else if (!CheckInEnvironment.CheckInEditor() && !CheckInEnvironment.CheckDoingNet())
        {
            onNetGameInfo = new OnNetGameInfoFromSpSt(sheetsService, allDirs.SpreadSheetID);
        }
        List<string> sheetElementOrder = new CollectivelyGetFromSpSt().GetElementTypeArray(onNetGameInfo);
        if (sheetElementOrder == null || sheetElementOrder.Count <= 0) throw new System.Exception("failed to get sheetElement");

        SpreadSheetElementOrder = new List<string>(sheetElementOrder);
    }

    public int ReturnLiminalRow()
    {
        if(LiminalRow != -1)
        {
            return LiminalRow;
        }
        else
        {
            GetLiminalRow();
            return LiminalRow;
        }
    }

    public void GetLiminalRow()
    {
        NetworksSingleton networksSingleton = NetworksSingleton.Instance;
        List<string> spreadSheetElementOrder = networksSingleton.ReturnElementOrder();
        AllDirs allDirs = AllDirs.GetInstance();

        //GameIDが記載されている列数を取得する
        int numberofColumns = SpStTools.IndextoSSColumn(spreadSheetElementOrder.IndexOf("GameID"));
        Vector2 gameIDStartCell = new Vector2(numberofColumns, allDirs.SpreadSheetStartCellPos.y);
        string jsonPathKey = allDirs.JsonPathKey;
        SheetsService sheetsService = new CreateAPIService(jsonPathKey).CreateSheetAPIService();
        OnNetGameInfo onNetGameInfo = null;
        if(!CheckInEnvironment.CheckInEditor() || CheckInEnvironment.CheckDoingNet())
        {
            onNetGameInfo = new OnNetGameInfoFromSpSt(sheetsService, allDirs.SpreadSheetID);
        }
        else if (!CheckInEnvironment.CheckInEditor() && !CheckInEnvironment.CheckDoingNet())
        {
            onNetGameInfo = new OnNetGameInfoFromSpSt(sheetsService, allDirs.SpreadSheetID);
        }
        LastCellManager lastCellManager = new LastCellManager();
        //第二引数のマジックナンバー"3"は調べるセル(GameID)を指定してる。要修正
        int liminalRow = lastCellManager.ReturnLastCellPos(onNetGameInfo, new Vector2(numberofColumns, 3), SearchUnit.LargeRange, DirectionOnSpSt.row, null);
        LiminalRow = liminalRow;
    }
}
