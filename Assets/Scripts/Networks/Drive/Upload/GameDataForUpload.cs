
using System.Linq;
using System.Reflection;

public static class GameDataForUpload
{
    /// <summary>
    /// ゲームのアップロードに際して最低限必要な情報がセットされているかを確認
    /// </summary>
    public static bool QualityCheck(GameData originData, string localGamePath)
    {
        //全ての項目を確認していく
        if(originData == null)
        {
            return false;
        }

        if(localGamePath == null || localGamePath == "")
        {
            return false;
        }

        if(originData.GameTitle == null || originData.GameTitle == "")
        {
            return false;
        }

        if(originData.GameExeName == null || originData.GameExeName == "")
        {
            return false;
        }

        if (originData.GameSoftwareType == null || originData.GameSoftwareType == "")
        {
            return false;
        }

        return true;
    }

    public static GameData CreateGameDataForUpload(GameData originData, string localGamePath, string localImagePath = "")
    {
        originData.GameDriveId = localGamePath;
        originData.GameImageId = localImagePath;

        ForceReplaceWord forceReplaceWord = new ForceReplaceWord();
        //文字の置き換えを行う
        originData.GameTitle = forceReplaceWord.ReplacedWord(originData.GameTitle);
        originData.GameDirName = forceReplaceWord.ReplacedWord(originData.GameDirName);
        originData.GameExeName = forceReplaceWord.ReplacedWord(originData.GameExeName);
        originData.GameDescription = forceReplaceWord.ReplacedWord(originData.GameDescription);
        for(int i = 0; i <= originData.GameDevelopper.Count() - 1; i++)
        {
            originData.GameDevelopper[i] = forceReplaceWord.ReplacedWord(originData.GameDevelopper[i]);
        }
        for (int i = 0; i <= originData.GameTags.Count() - 1; i++)
        {
            originData.GameTags[i] = forceReplaceWord.ReplacedWord(originData.GameTags[i]);
        }
        return originData;
    }
}