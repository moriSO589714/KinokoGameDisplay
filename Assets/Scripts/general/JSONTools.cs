using System;
using System.Diagnostics;
using System.IO;

/// <summary>
/// JSON関係の汎用的な処理をまとめたクラス
/// </summary>
public class JSONTools
{
    /// <summary>
    /// jsonデータを読み取ってstringデータにする
    /// </summary>
    /// <param name="jsonFilePath"></param>
    /// <returns></returns>
    public string ReadJSON(string jsonFilePath)
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

        //このときstringDataByJSONにはjson形式のテキストがそのまま入っている状態
        return stringDataByJSON;
    }
}
