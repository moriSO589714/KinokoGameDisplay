using SFB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class CmdGenericInfoInputOfAddGame
{
    private readonly string _decisionWord = "upload";
    public readonly string _imageExtension = "png";

    private bool _allowDecision = false;

    private GameData _currentGameData = null;
    private string _userGamePath = "";
    private string _userImagePath = "";
    private Action<GameData, string, string> _sendGameDataAct = null;
    private CmdSceneManager _sceneManager = null;

    WordEmtCell _categoryWec;
    WordEmtCell _tagsLib;
    WordEmtCell _devsLib;
    WordEmtCell _toolsLib;

    public CmdGenericInfoInputOfAddGame(Action<GameData, string, string> sendGameDataAct)
    {
        _sendGameDataAct = sendGameDataAct;
        Init();
    }

    public CmdGenericInfoInputOfAddGame(){}

    private void Init()
    {
        _currentGameData = new GameData();
        _currentGameData.GameDevelopper = new string[0];
        _currentGameData.GameTags = new string[0];
        _sceneManager = CmdSceneManager.Instance;

        //各項目のwecを取得する
        GameDatasSingleton gameDatasSingleton = GameDatasSingleton.Instance;
        List<GameData> gameDatas = gameDatasSingleton.AllGameDatas;

        _tagsLib = CreateLibFromGameDatas.CreateTagsLib(gameDatas);
        _devsLib = CreateLibFromGameDatas.CreateDeveropperLib(gameDatas);
        _toolsLib = CreateLibFromGameDatas.CreateToolsLib(gameDatas);

        _categoryWec = WECLibCreater.CreateLibFromLineAndPriority
            (new Dictionary<string, int> {
                        { CmdAddGameCategory.tags.ToString(), 0},
                        { CmdAddGameCategory.softwaretype.ToString(), 1},
                        { CmdAddGameCategory.deveroppers.ToString(), 2},
                        { CmdAddGameCategory.imagepath.ToString(), 3 },
                        { CmdAddGameCategory.exepath.ToString(), 4 },
                        { CmdAddGameCategory.folderpath.ToString(), 5},
                        { CmdAddGameCategory.description.ToString(), 6},
                        { CmdAddGameCategory.title.ToString(), 7}
        });
    }

    private void End()
    {
        _sendGameDataAct = null;
        _allowDecision = false;
    }

    public void StartInputData(CmdReturn returnCmdReceive)
    {
        _sceneManager.InputFieldManager.ChangeAction((string message) => SwitchInputContent(message, returnCmdReceive), _categoryWec);
        _sceneManager.OutPutManager.SendMessage
            ($"設定する項目名を送信してください。({returnCmdReceive.ReturnWord}で1つ前に戻れます)" +
            $"\n・{CmdAddGameCategory.title}:{_currentGameData?.GameTitle}" +
            $"\n・{CmdAddGameCategory.description}:{_currentGameData?.GameDescription}" +
            $"\n・{CmdAddGameCategory.folderpath}:{_userGamePath ?? ""}" +
            $"\n・{CmdAddGameCategory.exepath}:{_currentGameData?.GameExeName}" +
            $"\n・{CmdAddGameCategory.imagepath}:{_userImagePath ?? ""}" +
            $"\n・{CmdAddGameCategory.deveroppers}:{MergeArray(_currentGameData?.GameDevelopper)}" +
            $"\n・{CmdAddGameCategory.softwaretype}:{_currentGameData?.GameSoftwareType}" +
            $"\n・{CmdAddGameCategory.tags}:{MergeArray(_currentGameData?.GameTags)}", OutPutTextLogColorSets.SystemDefault
            );

        //アップロードに必要なデータが最低限セットされているかを確認する
        if (GameDataForUpload.QualityCheck(_currentGameData, _userGamePath))
        {
            _allowDecision = true;
            _sceneManager.OutPutManager.SendMessage
                ($"※※アップロードが行えます。アップロードを実行する場合は「{_decisionWord}」を送信してください※※", OutPutTextLogColorSets.Blue);
        }
    }

    private void SwitchInputContent(string message, CmdReturn returnReceiveCmd)
    {
        if (returnReceiveCmd.ReturnCheck(message))
        {
            return;
        }

        if (message == _decisionWord && _allowDecision)
        {
            _sendGameDataAct.Invoke(_currentGameData, _userGamePath, _userImagePath);
            End();
            return;
        }

        if (!Enum.TryParse<CmdAddGameCategory>(message, out var content))
        {
            _sceneManager.OutPutManager.SendMessage("送信された項目は存在しません", OutPutTextLogColorSets.AccentDefault);
            return;
        }

        CmdReturn returnStartInputData = new CmdReturn(() => StartInputData(returnReceiveCmd));

        switch (content) 
        {
            case CmdAddGameCategory.title:
                _sceneManager.OutPutManager.SendMessage("タイトル名を送信してください", OutPutTextLogColorSets.SystemDefault);
                _sceneManager.InputFieldManager.ChangeAction((string message) => ReceiveTitle(message, returnStartInputData));
                break;
            case CmdAddGameCategory.description:
                _sceneManager.OutPutManager.SendMessage("ゲームの説明を送信してください", OutPutTextLogColorSets.SystemDefault);
                _sceneManager.InputFieldManager.ChangeAction((string message) => ReceiveDescription(message, returnStartInputData));
                break;
            case CmdAddGameCategory.folderpath:
                _sceneManager.OutPutManager.SendMessage("ゲームが入っているフォルダのパスを送信してください", OutPutTextLogColorSets.SystemDefault);
                try
                {
                    string selectedPath = new OpenFilePanel().OpenFolderPanelAndReturnPath();
                    if (selectedPath != null) _sceneManager.InputFieldManager.ChangeInputfieldVal(selectedPath);
                    _sceneManager.InputFieldManager.ChangeAction((string message) => ReceiveGameFolderPath(message, returnStartInputData));
                }
                catch(System.Exception e)
                {
                    _sceneManager.OutPutManager.SendMessage("エラーが発生しました。項目名から再送信してください", OutPutTextLogColorSets.AccentDefault);
                    Debug.LogException(e);
                }
                break;
            case CmdAddGameCategory.exepath:
                _sceneManager.OutPutManager.SendMessage("ゲームが入っているフォルダのパスを送信してください", OutPutTextLogColorSets.SystemDefault);
                try
                {
                    ExtensionFilter filter = new ExtensionFilter("All File", "*");
                    string selectedPath = new OpenFilePanel().OpenFilePanelAndReturnPath(new ExtensionFilter[1] { filter });
                    if (selectedPath != null) _sceneManager.InputFieldManager.ChangeInputfieldVal(selectedPath);
                    _sceneManager.InputFieldManager.ChangeAction((string message) => ReceiveGameExePath(message, returnStartInputData));
                }
                catch(System.Exception e)
                {
                    _sceneManager.OutPutManager.SendMessage("エラーが発生しました。項目名から再送信してください", OutPutTextLogColorSets.AccentDefault);
                    Debug.LogError(e);
                }
                break;
            case CmdAddGameCategory.imagepath:
                _sceneManager.OutPutManager.SendMessage("サムネイル画像のパスを送信してください", OutPutTextLogColorSets.SystemDefault);
                try
                {
                    ExtensionFilter filter = new ExtensionFilter("Image Path", _imageExtension);
                    string selectedPath = new OpenFilePanel().OpenFilePanelAndReturnPath(new ExtensionFilter[1] { filter });
                    if (selectedPath != null) _sceneManager.InputFieldManager.ChangeInputfieldVal(selectedPath);
                    _sceneManager.InputFieldManager.ChangeAction((string message) => ReceiveImagePath(message, returnStartInputData));
                }
                catch(System.Exception e)
                {
                    _sceneManager.OutPutManager.SendMessage("エラーが発生しました。項目名から再送信してください", OutPutTextLogColorSets.AccentDefault);
                    Debug.LogError(e);
                }
                break;
            case CmdAddGameCategory.deveroppers:
                _sceneManager.OutPutManager.SendMessage("ゲームの開発者名を送信してください", OutPutTextLogColorSets.SystemDefault);
                _sceneManager.InputFieldManager.ChangeAction((string message) => ReceiveAddDeveroppers(message, returnStartInputData), _devsLib);
                break;
            case CmdAddGameCategory.softwaretype:
                _sceneManager.OutPutManager.SendMessage("使用したツール・ソフトウェアを送信してください", OutPutTextLogColorSets.SystemDefault);
                _sceneManager.InputFieldManager.ChangeAction((string message) => ReceiveTool(message, returnStartInputData), _toolsLib);
                break;
            case CmdAddGameCategory.tags:
                _sceneManager.OutPutManager.SendMessage("追加するタグを送信してください", OutPutTextLogColorSets.SystemDefault);
                _sceneManager.InputFieldManager.ChangeAction((string message) => ReceiveAddTags(message, returnStartInputData), _tagsLib);
                break;

        }

    }

    private string MergeArray(string[] array)
    {
        if(array == null || array.Count() == 0)
        {
            return "";
        }

        return String.Join(",", array);
    }

    //各項目登録用の関数
    //=====================================================================================================================================
    private void ReceiveTitle(string message, CmdReturn returnStartInputData)
    {
        if (returnStartInputData.ReturnCheck(message)) return;

        var systemReply = CmdRegister.RegisterSingleCategory(message,
            registerVal => { _currentGameData.GameTitle = registerVal; });

        _sceneManager.OutPutManager.SendMessage(systemReply.replyMessage, systemReply.logColor);
        returnStartInputData.ForceDoingAction();
    }

    private void ReceiveDescription(string message, CmdReturn returnStartInputData)
    {
        if (returnStartInputData.ReturnCheck(message)) return;

        var systemReply = CmdRegister.RegisterSingleCategory(message,
                registerVal => { _currentGameData.GameDescription = registerVal; });

        _sceneManager.OutPutManager.SendMessage(systemReply.replyMessage, systemReply.logColor);
        returnStartInputData.ForceDoingAction();
    }

    private void ReceiveTool(string message, CmdReturn returnStartInputData)
    {
        if (returnStartInputData.ReturnCheck(message)) return;

        var systemReply = CmdRegister.RegisterSingleCategory(message,
                registerVal => { _currentGameData.GameSoftwareType = registerVal; });

        _sceneManager.OutPutManager.SendMessage(systemReply.replyMessage, systemReply.logColor);
    }

    private void ReceiveAddDeveroppers(string message, CmdReturn returnStartInputData)
    {
        if (returnStartInputData.ReturnCheck(message)) return;

        var systemReply = CmdRegister.RegisterArrayCategory(message, _currentGameData.GameDevelopper.ToList(),
                registerVal => { _currentGameData.GameDevelopper = registerVal.ToArray(); });

        _sceneManager.OutPutManager.SendMessage(systemReply.replyMessage, systemReply.logColor);
    }

    private void ReceiveAddTags(string message, CmdReturn returnStartInputData)
    {
        if (returnStartInputData.ReturnCheck(message)) return;

        var systemReply = CmdRegister.RegisterArrayCategory(message, _currentGameData.GameTags.ToList(),
                registerVal => { _currentGameData.GameTags = registerVal.ToArray(); });

        _sceneManager.OutPutManager.SendMessage(systemReply.replyMessage, systemReply.logColor);
    }

    private void ReceiveGameFolderPath(string message, CmdReturn returnStartInputData)
    {
        if (returnStartInputData.ReturnCheck(message)) return;

        var systemReply = CmdRegister.RegisterDirPath(message,
                registerVal => { _userGamePath = registerVal; });

        if(systemReply.logColor == OutPutTextLogColorSets.SystemDefault)
        {
            _currentGameData.GameDirName = Path.GetFileName(_userGamePath);
            _currentGameData.GameExeName = "";
        }

        _sceneManager.OutPutManager.SendMessage(systemReply.replyMessage, systemReply.logColor);
        returnStartInputData.ForceDoingAction();
    }

    private void ReceiveGameExePath(string message, CmdReturn returnStartInputData)
    {
        if (returnStartInputData.ReturnCheck(message)) return;

        var systemReply = CmdRegister.RegisterExePath(message, _userGamePath,
                registerVal => _currentGameData.GameExeName = registerVal);

        _sceneManager.OutPutManager.SendMessage(systemReply.replyMessage, systemReply.logColor);
        returnStartInputData.ForceDoingAction();
    }

    private void ReceiveImagePath(string message, CmdReturn returnStartInputData)
    {
        if (returnStartInputData.ReturnCheck(message)) return;

        var systemReply = CmdRegister.RegisterFilePath(message,
                registerVal => _userImagePath = registerVal);

        _sceneManager.OutPutManager.SendMessage(systemReply.replyMessage, systemReply.logColor);
        returnStartInputData.ForceDoingAction();
    }
    //=====================================================================================================================================
}

public enum CmdAddGameCategory
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
