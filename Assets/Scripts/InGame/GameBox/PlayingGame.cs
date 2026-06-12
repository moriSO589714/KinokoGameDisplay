using System.Diagnostics;
using System.IO;

/// <summary>
/// ゲームの起動を行う
/// </summary>
public class PlayingGame
{
    public void OnPlayGame(GameData targetGameData)
    {
        AllDirs allDirs = AllDirs.GetInstance();
        //実行するファイル名を取得
        string gameExeName = targetGameData.GameExeName;
        //実行するファイルのパスを取得
        string filePath = Path.Combine(allDirs.GameFilePath, targetGameData.GameID, targetGameData.GameDirName, gameExeName);
        if (!File.Exists(filePath))
        {
            throw new System.Exception("実行するファイルが見つかりません Path>>>" + filePath);
        }

        //実行
        Process proc = new Process();
        proc.StartInfo.FileName = filePath;
        proc.Start();
    }
}
