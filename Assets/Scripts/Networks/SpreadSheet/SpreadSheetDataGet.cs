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
    public List<string> GetElementTypeArray(string jsonKeyPath, string sheetID)
    {
        SheetsService sheetService = CreateSSAPI(jsonKeyPath);

        Vector2 targetPos = new Vector2(2, 2);
        //ScrollCellValueSearchで得られたディクショナリ型のデータを要素のみを取り出してリストに入れる
        List<string> elementTypeArray = ScrollCellValueSearch(sheetService, sheetID, targetPos, false).Select(x => x.Value).ToList();

        return elementTypeArray;
    }

    /// <summary>
    /// スプレッドシートから特定の条件にあうゲームの情報を取得し、GameDataクラスとして返す
    /// 複数の条件を適応させたい場合、このメソッドを複数実行する
    /// </summary>
    /// <param name="conditions">検索する条件を設定用。絞りこみたい条件を変数として格納したGameDataクラスを渡すことで、その条件にあうゲームの情報のみを返す</param>
    /// <returns></returns>
    public List<GameData> GetGameDataOfSpreadSheet(GameData condition, string jsonKeyPath, string sheetID)
    {
        //シートの最も上の行(C,*)から条件に合うものだけをゲームデータにして、配列に格納していく

        FieldInfo[] gameDataFields = typeof(GameData).GetFields();
        //絞り込み条件を<変数名,絞りこむ値>として保存するディクショナリ
        Dictionary<string, string> conditionsDictionary = new Dictionary<string, string>();
        //まず条件として指定されているものを取得する
        foreach (FieldInfo fi in gameDataFields)
        {
            var value = fi.GetValue(condition);
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
            conditionsDictionary[fi.Name] = librarySetStr;
        }

        //ディクショナリの要素とスプレッドシートの要素名の列数を変換させる
        NetworksSingleton networksSingleton = NetworksSingleton.Instance;
        List<string> sheetElementOrder = networksSingleton.GetElementOrder();

        SheetsService sheetService = CreateSSAPI(jsonKeyPath);

        Dictionary<int, string> rowNumAndConditions = new Dictionary<int, string>();
        foreach (var conditions in conditionsDictionary)
        {
            int rowOrder = sheetElementOrder.IndexOf(conditions.Key);
            if (rowOrder == -1) continue;
            //実際のスプレッドシートの列数に合わせるために整数2を足す
            rowOrder += 2;

            rowNumAndConditions[rowOrder] = conditions.Value;
        }

        Dictionary<Vector2, string> clearConditions = new Dictionary<Vector2, string>();
        //絞り込み条件が指定されている最初の列を探していって、条件にあうものがあればその他の条件も一致しているかを確認する
        foreach (var dicValue in rowNumAndConditions)
        {
            Dictionary<Vector2, string> getSpreadSheetValue = ScrollCellValueSearch(sheetService, sheetID, new Vector2(dicValue.Key, 3), true);
            clearConditions = getSpreadSheetValue.Where(x => CheckConditions(dicValue.Value, x.Value)).ToDictionary(x => x.Key, x => x.Value);

            //他の全ての条件に適していないものを削除する
            foreach (var clearDicValue in clearConditions)
            {
                //他に設定されている条件があるなら、その条件に合っているかを確かめる
                foreach (var conditions in rowNumAndConditions)
                {
                    if (conditions.Key == dicValue.Key) continue;

                    //検索されるセルの座標
                    Vector2 searchedCellPos = new Vector2(conditions.Key, clearDicValue.Key.y);
                    string checkedCellValue = ReturnSSValue(sheetService, sheetID, searchedCellPos, searchedCellPos)[0][0];

                    bool isClearCondition = CheckConditions(conditions.Value, checkedCellValue);
                    if (!isClearCondition) clearConditions.Remove(clearDicValue.Key); break;
                }
            }

            break;
        }


        //条件を満たしているゲームをGameDataクラスに変更する
        List<GameData> returnList = new List<GameData>();
        foreach (var lastClearValue in clearConditions)
        {
            //そのゲームのデータをスプレッドシートから全て取得
            List<List<string>> getedAllData = ReturnSSValue(sheetService, sheetID, new Vector2(2, lastClearValue.Key.y), new Vector2(sheetElementOrder.Count + 3, lastClearValue.Key.y));
            //多次元配列のリストをstringのリストに変換
            List<string> getedAllDataArray = new List<string>(getedAllData[0]);

            GameData createdGameData = SpreadSheetElementArrayToGameData(sheetElementOrder, getedAllDataArray);
            if (createdGameData != null) returnList.Add(createdGameData);
        }
        return new List<GameData>(returnList);
    }

    /// <summary>
    /// 1行のスプレッドシートの値のリスト(1つのゲームぶんのデータ)からGameDataクラスに格納するメソッド
    /// </summary>
    /// <param name="sheetElementOrder"></param>
    /// <param name="SpreadSheetValue">1行ぶんの値が入ったリスト。取得していない要素・値が代入されていない要素も("")として代入する必要がある</param>
    /// <returns></returns>
    public GameData SpreadSheetElementArrayToGameData(List<string> sheetElementOrder, List<string> SpreadSheetValue)
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

    /// <summary>
    /// 対象のセルが条件に合っているかを確認するメソッド
    /// </summary>
    /// <returns>条件に適している場合trueを返す</returns>
    public bool CheckConditions(string condition, string cellValue)
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
}
