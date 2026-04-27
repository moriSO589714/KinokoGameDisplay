using Cysharp.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using System.IO;

public class CreateAPIService
{
    string _jsonPathKey;
    public CreateAPIService(string jsonPathKey)
    {
        _jsonPathKey = jsonPathKey;
    }

    public SheetsService CreateSheetAPIService()
    {
        GoogleCredential credential;
        using (var stream = new FileStream(_jsonPathKey, FileMode.Open, FileAccess.Read))
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
        return null;
    }
}
