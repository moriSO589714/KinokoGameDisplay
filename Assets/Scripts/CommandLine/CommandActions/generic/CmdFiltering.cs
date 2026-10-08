using Google.Apis.Drive.v3.Data;
using Ookii.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;

public class CmdFiltering
{
    private FilterCondition _currentFilterCondition;
    private CmdSceneManager _cmdSceneManager;
    private string _decisionWord = "decision";

    private Action<FilterCondition> _sendCondition;

    WordEmtCell _filteringCategoryWec;
    WordEmtCell _titleWec;
    WordEmtCell _tagsWec;
    WordEmtCell _devsWec;
    WordEmtCell _idsWec;
    WordEmtCell _toolsWec;

    public CmdFiltering(Action<FilterCondition> conditionSend)
    {
        Init();
        _sendCondition = conditionSend;
    }

    private void Init()
    {
        if(_currentFilterCondition == null)
        {
            _currentFilterCondition = new FilterCondition();
        }
        _cmdSceneManager = CmdSceneManager.Instance;

        _currentFilterCondition.GameNames.Add(new List<string>());
        _currentFilterCondition.GameTags.Add(new List<string>());
        _currentFilterCondition.GameDevs.Add(new List<string>());

        GameDatasSingleton gameDatasSingleton = GameDatasSingleton.Instance;
        List<GameData> allGameDatas = gameDatasSingleton.AllGameDatas;

        _filteringCategoryWec = WECLibCreater.CreateLibFromStrList(_currentFilterCondition._filteringCategory.ToList(), "");
        _titleWec = WECLibCreater.CreateLibFromStrList(allGameDatas.Select(x => x.GameTitle).ToList(), "");
        _tagsWec = CreateLibFromGameDatas.CreateTagsLib(allGameDatas);
        _devsWec = CreateLibFromGameDatas.CreateDeveropperLib(allGameDatas);
        _idsWec = CreateLibFromGameDatas.CreateGameIdLib(allGameDatas);
        _toolsWec = CreateLibFromGameDatas.CreateToolsLib(allGameDatas);

        GameStatusDefaultSetter();
    }

    public void End()
    {
        
    }

    public void WaitSendCategory()
    {
        CmdReturn cmdReturn = new CmdReturn(WaitSendCategory);
        string checkSentence = $"設定する項目名を送信してください(「{cmdReturn.ReturnWord}」で1つ前に戻れます)";
        foreach(string category in _currentFilterCondition._filteringCategory)
        {
            string appendVal = $"\n・{category}：{_currentFilterCondition.ReturnValueForCategory(category)}";
            checkSentence += appendVal;
        }
        _cmdSceneManager.OutPutManager.SendLogMessage(checkSentence, OutPutTextLogColorSets.SystemDefault);

        _cmdSceneManager.InputFieldManager.ChangeAction((string message) => ReceiveFilteringCategory(message, cmdReturn), _filteringCategoryWec);
        _cmdSceneManager.OutPutManager.SendLogMessage($"「{_decisionWord}」で決定できます。", OutPutTextLogColorSets.Blue);
    }

    public void FixStatusList(List<GameStatus> newList)
    {
        _currentFilterCondition.Statuses = newList;
    }

    private void ReceiveFilteringCategory(string message, CmdReturn cmdReturn)
    {
        if (cmdReturn.ReturnCheck(message)) return;

        CmdReturn returnWaitSendCategory = new CmdReturn(WaitSendCategory);
        if(message == _currentFilterCondition._filteringCategory[0])
        {
            _cmdSceneManager.OutPutManager.SendLogMessage("ステータスを送信してください", OutPutTextLogColorSets.SystemDefault);
            _cmdSceneManager.InputFieldManager.ChangeAction((string message) => ReceiveGameStatus(message, returnWaitSendCategory));
        }
        else if (message == _currentFilterCondition._filteringCategory[1])
        {
            _cmdSceneManager.OutPutManager.SendLogMessage("タイトルを送信してください", OutPutTextLogColorSets.SystemDefault);
            _cmdSceneManager.InputFieldManager.ChangeAction((string message) => ReceiveTitle(message, returnWaitSendCategory), _titleWec);
        }
        else if(message == _currentFilterCondition._filteringCategory[2])
        {
            _cmdSceneManager.OutPutManager.SendLogMessage("タグを送信してください", OutPutTextLogColorSets.SystemDefault);
            _cmdSceneManager.InputFieldManager.ChangeAction((string message) => ReceiveTag(message, returnWaitSendCategory), _tagsWec);
        }
        else if(message == _currentFilterCondition._filteringCategory[3])
        {
            _cmdSceneManager.OutPutManager.SendLogMessage("開発者名を送信してください", OutPutTextLogColorSets.SystemDefault);
            _cmdSceneManager.InputFieldManager.ChangeAction((string message) => ReceiveDev(message, returnWaitSendCategory), _devsWec);
        }
        else if (message == _currentFilterCondition._filteringCategory[4])
        {
            _cmdSceneManager.OutPutManager.SendLogMessage("ゲームIDを送信してください", OutPutTextLogColorSets.SystemDefault);
            _cmdSceneManager.InputFieldManager.ChangeAction((string message) => ReceiveId(message, cmdReturn), _idsWec);
        }
        else if(message == _currentFilterCondition._filteringCategory[5])
        {
            _cmdSceneManager.OutPutManager.SendLogMessage("ソフト名を送信してください", OutPutTextLogColorSets.SystemDefault);
            _cmdSceneManager.InputFieldManager.ChangeAction((string message) => ReceiveSoft(message, returnWaitSendCategory), _toolsWec);
        }
        else if (message == _decisionWord)
        {
            SendFilterCondition();
        }
    }    
    //====================================================================================
    
    private void ReceiveGameStatus(string message, CmdReturn cmdReturn)
    {
        if (cmdReturn.ReturnCheck(message)) return;
        if (OrWordCheck(message)) return;
        //messageがGameStatus型にParse出来るかを試しておく
        try
        {
            GameStatus parsed = (GameStatus)Enum.Parse(typeof(GameStatus), message);
        }
        catch (Exception e)
        {
            _cmdSceneManager.OutPutManager.SendLogMessage($"{message}はGameStatusに存在しません。送信し直してください", OutPutTextLogColorSets.AccentDefault);
            return;
        }
        
        List<string> toStrList = ConvertStatusToStr(_currentFilterCondition.Statuses);
        var systemReply = CmdRegister.RegisterArrayCategory(message, toStrList
            , registerValue => { toStrList = registerValue; });

        List<GameStatus> gameStatusList = ConvertStrStatusToEnum(toStrList);
        _currentFilterCondition.Statuses = gameStatusList;

        _cmdSceneManager.OutPutManager.SendLogMessage(systemReply.replyMessage, systemReply.logColor);
    }

    private void ReceiveTitle(string message, CmdReturn cmdReturn)
    {
        if (cmdReturn.ReturnCheck(message)) return;
        if (OrWordCheck(message)) return;        

        var systemReply = CmdRegister.RegisterArrayCategory(message, _currentFilterCondition.GameNames[0]
            , registerVal => { _currentFilterCondition.GameNames[0] = registerVal; });

        _cmdSceneManager.OutPutManager.SendLogMessage(systemReply.replyMessage, systemReply.logColor);
    }

    private void ReceiveTag(string message, CmdReturn cmdReturn)
    {
        if (cmdReturn.ReturnCheck(message)) return;
        if (OrWordCheck(message)) return;

        var systemReply = CmdRegister.RegisterArrayCategory(message, _currentFilterCondition.GameTags[0]
            , registerVal => { _currentFilterCondition.GameTags[0] = registerVal; });

        _cmdSceneManager.OutPutManager.SendLogMessage(systemReply.replyMessage, systemReply.logColor);
    }

    private void ReceiveDev(string message, CmdReturn cmdReturn)
    {
        if (cmdReturn.ReturnCheck(message)) return;
        if (OrWordCheck(message)) return;

        var systemReply = CmdRegister.RegisterArrayCategory(message, _currentFilterCondition.GameDevs[0]
            , registerVal => { _currentFilterCondition.GameDevs[0] = registerVal; });

        _cmdSceneManager.OutPutManager.SendLogMessage(systemReply.replyMessage, systemReply.logColor);
    }

    private void ReceiveId(string message, CmdReturn cmdReturn)
    {
        if (cmdReturn.ReturnCheck(message)) return;

        var systemReply = CmdRegister.RegisterSingleValInWList(message, _currentFilterCondition.GameIds,
            registerVal => { _currentFilterCondition.GameIds = registerVal; });

        _cmdSceneManager.OutPutManager.SendLogMessage(systemReply.replyMessage, systemReply.logColor);
    }

    private void ReceiveSoft(string message, CmdReturn cmdReturn)
    {
        if (cmdReturn.ReturnCheck(message)) return;
        if (OrWordCheck(message)) return;

        var systemReply = CmdRegister.RegisterArrayCategory(message, _currentFilterCondition.Softs
            , registerVal => { _currentFilterCondition.Softs = registerVal; });
    }
    //====================================================================================

    private void SendFilterCondition()
    {
        _sendCondition?.Invoke(_currentFilterCondition);
        End();
    }

    /// <summary>
    /// FilterConditionで初期値を全てにしておくために値を編集
    /// </summary>
    private void GameStatusDefaultSetter()
    {
        _currentFilterCondition.Statuses.Add(GameStatus.ByLocal);
        _currentFilterCondition.Statuses.Add(GameStatus.NotDownloaded);
        _currentFilterCondition.Statuses.Add(GameStatus.Downloading);
        _currentFilterCondition.Statuses.Add(GameStatus.Downloaded);
        _currentFilterCondition.Statuses.Add(GameStatus.UpdateAvailable);
    }

    private List<string> ConvertStatusToStr(List<GameStatus> statusList)
    {
        List<string> strList = statusList.Select(x => x.ToString()).ToList();
        return strList;
    }

    /// <summary>
    /// parse失敗する可能性あるので、tryで実行させる
    /// </summary>
    private List<GameStatus> ConvertStrStatusToEnum(List<string> strStatusList)
    {
        List<GameStatus> statusList = new List<GameStatus>();
        foreach(string str in strStatusList)
        {
            GameStatus gameStatus = (GameStatus)Enum.Parse(typeof(GameStatus), str);
            statusList.Add(gameStatus);
        }

        return statusList;
    }

    /// <summary>
    /// orキーワードには対応させないため、こっちで弾いとく
    /// </summary>
    private bool OrWordCheck(string message)
    {
        if (message == "or")
        {
            _cmdSceneManager.OutPutManager.SendLogMessage("orの使用には対応していません", OutPutTextLogColorSets.AccentDefault);
            return true;
        }
        return false;
    }
}