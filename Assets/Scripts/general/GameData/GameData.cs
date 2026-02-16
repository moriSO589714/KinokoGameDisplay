using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct GameData
{
    public string GameTitle; //ゲームのタイトル
    public string GameDirName; //ゲームのフォルダ名
    public string GameJsonName; //ゲームのjsonファイル名
    public string GameVersion; //ゲームのバージョン(2504011225の形)
    public string GameDescription; //ゲームの説明
    public string GameDevelopper; //ゲームの作者
    public string GameDriveId; //ゲームのドライブID
    public string GameImageId; //ゲームのサムネのドライブID
    public string[] GameTags; //ゲームに付与されているタグ
    public GameStatus Status; //ゲームの状態(ダウンロードされているかなど)
}

public enum GameStatus
{
    NotDownloaded,
    Downloaded,
    UpdateAvailable //ダウンロードされているけど新しいバージョンがある
}
