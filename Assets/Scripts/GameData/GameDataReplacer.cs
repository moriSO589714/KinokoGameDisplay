using System.Reflection;

public static class GameDataReplacer
{
    public static GameData ComparisonAndReplace(GameData oldGameData, GameData newGameData)
    {
        FieldInfo[] gameDataFields = typeof(GameData).GetFields();
        foreach (FieldInfo field in gameDataFields)
        {
            var oldDataVal = field.GetValue(oldGameData);
            var newDataVal = field.GetValue(newGameData);
            if (oldDataVal != newDataVal)
            {
                //古い方のインスタンスのフィールドの値を新しいものに書き換える
                field.SetValue(oldGameData, newDataVal);
            }
        }

        return oldGameData;
    }
}