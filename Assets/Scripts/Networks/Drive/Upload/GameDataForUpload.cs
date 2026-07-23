
public static class GameDataForUpload
{
    public static GameData CreateGameDataForUpload(GameData originData, string localGamePath, string localImagePath = "")
    {
        originData.GameDriveId = localGamePath;
        originData.GameImageId = localImagePath;
        return originData;
    }
}
