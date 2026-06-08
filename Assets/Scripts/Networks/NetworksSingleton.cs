using Google.Apis.Drive.v3;
using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ネットワーク関係の値を保持しておくシングルトンクラス
/// API等などにより取得に時間がかかるものは基本的にこのクラスに入れておく
/// </summary>
public class NetworksSingleton : BasedSingleton<NetworksSingleton>
{
    private List<string> _spreadSheetElementOrder = null;
    private List<List<string>> _allGameDataOnSpSt = null;
    private int _liminalRow = -1;

    private SheetsService _sheetsService = null;
    private DriveService _driveService = null;

    public List<string> ReturnElementOrder(bool forceLoad)
    {
        if (_spreadSheetElementOrder != null && !forceLoad)
        {
            return new List<string>(_spreadSheetElementOrder);
        }
        else
        {
            _spreadSheetElementOrder = new GetDataFromSpStAPI().GetElementOrder();
            return _spreadSheetElementOrder;
        }
    }

    public int ReturnLiminalRow(bool forceLoad)
    {
        if(_liminalRow != -1 && !forceLoad)
        {
            return _liminalRow;
        }
        else
        {
            List<string> elementOrder = ReturnElementOrder(false);
            _liminalRow = new GetDataFromSpStAPI().GetLiminalRow(elementOrder);
            return _liminalRow;
        }
    }

    public List<List<string>> ReturnGameInfoAllData(bool forceLoad)
    {
        if(_allGameDataOnSpSt != null && !forceLoad)
        {
            return _allGameDataOnSpSt;
        }
        else
        {
            int liminalRow = ReturnLiminalRow(false);
            List<string> elementOrder = ReturnElementOrder(false);
            _allGameDataOnSpSt = new GetDataFromSpStAPI().GetAllGameData(liminalRow, elementOrder);
            return _allGameDataOnSpSt;
        }
    }

    public SheetsService ReturnSheetsService()
    {
        if(_sheetsService == null)
        {
            AllDirs allDirs = AllDirs.GetInstance();
            _sheetsService = new CreateAPIService(allDirs.JsonPathKey).CreateSheetAPIService();
        }

        return _sheetsService;
    }

    public DriveService ReturnDriveService()
    {
        if(_driveService == null)
        {
            AllDirs allDirs = AllDirs.GetInstance();
            _driveService = new CreateAPIService(allDirs.JsonPathKey).CreateDriveAPIService();
        }

        return _driveService;
    }
}
