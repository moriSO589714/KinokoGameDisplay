using UnityEngine;

/// <summary>
/// 自由に変更可能なディレクトリーのパスを代入するためのクラス
/// </summary>
public class FlexibleDirs
{
    //ゲームが保存されるディレクトリ
    public string GameFilePath = "dd\\GFP";
    //jsonFileが保存されるディレクトリ
    public string JsonsDirPath = "dd\\JP";
    //イメージ写真が保存されるディレクトリ
    public string ImageFolderPath = "dd\\IFP";
    //ゲームデータが保存されているドライブのID(最も上層)
    public string GameSavedDriveID;
    //スプレッドシートのID
    public string SpreadSheetID;
    //スプレッドシートでデータテーブルの範囲が始まるセルの場所(列,行)
    public Vector2 SpreadSheetStartCellPos;
    //スプレッドシートで項目名が書かれた行が始まるセルの場所(列,行)
    public Vector2 SpStElementStartCellPos;
}