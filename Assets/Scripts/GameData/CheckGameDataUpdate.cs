using System.Reflection;

public static class CheckGameDataUpdate
{
    /// <summary>
    /// oldゲームデータとnewゲームデータを比較してアップデート部があるかどうかを確認。あるなら、それに伴った処理を実行する
    /// 2つの引数のGameDataは同じゲームを指す必要がある。(GameIDが同じ)
    /// </summary>
    /// <param name="oldGameData"></param>
    /// <param name="newGameData"></param>
    public static void CheckUpdate(GameData oldGameData, GameData newGameData)
    {
        bool isExistUpdate = false;
        long oldGameVersion = long.TryParse(oldGameData.GameVersion, out long result) ? result : 0;
        long newGameVersion = long.TryParse(oldGameData.GameVersion, out long result2) ? result2 : 0;

        //新しいほうのゲームのバージョンが新しくなっていた場合
        if(oldGameData.Status == GameStatus.Downloaded && oldGameVersion  < newGameVersion)
        {
            oldGameData.Status = GameStatus.UpdateAvailable;
        }

        //それ以外に異なるデータが存在する場合
        FieldInfo[] gameDataFields = typeof(GameData).GetFields();
        foreach(FieldInfo field in gameDataFields)
        {
            if (field.Name == "GameVersion") continue;
            if (field.Name == "Status") continue;
            if (field.Name == "GameDirName") continue;
            if (field.Name == "GameExeName") continue;            
            
            var oldDataValue = field.GetValue(oldGameData);
            var newDataValue = field.GetValue(newGameData);
            if(oldDataValue != newDataValue)
            {
                //古い方のインスタンスのフィールドの値を新しいものに書き換える
                field.SetValue(oldGameData, newDataValue);
                isExistUpdate = true;
            }
        }

        if (isExistUpdate && (oldGameData.Status == GameStatus.Downloaded || oldGameData.Status == GameStatus.UpdateAvailable))
        {
            AllDirs allDirs = AllDirs.GetInstance();
            GameData forWriteGameData = new GameData(oldGameData);
            forWriteGameData.Status = GameStatus.Downloaded;
            //jsonファイルを新しいものに更新する
            GameJsonUpdate.UpdateJson(forWriteGameData, allDirs);
        }
    }
}
