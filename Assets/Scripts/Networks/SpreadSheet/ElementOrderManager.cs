using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// スプレッドシートの項目名の並びを扱うクラス
/// スプレッドシートの項目を取得する処理はGetDataFromSpStAPIクラスにあることに注意
/// このクラスはスプレッドシート上での要素の並びとローカルでの要素の変換を行う
/// </summary>
public static class ElementOrderManager
{
    /// <summary>
    /// 一行ぶんのスプレッドシートのデータをGameDataクラスに変換する
    /// </summary>
    /// <param name="sheetElementOrder">スプレッドシート上の項目順</param>
    /// <param name="sheetValues">1行ぶんの値が入ったリスト。取得していない要素・値が代入されていない要素も("")として代入する必要がある</param>
    /// <returns></returns>
    public static GameData SheetValuesToGameData(List<string> sheetElementOrder, List<string> sheetValues)
    {
        GameData resultGameData = new GameData();
        if(sheetElementOrder.Count != sheetValues.Count)
        {
            throw new System.Exception("引数において、スプレッドシート上の要素の項目数と渡された1行ぶんのデータ数に違いがあるためGameDataクラスへの格納が不可能です。");
        }

        //GameDataクラスで定義されているFieldを取得する
        FieldInfo[] gameDataFieldInfo = typeof(GameData).GetFields();
        //スプレッドシートの項目名とGameDataクラスのフィールドで定義されているフィールド名で一致するものがあった場合インデックス値を使い、GameDataクラスに格納する
        for(int i = 0; i <= sheetValues.Count - 1; i++)
        {
            int fieldIndex = Array.FindIndex(gameDataFieldInfo, x => x.Name == sheetElementOrder[i]);
            if (fieldIndex == -1) continue;

            //GameDataクラスでの変数の型がstringの配列であった場合、スプレッドシートの値を配列に加工する
            if (gameDataFieldInfo[fieldIndex].FieldType == typeof(string[]))
            {
                ForceReplaceWord forceReplaceWord = new ForceReplaceWord();
                string[] setValueArray = forceReplaceWord.DivideSheetStrToArray(sheetValues[i]);
                gameDataFieldInfo[fieldIndex].SetValue(resultGameData, setValueArray);
            }
            else //そのままスプシのデータを代入する場合
            {
                gameDataFieldInfo[fieldIndex].SetValue(resultGameData, sheetValues[i]);
            }
        }
        //GameDataのステータスはNotDownloadにしておく(他のステータスが当てはまる場合はこの処理が呼ばれた後に上書きすること)
        resultGameData.Status = GameStatus.NotDownloaded;
        //スプシの値を正しいフィールドに格納したGameDataクラスを返す
        return resultGameData;
    }

    /// <summary>
    /// GameDataクラスに格納されているデータをスプレッドシートで使えるよう、スプシの項目の並び順に並び替える
    /// </summary>
    /// <param name="sheetElementOrder"></param>
    /// <param name="gameData"></param>
    /// <returns></returns>
    public static List<string> GameDataToSheetFormat(List<string> sheetElementOrder, GameData gameData)
    {
        List<string> sheetFormatValues = new List<string>();
        //スプシ上でゲームデータを管理している領域に合うように、実際にスタートする位置まで空要素を代入する
        AllDirs allDirs = AllDirs.GetInstance();
        int columnStartInSheet = (int)allDirs.SpStElementStartCellPos.x;
        for(int i = 1; i < columnStartInSheet; i++)
        {
            sheetFormatValues.Add("");
        }

        //ゲームデータクラスで定義されているFieldを取得
        FieldInfo[] gameDataFieldInfo = typeof(GameData).GetFields();

        for(int i = 0; i <= sheetElementOrder.Count - 1; i++)
        {
            //スプレッドシートでの項目名と一致するGameDataの変数名が存在するか
            int fieldIndex = Array.FindIndex(gameDataFieldInfo, x => x.Name == sheetElementOrder[i]);
            if(fieldIndex == -1)
            {
                //GameDataクラスのフィールドにスプシの項目名と同じ名前の変数が存在しない
                sheetFormatValues.Add("");
                continue;
            }
            else
            {
                //渡されたGameDataインスタンスの値を取得
                var inInstanceGameDataValue = gameDataFieldInfo[fieldIndex].GetValue(gameData);
                if(inInstanceGameDataValue == null) //値がnullであった場合はそこで終了
                {
                    sheetFormatValues.Add("");
                    continue;
                }

                //GameDataの変数型がstringの配列であった場合
                if (gameDataFieldInfo[fieldIndex].FieldType == typeof(string[]))
                {
                    //string型に結合してからリストに格納する
                    ForceReplaceWord forceReplaceWord = new ForceReplaceWord();
                    string combinedStr = forceReplaceWord.CombineArrayToSheetStr((string[])inInstanceGameDataValue);
                    sheetFormatValues.Add(combinedStr);
                }
                else
                {
                    sheetFormatValues.Add((string)inInstanceGameDataValue);
                }
            }
        }
        return sheetFormatValues;
    }
}
