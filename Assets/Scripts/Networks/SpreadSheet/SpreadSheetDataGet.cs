using Google.Apis.Sheets.v4;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class SpreadSheetDataGet : SpreadSheetBased
{
    /// <summary>
    /// スプレッドシートから要素名(GameDataのjsonと共通)を取り出してきて、スプシにある順番で配列に格納して返す
    /// (重い処理なので複数走らせない)
    /// </summary>
    public List<string> GetElementTypeArray(string jsonKeyPath, string spStID)
    {
        SheetsService spStService = CreateSpStAPI(jsonKeyPath);

        AllDirs allDirs = AllDirs.GetInstance();
        Vector2 targetPos = new Vector2(allDirs.SpreadSheetStartCellPos.x, allDirs.SpreadSheetStartCellPos.y - 1);
        //ScrollCellValueSearchで得られたディクショナリ型のデータを要素のみを取り出してリストに入れる
        List<string> elementTypeArray = ScrollCellValueSearch(spStService, spStID, targetPos, false).Select(x => x.Value).ToList();

        return elementTypeArray;
    }

    /// <summary>
    /// スプレッドシートから特定の条件にあうゲームの情報を取得し、GameDataクラスとして返す
    /// 複数の条件を適応させたい場合、このメソッドを複数実行する
    /// </summary>
    /// <param name="conditions">検索する条件を設定用。絞りこみたい条件を変数として格納したGameDataクラスを渡すことで、その条件にあうゲームの情報のみを返す</param>
    /// <returns></returns>
    public List<GameData> FilterGameDataFromSpreadSheet(GameData filterGameData, string jsonKeyPath, string spStId)
    {
        AllDirs allDirs = AllDirs.GetInstance();
        //シートの最も上の行(C,*)から条件に合うものだけをゲームデータにして、配列に格納していく
        Dictionary<string, string> stringDictionaryFilter = GameDataFilterToDictionaryFilter(filterGameData);

        //ディクショナリの要素とスプレッドシートの要素名の列数を変換させる
        NetworksSingleton networksSingleton = NetworksSingleton.Instance;
        List<string> spStElementOrder = networksSingleton.ReturnElementOrder();

        SheetsService spStService = CreateSpStAPI(jsonKeyPath);

        //keyがstring型の要素名であったのを、スプレッドシートの列数に変換して値(条件)と一緒にディクショナリに格納する
        Dictionary<int, string> columnNumAndFilters = new Dictionary<int, string>();
        foreach (var filters in stringDictionaryFilter)
        {
            int columnOrder = StrFieldNameToElementColumnInSpSt(networksSingleton.ReturnElementOrder(),filters.Key);
            if (columnOrder == -1) continue;
            columnNumAndFilters[columnOrder] = filters.Value;
        }
        //最終的に絞り込まれたゲームの1セル
        Dictionary<Vector2, string> lastFilterdCellDictionary = RemoveNoGoodElement(columnNumAndFilters, spStService, spStId);

        //条件を満たしているゲームをGameDataクラスに変更する
        List<GameData> returnList = new List<GameData>();
        foreach (var lastFilteredCellPair in lastFilterdCellDictionary)
        {
            //そのゲームのデータをスプレッドシートから全て取得
            List<List<string>> getedAllData = ReturnSSValue(spStService, spStId, new Vector2(allDirs.SpreadSheetStartCellPos.x, lastFilteredCellPair.Key.y), new Vector2(new SpreadSheetTools().IndextoSSColumn(spStElementOrder.Count), lastFilteredCellPair.Key.y));
            //多次元配列のリストをstringのリストに変換
            List<string> getedAllDataArray = new List<string>(getedAllData[0]);

            GameData createdGameData = SpreadSheetElementArrayToGameData(spStElementOrder, getedAllDataArray);
            if (createdGameData != null) returnList.Add(createdGameData);
        }
        return new List<GameData>(returnList);
    }

    private Dictionary<string, string> GameDataFilterToDictionaryFilter(GameData filterGameData)
    {
        FieldInfo[] gameDataFields = typeof(GameData).GetFields();
        //絞り込み条件を<変数名,絞りこむ値>として保存するディクショナリ
        Dictionary<string, string> filtersDictionary = new Dictionary<string, string>();
        //まず条件として指定されているものを取得する
        foreach (FieldInfo fi in gameDataFields)
        {
            var value = fi.GetValue(filterGameData);
            if (value == null) continue;

            string librarySetStr = "";
            if (fi.FieldType == typeof(string[]))
            {
                librarySetStr += "#";
                librarySetStr += string.Join("#", (string[])value);
            }
            else
            {
                librarySetStr += value.ToString();
            }
            filtersDictionary[fi.Name] = librarySetStr;
        }

        return filtersDictionary;
    }

    /// <summary>
    /// スプレッドシートの要素名の並び順リストから、string型で指定された要素名がスプレッドシートの何列目にあるかを返す
    /// </summary>
    /// <param name="spStElementOrder"></param>
    /// <param name="convertedStrFieldName"></param>
    /// <returns>スプレッドシートの列数(配列のindex値ではない)</returns>
    private int StrFieldNameToElementColumnInSpSt(List<string> spStElementOrder, string convertedStrFieldName)
    {
        int elementColumn = spStElementOrder.IndexOf(convertedStrFieldName);
        if (elementColumn == -1) return elementColumn;
        elementColumn = new SpreadSheetTools().IndextoSSColumn(elementColumn);
        return elementColumn;
    }

    private Dictionary<Vector2, string> RemoveNoGoodElement(Dictionary<int, string> filterDictionary, SheetsService spStService, string spStId)
    {
        AllDirs allDirs = AllDirs.GetInstance();
        int firstCheckColumn = filterDictionary.First().Key;
        NetworksSingleton networksSingleton = NetworksSingleton.Instance;
        int liminulColumns = networksSingleton.ReturnLiminalRow();

        //検索を行う始めの1列をスプレッドシートから取得してくる
        List<List<string>> firstCheckSpStValues = ReturnSSValue(spStService, spStId, new Vector2(firstCheckColumn, allDirs.SpreadSheetStartCellPos.y), new Vector2(firstCheckColumn, liminulColumns));
        //多重配列で取得してきた値を座標,値のディクショナリに変換する
        Dictionary<Vector2, string> firstCheckSpStDic = ConvertWListintoDictionary(firstCheckSpStValues);

        Dictionary<Vector2, string> clearValues = firstCheckSpStDic.Where(x => CheckConditions(filterDictionary[firstCheckColumn], x.Value)).ToDictionary(x => x.Key, x => x.Value);
        
        //始めの1列以外の条件にあっているかを確かめる
        List<Vector2> removeInClearVeluesKey = new List<Vector2>();
        foreach(var clearedPairs in clearValues)
        {
            foreach (var filterPairs in filterDictionary)
            {
                //一度確認している列は検索しない
                if (filterPairs.Key == firstCheckColumn) continue;
                //セルの値を取得
                Vector2 searchedCellPos = new Vector2(filterPairs.Key, clearedPairs.Key.y);
                List<List<string>> getedValue = ReturnSSValue(spStService, spStId, searchedCellPos, searchedCellPos);
                string checkedCellValue = "";
                bool isClear = CheckConditions(filterPairs.Value, checkedCellValue);
                if(!isClear) removeInClearVeluesKey.Add(clearedPairs.Key); break;
            }
        }

        //clearValuesから他条件に適合しない要素を削除する
        foreach (var removePairs in removeInClearVeluesKey)
        {
            clearValues.Remove(removePairs);
        }

        return clearValues;
    }

    /// <summary>
    /// 対象のセルが条件に合っているかを確認するメソッド
    /// </summary>
    /// <returns>条件に適している場合trueを返す</returns>
    private bool CheckConditions(string condition, string cellValue)
    {
        string[] splitCondition = condition.Split('#', StringSplitOptions.RemoveEmptyEntries);
        string[] splitCellValue = cellValue.Split("#", StringSplitOptions.RemoveEmptyEntries);
        if (splitCondition.Length == 1)
        {
            if (splitCellValue.Contains(splitCondition[0])) return true;
        }
        //配列型で条件が複数存在する場合(タグなど)
        else
        {
            int counter = 0;
            //条件が全て合致するかを確認する
            foreach (string str in splitCondition)
            {
                if (splitCellValue.Contains(str)) counter++;
            }
            if (counter == splitCondition.Count()) return true;
        }

        return false;
    }

    /// <summary>
    /// 1行のスプレッドシートの値のリスト(1つのゲームぶんのデータ)からGameDataクラスに格納するメソッド
    /// </summary>
    /// <param name="sheetElementOrder"></param>
    /// <param name="SpreadSheetValue">1行ぶんの値が入ったリスト。取得していない要素・値が代入されていない要素も("")として代入する必要がある</param>
    /// <returns></returns>
    private GameData SpreadSheetElementArrayToGameData(List<string> sheetElementOrder, List<string> SpreadSheetValue)
    {
        GameData returnGameData = new GameData();
        if (sheetElementOrder.Count != SpreadSheetValue.Count)
        {
            throw new System.Exception("dont match list length between sheetElementOrder and SpreadSheetValue. try to reload sheetElementOrder");
        }

        FieldInfo[] gamedataFieldInfo = typeof(GameData).GetFields();
        //GameDataクラスで定義されたフィールド名の取得、代入
        for (int i = 0; i < SpreadSheetValue.Count; i++)
        {
            int fieldInfoIndex = Array.FindIndex(gamedataFieldInfo, x => x.Name == sheetElementOrder[i]);
            if (fieldInfoIndex == -1) continue;

            //変数型がstringの配列であった場合,配列に加工してから代入する
            if (gamedataFieldInfo[fieldInfoIndex].FieldType == typeof(string[]))
            {
                string[] setValueArray = SpreadSheetValue[i].Split("#", StringSplitOptions.RemoveEmptyEntries);
                gamedataFieldInfo[fieldInfoIndex].SetValue(returnGameData, setValueArray);
            }
            else
            {
                gamedataFieldInfo[fieldInfoIndex].SetValue(returnGameData, SpreadSheetValue[i]);
            }
        }

        return returnGameData;
    }

}
