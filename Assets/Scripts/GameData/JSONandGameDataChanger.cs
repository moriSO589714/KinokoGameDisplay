using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// JSONデータとGameDataクラスを変換するクラス
/// </summary>
public class JSONandGameDataChanger
{
    /// <summary>
    /// 指定のディレクトリにあるjsonファイル群を全てGameDataにする
    /// </summary>
    /// <param name="jsonDirPath">jsonファイル群が置かれているフォルダのパス</param>
    /// <returns>GameDataクラスのリスト</returns>
    public List<GameData> JSONDirPathToGameData(string jsonDirPath)
    {
        //指定のフォルダ内にある全てのファイルを取得する
        string[] files = Directory.GetFiles(jsonDirPath);
        //拡張子が.jsonではないファイルをnotJsonに記録しておく
        //https://shirakamisauto.hatenablog.com/entry/2016/06/27/080017
        var notJSON = files
        //匿名クラスのリストに要素とインデックスを射影する。
        .Select((p, i) => new { Content = p, Index = i })
        //射影されたリスト内でフィルタリング
        .Where(x =>
            {
                //匿名クラスのContent(配列filesの要素)から拡張子のみを抽出
                string extention = Path.GetExtension(x.Content);
                //もし拡張子がjsonでない場合はtrueを返す
                return extention != ".json";
            })
        //匿名クラスのリストからインデックスを抽出してnotJSONに追加する
        .Select(x => x.Index)
        .ToList();

        List<string> targetJSONs = files.ToList();
        //元のfilesからjsonデータを消す
        foreach (var index in notJSON)
        {
            targetJSONs.RemoveAt(index);
        }

        //jsonファイル群をGameDataクラスへ変換する
        List<GameData> gameDatas = new List<GameData>();
        foreach(string jsf in targetJSONs)
        {
            try
            {
                gameDatas.Add(JSONtoGameData(jsf));
            }
            catch(Exception e)
            {
                Debug.Log("jsonファイルからGameDataへの変換に失敗しました。FILEPATH:" + jsf + "ERROR>>" + e);
            }
        }

        return gameDatas;
    }

    /// <summary>
    /// JSONファイルのパスからGameDataクラスへデータを代入するクラス
    /// </summary>
    /// <param name="jsonFilePath">jsonファイルのパス</param>
    /// <returns>GameDataクラス(単体)</returns>
    public GameData JSONtoGameData(string jsonFilePath)
    {
        //JsonUtilityを利用するために、jsonファイルを読み込んでstring型にする必要がある
        string jsonData = new JSONTools().ReadJSON(jsonFilePath);
        //UnityのJsonUtilityを利用してJSON形式のデータをGameDataクラスに代入する
        //JSONデータに不足がある場合、gameDataの各変数にはnullが入る
        //逆にGameDataクラス側に不足がある場合は代入は行われない -> GameDataの変数項目に追加が合った場合でも下位互換が可能
        GameData gameData = JsonUtility.FromJson<GameData>(jsonData);
        return gameData;
    }
}
