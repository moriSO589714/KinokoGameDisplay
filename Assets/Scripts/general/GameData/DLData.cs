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
    //zipファイルの容量
    public long GameSize { get; private set; }
    //zipファイルの名前
    public string FileName { get; private set; }
    //分割したファイルの個数
    public long SplitedFileNum { get; private set; }
    //ファイル群にすべてのデータが揃っているか
    public bool IsCheckAllRequiredData { get; private set; } = false;

    /// <summary>
    /// インスタンス時にデータをセットするようのコンストラクタ
    /// </summary>
    /// <param name="filesPath"></param>
    public DLData(string filesPath)
    {
        SetDataByPath(filesPath);
    }
    public DLData(long gameSize, string fileName, long splitedFileNum)
    {
        SetDataByCustomDatas(gameSize, fileName, splitedFileNum);
    }
    public DLData(byte[] byteData)
    {
        SetDataByByte(byteData);
    }
    public DLData()
    {

    }

    /// <summary>
    /// 分割されたファイル群が入ったフォルダのパスからデータをセットするメソッド
    /// </summary>
    /// <param name="filesPath"></param>
    public void SetDataByPath(string filesPath)
    {
        //ファイル群のパスを順番にソートする
        string[] sortedFiles = new FreezingGeneric().sortingFilesByPath(filesPath);
    }

    /// <summary>
    /// データをセットするメソッド（""の場合はセットしない）
    /// </summary>
    /// <param name="gameSize">ゲームの全体容量(不明時は-1を代入)</param>
    /// <param name="fileName">ゲームデータが入ったフォルダの名前(不明時は""を代入)</param>
    /// <param name="splitedFileNum">ゲームが分割された際の分割数(不明時は-1を代入)</param>
    public void SetDataByCustomDatas(long gameSize, string fileName,long splitedFileNum)
    {
        if(gameSize != -1) GameSize = gameSize;
        if(fileName != "") FileName = fileName;
        if(splitedFileNum != -1) SplitedFileNum = splitedFileNum;
    }

    /// <summary>
    /// バイト配列からデータを取り出しセットする(通常、.00ファイルを読み込む際に利用する)
    /// </summary>
    /// <param name="byteData"></param>
    public void SetDataByByte(byte[] byteData)
    {
        string dlData = System.Text.Encoding.UTF8.GetString(byteData);

        string[] splitedDatas = dlData.Split(",");
        GameSize = long.Parse(splitedDatas[0]);
        FileName = splitedDatas[1];
        SplitedFileNum = long.Parse(splitedDatas[2]);
    }

    /// <summary>
    /// サイズとファイル名をバイト配列にして渡す(,で区切り)
    /// </summary>
    public byte[] ReturnByteData()
    {
        string toByteData = GameSize.ToString() + "," + FileName + "," + SplitedFileNum.ToString();
        byte[] byteData = System.Text.Encoding.UTF8.GetBytes(toByteData);
        return byteData;
    }


}
