using Google.Apis.Drive.v3;
using System.Collections.Generic;

public class OnNetGetParentIdfromDv : OnNetGetParentId
{
    private DriveService _driveService;

    public OnNetGetParentIdfromDv(DriveService driveService)
    {
        _driveService = driveService;
    }

    public string GetParentId(string childDriveId)
    {
        var request = _driveService.Files.Get(childDriveId);
        //フィールドパラメータに親を取得することを指定
        request.Fields = "parents";

        List<string> parentId = new List<string>();
        try
        {
            var result = request.Execute();
            //親のドライブIDをstrignのリストとして取得
            parentId = result.Parents as List<string>;
        }
        catch (Google.GoogleApiException ex)
        {
            throw new System.Exception("ドライブ上の親ファイルのid取得に失敗しました", ex);
        }

        //現行(2026)のドライブではほとんどの場合親フォルダは一つになるため、一つ目の要素を親フォルダのIDとして返す
        if (parentId.Count == 1)
        {
            return parentId[0];
        }
        else
        {
            throw new System.Exception("指定されたドライブのフォルダの親フォルダは複数存在するか、存在しません");
        }
    }
}
