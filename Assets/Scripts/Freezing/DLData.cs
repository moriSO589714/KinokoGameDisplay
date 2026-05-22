using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// データ通信時に分割したファイルの整合性をチェックするための情報を格納するクラス
/// string型がbyte[]として、.00に保存される
/// </summary>
public class DLData
{
    //zipファイルのサイズ(Bite)
    public long GameSize { get; private set; }
    //zipファイルの名前
    public string FileName { get; private set; }
    //分割したファイルの個数
    public long SplitFileNum { get; private set; }
    //ファイル群にすべてのデータが揃っているか
    public bool IsCheckAllRequiredData { get; private set; } = false;

    public readonly string DLDataFileExtention = ".000";

    /// <summary>
    /// インスタンス時にデータをセットするようのコンストラクタ
    /// </summary>
    /// <param name="filesPath"></param>
    public DLData(string filesPath)
    {
        DeserializeDataByFilePath(filesPath);
    }
    public DLData(long gameSize, string fileName, long splitFileNum)
    {
        SerializeDataByCustomDatas(gameSize, fileName, splitFileNum);
    }
    public DLData(byte[] byteData)
    {
        DeserializeDataByByte(byteData);
    }
    public DLData()
    {

    }

    /// <summary>
    /// 分割されたファイル群が入ったフォルダのパスからデータをセットするメソッド
    /// </summary>
    /// <param name="filesPath"></param>
    public void SerializeDataByPath(string filesPath)
    {
        //ファイル群のパスを順番にソートする
        string[] sortedFiles = new FreezingTools().sortingFilesByPath(filesPath);
    }

    /// <summary>
    /// データをセットするメソッド（""の場合はセットしない）
    /// </summary>
    /// <param name="gameSize">ゲームの全体容量(不明時は-1を代入)</param>
    /// <param name="fileName">ゲームデータが入ったフォルダの名前(不明時は""を代入)</param>
    /// <param name="splitFileNum">ゲームが分割された際の分割数(不明時は-1を代入)</param>
    public void SerializeDataByCustomDatas(long gameSize, string fileName,long splitFileNum)
    {
        if(gameSize != -1) GameSize = gameSize;
        if(fileName != "") FileName = fileName;
        if(splitFileNum != -1) SplitFileNum = splitFileNum;
    }

    /// <summary>
    /// バイト配列からデータを取り出しセットする(通常、.00ファイルを読み込む際に利用する)
    /// </summary>
    /// <param name="byteData"></param>
    public void DeserializeDataByByte(byte[] byteData)
    {
        string dlData = System.Text.Encoding.UTF8.GetString(byteData);

        string[] splitedDatas = dlData.Split(",");
        GameSize = long.Parse(splitedDatas[0]);
        FileName = splitedDatas[1];
        SplitFileNum = long.Parse(splitedDatas[2]);
    }

    public void DeserializeDataByFilePath(string filePath)
    {
        //ファイルをバイト配列に変換
        byte[] bytes = File.ReadAllBytes(filePath);
        //バイト配列を使ったデシリアライズを行う
        DeserializeDataByByte(bytes);
    }

    /// <summary>
    /// サイズとファイル名をバイト配列にして渡す(,で区切り)
    /// </summary>
    public byte[] ReturnByteData()
    {
        string toByteData = GameSize.ToString() + "," + FileName + "," + SplitFileNum.ToString();
        byte[] byteData = System.Text.Encoding.UTF8.GetBytes(toByteData);
        if (byteData == null) throw new Exception("converted byteArray is null.");
        return byteData;
    }

    /// <summary>
    /// 自身をファイルとして指定のパスに保存する
    /// </summary>
    public void SerializeDLData(string savePath)
    {
        using (FileStream fs = new FileStream(savePath, FileMode.Create, FileAccess.Write))
        {
            fs.Write(this.ReturnByteData());
        }
    }
}
