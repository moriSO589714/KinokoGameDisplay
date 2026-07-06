
using System.Collections.Generic;
using System.Linq;

public class SortLibrary
{
    //(GameDataクラスの変数名, ソート画面で表示する名前)
    public readonly Dictionary<string, string> CategoryDic = new Dictionary<string, string>() 
    {
        { "GameTitle", "タイトル名"},
        { "GameVersion", "ゲーム更新日"},
        { "GameID", "ゲームID"}
    };

    //ソート画面での表示名のみのリスト
    public List<string> CategoryNames => CategoryDic.Values.ToList();

    public readonly Dictionary<bool, string> AscOrDescDic = new Dictionary<bool, string>()
    {
        { true, "昇順"},
        { false, "降順" }
    };

    public List<string> AscAndDescNames => AscOrDescDic.Values.ToList();
}
