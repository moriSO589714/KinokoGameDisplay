using Cysharp.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using System.Diagnostics;
using System.IO;
using System.Text;

public class CreateAPIService
{
    //string型のjson形式のパスキー
    string _jsonPathKey;
    public CreateAPIService(string jsonPathKey)
    {
        if (File.Exists(jsonPathKey))
        {
            //復号化を行う
            try
            {
                string key = new PathKeyManager().GetKeyFromCipherTxtPath(jsonPathKey);
                _jsonPathKey = key;
            }
            catch(System.Exception e)
            {
                UnityEngine.Debug.LogError(e);
                throw new System.Exception("アクティベーションコードが不正です。正しいコードを再登録してください");
            }
        }
        else
        {
            throw new System.Exception("通信を行うためのアクティベーションコードが登録されていません");
        }
    }

    public SheetsService CreateSheetAPIService()
    {
        GoogleCredential credential;

        //string型のパスキーをメモリ上に置き、ストリームを利用して認証情報を作成する
        byte[] pathKeyByte = Encoding.UTF8.GetBytes(_jsonPathKey);
        using (var stream = new MemoryStream(pathKeyByte))
        {
            credential = GoogleCredential.FromStream(stream).CreateScoped(SheetsService.ScopeConstants.Spreadsheets);
        }

        SheetsService sheetService = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "Spread Sheet",
        });

        return sheetService;
    }

    public DriveService CreateDriveAPIService()
    {
        GoogleCredential credential;

        //string型のパスキーをメモリ上に置き、ストリームを利用して認証情報を作成する
        byte[] pathKeyByte = Encoding.UTF8.GetBytes(_jsonPathKey);
        using (var stream = new MemoryStream(pathKeyByte))
        {
            credential = GoogleCredential.FromStream(stream).CreateScoped(DriveService.ScopeConstants.Drive);
        }

        DriveService service = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "DriveService"
        });
        return service;
    }
}
