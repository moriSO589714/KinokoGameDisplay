using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// ゲームの詳細情報が入るクラス
/// </summary>
[Serializable]
[System.Diagnostics.DebuggerDisplay(
    "ゲームタイトル：{GameTitle}\nゲームのフォルダ名：{GameDirName}\nゲームの実行ファイルの名前：{GameExeName}\nゲームのjsonファイル名：{GameJsonName}\nゲームバージョン{GameVersion}" +
    "\nゲームの説明：{GameDescription}\nゲームの作者：{GameDevelopper}\nゲームツール：{GameSoftwareType}\nゲームのDriveID：{GameDriveId}\nゲームのサムネ画像のDriveID：{GameImageId}" +
    "\nゲームに付与されているタグ：{GameTags}\nゲームの状態：{Status}"
    )]
public class GameData
{
    public string GameTitle; //ゲームのタイトル
    public string GameDirName; //ゲームのフォルダ名
    public string GameExeName; //ゲームの実行ファイルの名前
    public string GameJsonName; //ゲームのjsonファイル名
    public string GameVersion; //ゲームのバージョン(2504011225の形)
    public string GameDescription; //ゲームの説明
    public string GameDevelopper; //ゲームの作者
    public string GameSoftwareType; //使われたゲーム開発環境
    public string GameDriveId; //ゲームのドライブID
    public string GameImageId; //ゲームのサムネのドライブID
    public string[] GameTags; //ゲームに付与されているタグ
    public GameStatus Status; //ゲームの状態(ダウンロードされているかなど)
}

/// <summary>
/// ゲームのステータスを表す
/// </summary>
public enum GameStatus
{
    NotDownloaded,
    Downloaded,
    UpdateAvailable //ダウンロードされているけど新しいバージョンがある
}
