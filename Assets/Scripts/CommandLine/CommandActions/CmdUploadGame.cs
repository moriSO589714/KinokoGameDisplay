using Cysharp.Threading.Tasks;
using Google.Apis.Drive.v3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditor.VersionControl;
using UnityEngine;

public class CmdUploadGame : MonoBehaviour
{
    CmdSceneManager _cmdSceneManager;

    string _returnWord = "return";
    string _uploadWord = "upload";

    GameData _gameDataForUpload = null;
    string _localGamePath = null;
    string _localImagePath = null;

    bool _isUploadAvailable = false;

    OnNetDriveUploadFile _onNetDriveUploadFile;
    OnNetCreateFolder _onNetCreateFolder;
    OnNetGetParentId _onNetGetParentId;
    OnNetDriveGetName _onNetDriveGetName;
    OnNetDelete _onNetDelete;

    WordEmtCell _categoryWec;
    WordEmtCell _tagsLib;
    WordEmtCell _devsLib;
    WordEmtCell _toolsLib;

    public void FirstCmdCall()
    {
        if (_cmdSceneManager == null) _cmdSceneManager = CmdSceneManager.Instance;
        _cmdSceneManager.OutPutManager.ReceiveMessage("アップロードモードに変更します", OutPutTextLogColorSets.SystemDefault);
        _cmdSceneManager.InputFieldManager._endModeAction = End;
        Init();
        //スプシのロード中にコマンドの受付を行わないようにしておく
        _cmdSceneManager.InputFieldManager.ChangeAction(new CmdNothing().MessageGird);
        CancellationTokenSource cts = new CancellationTokenSource();
        //exitコマンドなどが入力された場合はunitaskの処理をキャンセルさせる
        _cmdSceneManager.InputFieldManager._endModeAction += () => { cts.Cancel(); };

        //実行
        LoadSpreadSheet(cts.Token);
    }

    public async UniTask LoadSpreadSheet(CancellationToken cts)
    {
        GameDataManager gameDataManager = new GameDataManager();

        string connectInternetLog = "インターネットに接続して、現在登録されているゲーム情報を取得しています";
        string messageId = _cmdSceneManager.OutPutManager.ReceiveMessage(connectInternetLog, OutPutTextLogColorSets.SystemDefault);
        CancellationTokenSource ctsForLogAnim = new CancellationTokenSource();
        new CmdWaitingAnimInLog().LoopWaitingLog(connectInternetLog, OutPutTextLogColorSets.SystemDefault, messageId, ctsForLogAnim.Token);
        
        try
        {
            await UniTask.RunOnThreadPool(gameDataManager.LoadGameDataFromSpSt);
        }
        catch(Exception e)
        {
            ctsForLogAnim.Cancel();
            _cmdSceneManager.OutPutManager.ReceiveMessage("ゲームデータの取得に失敗しました。", OutPutTextLogColorSets.SystemDefault);
            ReturnCmdReceive();
            return;
        }
        ctsForLogAnim.Cancel();

        //各項目のwecを取得する
        GameDatasSingleton gameDatasSingleton = GameDatasSingleton.Instance;
        _tagsLib = gameDatasSingleton.ReturnTagsLib();
        _devsLib = gameDatasSingleton.ReturnDeveroppersLib();
        _toolsLib = gameDatasSingleton.ReturnToolsLib();

        //処理にキャンセルが入っていた場合
        if (cts.IsCancellationRequested)
        {           
            return;
        }

        _cmdSceneManager.OutPutManager.ReceiveMessage("接続成功。初期処理を実行中", OutPutTextLogColorSets.SystemDefault);

        CmdUploadModeEntrance();
    }

    private void CmdUploadModeEntrance()
    {

        _cmdSceneManager.InputFieldManager.ChangeAction(SwitchInputContent, _categoryWec);
        
        _cmdSceneManager.OutPutManager.ReceiveMessage
            ($"設定する項目名を送信してください。(return で1つ前に戻れます)" +
            $"\n・{CmdUploadContent.title}:{_gameDataForUpload?.GameTitle}" +
            $"\n・{CmdUploadContent.description}:{_gameDataForUpload?.GameDescription}" +
            $"\n・{CmdUploadContent.folderpath}:{_localGamePath ?? ""}" +
            $"\n・{CmdUploadContent.exepath}:{_gameDataForUpload?.GameExeName}" +
            $"\n・{CmdUploadContent.imagepath}:{_localImagePath ?? ""}" +
            $"\n・{CmdUploadContent.deveroppers}:{MergeArray(_gameDataForUpload?.GameDevelopper)}" +
            $"\n・{CmdUploadContent.softwaretype}:{_gameDataForUpload?.GameSoftwareType}" +
            $"\n・{CmdUploadContent.tags}:{MergeArray(_gameDataForUpload?.GameTags)}" , OutPutTextLogColorSets.SystemDefault);

        //アップロードに必要なデータが最低限セットされているかを確認する
        if(GameDataForUpload.QualityCheck(_gameDataForUpload, _localGamePath))
        {
            _isUploadAvailable = true;
            _cmdSceneManager.OutPutManager.ReceiveMessage
                ($"※※アップロードが行えます。アップロードを実行する場合は「{_uploadWord}」を送信してください※※", OutPutTextLogColorSets.AccentDefault);         
        }
    }

    private void ReturnCmdReceive()
    {
        _cmdSceneManager.OutPutManager.ReceiveMessage("コマンド受付モードに戻ります", OutPutTextLogColorSets.SystemDefault);
        //コマンド受付に戻す
        _cmdSceneManager.InputFieldManager.ReturnCommandReceive();
    }

    private void SwitchInputContent(string message)
    {
        if(message == _returnWord)
        {
            ReturnCmdReceive();
            return;
        }

        if(message == _uploadWord && _isUploadAvailable)
        {
            //アップロード開始のメソッド(MessageGirdに入力先を変えておく)
            return;
        }

        if(!Enum.TryParse<CmdUploadContent>(message, out var content))
        {
            _cmdSceneManager.OutPutManager.ReceiveMessage("送信された項目は存在しません", OutPutTextLogColorSets.AccentDefault);
            return;
        }

        switch (content) 
        {
            case CmdUploadContent.title:
                _cmdSceneManager.OutPutManager.ReceiveMessage("タイトル名を送信してください", OutPutTextLogColorSets.SystemDefault);
                _cmdSceneManager.InputFieldManager.ChangeAction(ReceiveTitle);
                break;
            case CmdUploadContent.description:
                _cmdSceneManager.OutPutManager.ReceiveMessage("ゲームの説明を送信してください。( *!* で改行することができます)", OutPutTextLogColorSets.SystemDefault);
                _cmdSceneManager.InputFieldManager.ChangeAction(ReceiveDescription);
                break;
            case CmdUploadContent.folderpath:
                break;
            case CmdUploadContent.exepath:
                break;
            case CmdUploadContent.imagepath:
                break;
            case CmdUploadContent.deveroppers:
                _cmdSceneManager.OutPutManager.ReceiveMessage("ゲームの開発者名を送信してください。(複数送信可)\n既に送信した開発者名を再度送信することで取り消しが可能です", OutPutTextLogColorSets.SystemDefault);
                _cmdSceneManager.InputFieldManager.ChangeAction(ReceiveAddDeveroppers, _devsLib);
                break;
            case CmdUploadContent.softwaretype:
                _cmdSceneManager.OutPutManager.ReceiveMessage("使用したツール・ソフトウェアを送信してください。", OutPutTextLogColorSets.SystemDefault);
                _cmdSceneManager.InputFieldManager.ChangeAction(ReceiveTool, _toolsLib);
                break;
            case CmdUploadContent.tags:
                _cmdSceneManager.OutPutManager.ReceiveMessage("追加するタグを送信してください。(複数送信可)\n既に送信した開発者名を再度送信することで取り消しが可能です。", OutPutTextLogColorSets.SystemDefault);
                _cmdSceneManager.InputFieldManager.ChangeAction(ReceiveAddTags, _tagsLib);
                break;
        }
    }

    private void Init()
    {
        _gameDataForUpload = new GameData();
        if (CheckInEnvironment.isOnNet)
        {
            DriveService driveService = NetworksSingleton.Instance.ReturnDriveService();
            _onNetDriveUploadFile = new OnNetDriveUploadFileforDv(driveService);
            _onNetCreateFolder = new OnNetCreateFolderforDv(driveService);
            _onNetGetParentId = new OnNetGetParentIdfromDv(driveService);
            _onNetDriveGetName = new OnNetDriveGetNamefromDv(driveService);
            _onNetDelete = new OnNetDeleteforDv(driveService);
        }
        else
        {
            _onNetDriveUploadFile = new OnNetDriveUploadFileforTest();
            _onNetCreateFolder = new OnNetCreateFolderforTest();
            _onNetGetParentId = new OnNetGetParentIdfromTest();
            _onNetDriveGetName = new OnNetDriveGetNamefromTest();
            _onNetDelete = new OnNetDeleteforTest();
        }

        _categoryWec = WECLibCreater.CreateLibFromLineAndPriority
            (new Dictionary<string, int> {
                { CmdUploadContent.tags.ToString(), 0},
                { CmdUploadContent.softwaretype.ToString(), 1},
                { CmdUploadContent.deveroppers.ToString(), 2},
                { CmdUploadContent.imagepath.ToString(), 3 },
                { CmdUploadContent.exepath.ToString(), 4 },
                { CmdUploadContent.folderpath.ToString(), 5},
                { CmdUploadContent.description.ToString(), 6},
                { CmdUploadContent.title.ToString(), 7}
        });
    }

    private void End()
    {
        _gameDataForUpload = null;
        _localGamePath = null;
        _localImagePath = null;
    }

    private string MergeArray(string[] array)
    {
        if(array == null || array.Count() == 0)
        {
            return "";
        }

        return String.Join(",", array);
    }

    //各項目の登録用関数
    //=======================================================================================================================================================
    private void ReceiveTitle(string message)
    {
        CheckMessageAndRegisterSingle(message,
            registerVal => { _gameDataForUpload.GameTitle = registerVal; }, $"ゲームタイトルを「{message}」で登録しました");
    }

    private void ReceiveDescription(string message)
    {
        CheckMessageAndRegisterSingle(message,
            registerVal => { _gameDataForUpload.GameDescription = registerVal; }, $"ゲーム説明を登録しました");
    }

    private void ReceiveTool(string message)
    {
        CheckMessageAndRegisterSingle(message,
            registeVal => { _gameDataForUpload.GameSoftwareType = registeVal; }, $"ツール・ソフトウェアを「{message}」で登録しました");
    }

    private void ReceiveAddDeveroppers(string message)
    {
        CheckMessageAndRegisterArray(message, _gameDataForUpload.GameDevelopper,
            registerVal => { _gameDataForUpload.GameDevelopper = registerVal; }, "開発者");
    }

    private void ReceiveAddTags(string message)
    {
        CheckMessageAndRegisterArray(message, _gameDataForUpload.GameTags,
            registerVal => { _gameDataForUpload.GameTags = registerVal; }, "タグ");
    }

    //=======================================================================================================================================================
    
    private bool JudgementReturn(string message)
    {
        if(message == _returnWord)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private string CheckErrorWordInMessage(string inputMessage)
    {
        ForceReplaceWord forceReplaceWord = new ForceReplaceWord();
        string containsErrorWord = "";
        foreach(string word in forceReplaceWord.UnAvailableWordsList)
        {
            if (inputMessage.Contains(word))
            {
                containsErrorWord = word;
                break;
            }
        }

        if(containsErrorWord != "")
        {
            return containsErrorWord;
        }

        return "";
    }

    private void CheckMessageAndRegisterSingle(string inputMessage, Action<string> registerAct, string successMessage)
    {
        if (JudgementReturn(inputMessage))
        {
            CmdUploadModeEntrance();
            return;
        }

        string errorWord = CheckErrorWordInMessage(inputMessage);
        if(errorWord != "")
        {
            _cmdSceneManager.OutPutManager.ReceiveMessage($"不正な文字が含まれています。送信し直してください。不正文字>>>{errorWord}", OutPutTextLogColorSets.AccentDefault);
        }

        //GameDataインスタンスの特定のフィールドに登録
        registerAct(inputMessage);
        _cmdSceneManager.OutPutManager.ReceiveMessage(successMessage, OutPutTextLogColorSets.SystemDefault);
        CmdUploadModeEntrance();
    }

    private void CheckMessageAndRegisterArray(string inputMessage, string[] formerArray, Action<string[]> registerAct, string itemName)
    {
        if (JudgementReturn(inputMessage))
        {
            CmdUploadModeEntrance();
            return;
        }

        string errorWord = CheckErrorWordInMessage(inputMessage);
        if(errorWord != "")
        {
            _cmdSceneManager.OutPutManager.ReceiveMessage($"不正な文字が含まれています。送信し直してください。不正文字>>>{errorWord}", OutPutTextLogColorSets.AccentDefault);
        }

        if(formerArray != null && formerArray.Contains(inputMessage))
        {
            List<string> newList = formerArray.ToList();
            newList.Remove(inputMessage);
            registerAct(newList.ToArray());
            _cmdSceneManager.OutPutManager.ReceiveMessage($"{itemName}を削除しました。", OutPutTextLogColorSets.SystemDefault);
            return;
        }

        List<string> renewList = new List<string>();
        if(formerArray != null)
        {
            renewList = formerArray.ToList();
        }
        renewList.Add(inputMessage);
        registerAct(renewList.ToArray());
        _cmdSceneManager.OutPutManager.ReceiveMessage($"{itemName}を登録しました。続けて登録可能です。項目選択に戻る場合は「{_returnWord}」を送信してください", OutPutTextLogColorSets.SystemDefault);
        return;
    }
}

public enum CmdUploadContent 
{
    title,
    description,
    folderpath,
    exepath,
    imagepath,
    deveroppers,
    softwaretype,
    tags,
}