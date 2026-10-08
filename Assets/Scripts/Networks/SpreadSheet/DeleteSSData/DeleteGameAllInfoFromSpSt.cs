using Google.Apis.Sheets.v4;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

/// <summary>
/// ゲーム1つのデータをスプレッドシートから完全に削除する
/// </summary>
public class DeleteGameAllInfoFromSpSt
{
    private NetworksSingleton _networksSingleton;
    private AllDirs _allDirs;
    private SheetsService _sheetsService;
    private string _sheetId;
    private const string _gameIDVariableNameOnGameData = "GameID";
    
    public DeleteGameAllInfoFromSpSt()
    {
        _networksSingleton = NetworksSingleton.Instance;
        _sheetsService = _networksSingleton.ReturnSheetsService();
        _allDirs = AllDirs.GetInstance();
        _sheetId = _allDirs.SpreadSheetID;
    }

    /// <summary>
    /// スプレッドシートから指定のGameIdの情報を全て削除する
    /// ※もしスプレッドシートに指定のGameIdの情報がない場合、処理はスキップされる。
    /// </summary>
    public void DeleteGameInfo(string targetGameId)
    {
        NetworksSingleton _networksSingleton = NetworksSingleton.Instance;

        List<string> elementOrder = _networksSingleton.ReturnElementOrder(true);
        //スプレッドシートのデータを取得し直す
        List<List<string>> sheetDatas = _networksSingleton.ReturnGameInfoAllData(true);
        //このクラスのフィールドで定義されているGameIDを表す変数名が現在もGameDataクラスで定義されているかを確認する
        FieldInfo[] fieldInfosOfGameData = typeof(GameData).GetFields();
        if(!fieldInfosOfGameData.Any(x => x.Name == _gameIDVariableNameOnGameData))
        {
            throw new System.Exception("想定されているGameIDを表す変数がGameDataクラスに存在しません。");
        }

        //対象のGameIdのスプレッドシート上の行数を取得する
        int targetSheetRow = SpStTools.SearchSheetForRowFromGameId(_allDirs, elementOrder, _gameIDVariableNameOnGameData, sheetDatas, targetGameId);

        OnNetDeleteRow onNetDeleteRow = null;
        if (CheckInEnvironment.isOnNet)
        {
            onNetDeleteRow = new OnNetDeleteRowToSpSt(_sheetsService, _sheetId);
        }
        else
        {
            onNetDeleteRow = new OnNetDeleteRowToTest();
        }
        //削除を実行
        onNetDeleteRow.DeleteRow(targetSheetRow);
    }
}