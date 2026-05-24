using Google.Apis.Drive.v3;
using System.Collections.Generic;

public class OnNetDriveMetaDatafromDv : OnNetDriveMetaData
{
    private DriveService _driveService;

    public OnNetDriveMetaDatafromDv(DriveService driveService)
    {
        _driveService = driveService;
    }

    /// <summary>
    /// 特定のドライブフォルダ下にあるファイルのidと名前を取得する
    /// </summary>
    /// <returns>(ファイル名, ドライブID)のディクショナリ</returns>
    public Dictionary<string, string> GetFileList(string driveFolderId)
    {
        //リクエストの作成
        var request = _driveService.Files.List();
        //クエリパラメータの指定
        request.Q = "'" + driveFolderId + "' in parents";
        request.Fields = "nextPageToken, files(id,name)"; //nextPageTokenは1度の取得上限に達した際、次の検索に利用する

        var files = new List<Google.Apis.Drive.v3.Data.File>();
        do
        {
            var result = request.Execute();
            files.AddRange(result.Files);
            request.PageToken = result.NextPageToken;
        } while (!string.IsNullOrEmpty(request.PageToken));

        Dictionary<string, string> jsonDatas = new Dictionary<string, string>();
        //Drive APIのFile形式から(string,string)のディクショナリ形式に変換する
        foreach(var file in files)
        {
            jsonDatas.Add(file.Name, file.Id);
        }
        return jsonDatas;
    }
}