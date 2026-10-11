using System.Collections.Generic;
using System.IO;

public class GameUpdateReceiveProc : GameDlProc
{
    public GameUpdateReceiveProc(OnNetDriveMetaData metaData, OnNetDriveGetFile onNetDriveFetFile, GameData gameData) : base(metaData, onNetDriveFetFile, gameData)
    {
        GameData = ReplaceLastGameData(gameData);
    }

    protected override LastGameDLState CheckLastStatus(string gameID, string tempSliceGamePath)
    {
        //もしtemppSliceGamePathに前回の一時ファイルが残っていたら削除する
        string[] files = Directory.GetFiles(tempSliceGamePath);
        foreach(string path in files)
        {
            File.Delete(path);
        }

        return LastGameDLState.None;
    }

    protected override void MoveActualFolder(string tempGameDLPath, string newGamePath, string gameId)
    {
        //現在ダウンロードされているゲームデータ群(ゲーム・画像・json)を削除する
        new GameDeleteManager().UninstallGame(GameData);

        base.MoveActualFolder(tempGameDLPath, newGamePath, gameId);
    }

    /// <summary>
    /// ローカルに存在しているゲームデータを最新のスプレッドシートに保存されているゲームデータに交換する
    /// </summary>
    private GameData ReplaceLastGameData(GameData localGameData)
    {
        CollectivelyGetFromSpSt collectivelyGetFromSpSt = new CollectivelyGetFromSpSt();
        List<GameData> replacedGameDatas = collectivelyGetFromSpSt.AllGameDataFromSpSt(false);

        foreach(GameData replace in replacedGameDatas)
        {
            if(replace.GameID == localGameData.GameID)
            {
                return replace;
            }
        }

        throw new System.Exception($"スプレッドシート上に対象のゲームが存在しません。 GameID>>{localGameData.GameID}");
    }
}