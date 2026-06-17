using Cysharp.Threading.Tasks;
using System.Threading;

public class GameDlErrorRecovery
{

    public async UniTask RecoveryError(GameDlError gameDlError, CancellationToken ct)
    {
        await UniTask.RunOnThreadPool(() => { Recovery(gameDlError);}, cancellationToken: ct);
    }

    private void Recovery(GameDlError gameDlError)
    {
        AllDirs allDirs = AllDirs.GetInstance();
        GameDlCustomException gameDlCustomException = gameDlError.DlException;
        //ダウンロードしようとしているゲームのGamaDataインスタンスを取ってくる
        GameData targetGameData = gameDlError.Task.TaskInstance.GameData;

        if(gameDlCustomException.GameDlErrorType == GameDlErrorType.NeedCleanDirectory)
        {
            //ゲーム本体側のアンインストール処理を試す(jsonファイルは欠損しているが、ゲーム自体はディレクトリにある場合などに有効)
            new GameDeleteManager().UninstallGame(targetGameData);
            //エラーが発生しているゲームの一時保存フォルダの削除を試す
            string tempGamePath = CreateDirPath.TempGamePathForDl(tempDirPath: allDirs.TmpDLPath, targetGameData.GameID);
            DirectoryActs.CompleteDirDelete(tempGamePath);
        }
        else if(gameDlCustomException.GameDlErrorType == GameDlErrorType.NeedRetryAccessDrive)
        {
            //時間経過で治る可能性が高いため何もせずに返す
            return;
        }
        else
        {
            throw new System.Exception("このタスクはリカバリが不可能です");
        }
    }
}
