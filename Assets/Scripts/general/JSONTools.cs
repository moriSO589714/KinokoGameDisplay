using System;
using System.IO;
using UnityEngine;

/// <summary>
/// JSON関係の汎用的な処理をまとめたクラス
/// </summary>
public static class JSONTools
{
    /// <summary>
    /// jsonデータを読み取って任意の型に代入する
    /// </summary>
    /// <param name="jsonFilePath"></param>
    /// <returns></returns>
    public static T ReadJSON<T>(string jsonFilePath)
    {
        string stringDataByJSON = "";

        try
        {
            //指定パスのファイルを開く
            using (FileStream filestream = new FileStream(jsonFilePath, FileMode.Open, FileAccess.Read))
            {
                //StreamReaderでFileStreamから文字を読み取る
                using (StreamReader streamreader = new StreamReader(filestream))
                {
                    stringDataByJSON = streamreader.ReadToEnd();
                }
            }
        }
        catch(Exception e)
        {
            UnityEngine.Debug.LogException(e);
        }

        //UnityのJsonUtilityを利用してJSON形式のデータをGameDataクラスに代入する
        //JSONデータに不足がある場合、gameDataの各変数にはnullが入る
        //逆にGameDataクラス側に不足がある場合は代入は行われない -> GameDataの変数項目に追加が合った場合でも下位互換が可能
        //このときstringDataByJSONにはjson形式のテキストがそのまま入っている状態
        var inDataClass = JsonUtility.FromJson<T>(stringDataByJSON);

        return inDataClass;
    }

    /// <summary>
    /// 任意の型からjsonファイルを作成するジェネリッククラス
    /// </summary>
    public static void SerializeJson<T>(T serializedClass, string savePath)
    {
        string json = JsonUtility.ToJson(serializedClass);
        string dirName = Path.GetDirectoryName(savePath);
        if (!System.IO.Directory.Exists(dirName))
        {
            System.IO.Directory.CreateDirectory(dirName);
        }
        //文字列形式で保存されたjsonをファイルとして出力する
        File.WriteAllText(savePath, json);
    }
}
