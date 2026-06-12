using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// ゲームの詳細情報が入るクラス
/// </summary>
[Serializable]
[System.Diagnostics.DebuggerDisplay(
    "ゲームタイトル：{GameTitle}\nゲームのフォルダ名：{GameDirName}\nゲームの実行ファイルの名前：{GameExeName}\nゲームの固有番号：{GameID}\nゲームバージョン{GameVersion}" +
    "\nゲームの説明：{GameDescription}\nゲームの作者：{GameDevelopper}\nゲームツール：{GameSoftwareType}\nゲームのDriveID：{GameDriveId}\nゲームのサムネ画像のDriveID：{GameImageId}" +
    "\nゲームに付与されているタグ：{GameTags}\nゲームの状態：{Status}"
    )]
public class GameData
{
    public string GameTitle; //ゲームのタイトル
    [MustItem]
    public string GameDirName; //ゲームのフォルダ名
    [MustItem]
    public string GameExeName; //ゲームの実行ファイルの名前
    [MustItem]
    public string GameID; //ゲームの固有番号
    public string GameVersion; //ゲームのバージョン(2504011225の形)
    public string GameDescription; //ゲームの説明
    public string[] GameDevelopper; //ゲームの作者
    public string GameSoftwareType; //使われたゲーム開発環境
    public string GameDriveId; //ゲームのドライブID
    public string GameImageId; //ゲームのサムネのドライブID
    public string GameImageName; //ゲームのサムネのファイル名(〇〇.png)
    public string[] GameTags; //ゲームに付与されているタグ
    [MustItem]
    public GameStatus Status; //ゲームの状態(ダウンロードされているかなど)
}

/// <summary>
/// ゲームのステータスを表す
/// </summary>
public enum GameStatus
{
    ByLocal, //ローカルで追加されたゲーム
    NotDownloaded,
    Downloading,
    Downloaded,
    UpdateAvailable, //ダウンロードされているけど新しいバージョンがある
}

/// <summary>
/// GameDataの必要要件を満たしているか確認するための属性
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class MustItemAttribute : Attribute { }
