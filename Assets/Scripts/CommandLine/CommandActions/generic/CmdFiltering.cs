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

        GameStatusDefaultSetter();
    }

    public void End()
    {
        
    }

    public void WaitSendCategory()
    {
        string checkSentence = $"設定する項目名を送信してください(で1つ前に戻れます)";
        foreach(string category in _currentFilterCondition._filteringCategory)
        {
            string appendVal = $"\n・{category}：{_currentFilterCondition.ReturnValueForCategory(category)}";
            checkSentence += appendVal;
        }
        _cmdSceneManager.OutPutManager.ReceiveMessage(checkSentence, OutPutTextLogColorSets.SystemDefault);

        CmdReturn cmdReturn = new CmdReturn(WaitSendCategory);
        _cmdSceneManager.InputFieldManager.ChangeAction((string message) => ReceiveFilteringCategory(message, cmdReturn));
        _cmdSceneManager.OutPutManager.ReceiveMessage($"「{_decisionWord}」で決定できます。", OutPutTextLogColorSets.Blue);
    }

    private void ReceiveFilteringCategory(string message, CmdReturn cmdReturn)
    {
        if (cmdReturn.ReturnCheck(message)) return;

        CmdReturn returnWaitSendCategory = new CmdReturn(WaitSendCategory);
        if(message == _currentFilterCondition._filteringCategory[0])
        {
            _cmdSceneManager.OutPutManager.ReceiveMessage("ステータスを送信してください", OutPutTextLogColorSets.SystemDefault);
            _cmdSceneManager.InputFieldManager.ChangeAction((string message) => ReceiveGameStatus(message, returnWaitSendCategory));
        }
        else if (message == _currentFilterCondition._filteringCategory[1])
        {
            _cmdSceneManager.OutPutManager.ReceiveMessage("タイトルを送信してください", OutPutTextLogColorSets.SystemDefault);
            _cmdSceneManager.InputFieldManager.ChangeAction((string message) => ReceiveTitle(message, returnWaitSendCategory));
        }
        else if(message == _currentFilterCondition._filteringCategory[2])
        {
            _cmdSceneManager.OutPutManager.ReceiveMessage("タグを送信してください", OutPutTextLogColorSets.SystemDefault);
            _cmdSceneManager.InputFieldManager.ChangeAction((string message) => ReceiveTag(message, returnWaitSendCategory));
        }
        else if(message == _currentFilterCondition._filteringCategory[3])
        {
            _cmdSceneManager.OutPutManager.ReceiveMessage("開発者名を送信してください", OutPutTextLogColorSets.SystemDefault);
            _cmdSceneManager.InputFieldManager.ChangeAction((string message) => ReceiveDev(message, returnWaitSendCategory));
        }
        else if(message == _currentFilterCondition._filteringCategory[4])
        {
            _cmdSceneManager.OutPutManager.ReceiveMessage("ソフト名を送信してください", OutPutTextLogColorSets.SystemDefault);
            _cmdSceneManager.InputFieldManager.ChangeAction((string message) => ReceiveSoft(message, returnWaitSendCategory));
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
            _cmdSceneManager.OutPutManager.ReceiveMessage($"{message}はGameStatusに存在しません。送信し直してください", OutPutTextLogColorSets.AccentDefault);
            return;
        }
        
        List<string> toStrList = ConvertStatusToStr(_currentFilterCondition.Statuses);
        var systemReply = CmdRegister.RegisterArrayCategory(message, toStrList.ToArray()
            , registerValue => { toStrList = registerValue.ToList(); });

        List<GameStatus> gameStatusList = ConvertStrStatusToEnum(toStrList);
        _currentFilterCondition.Statuses = gameStatusList;

        _cmdSceneManager.OutPutManager.ReceiveMessage(systemReply.replayMessage, systemReply.logColor);
    }

    private void ReceiveTitle(string message, CmdReturn cmdReturn)
    {
        if (cmdReturn.ReturnCheck(message)) return;
        if (OrWordCheck(message)) return;        

        var systemReply = CmdRegister.RegisterArrayCategory(message, _currentFilterCondition.GameNames[0].ToArray()
            , registerVal => { _currentFilterCondition.GameNames[0] = registerVal.ToList(); });

        _cmdSceneManager.OutPutManager.ReceiveMessage(systemReply.replayMessage, systemReply.logColor);
    }

    private void ReceiveTag(string message, CmdReturn cmdReturn)
    {
        if (cmdReturn.ReturnCheck(message)) return;
        if (OrWordCheck(message)) return;



        var systemReply = CmdRegister.RegisterArrayCategory(message, _currentFilterCondition.GameTags[0].ToArray()
            , registerVal => { _currentFilterCondition.GameTags[0] = registerVal.ToList(); });

        _cmdSceneManager.OutPutManager.ReceiveMessage(systemReply.replayMessage, systemReply.logColor);
    }

    private void ReceiveDev(string message, CmdReturn cmdReturn)
    {
        if (cmdReturn.ReturnCheck(message)) return;
        if (OrWordCheck(message)) return;

        var systemReply = CmdRegister.RegisterArrayCategory(message, _currentFilterCondition.GameDevs[0].ToArray()
            , registerVal => { _currentFilterCondition.GameDevs[0] = registerVal.ToList(); });

        _cmdSceneManager.OutPutManager.ReceiveMessage(systemReply.replayMessage, systemReply.logColor);
    }

    private void ReceiveSoft(string message, CmdReturn cmdReturn)
    {
        if (cmdReturn.ReturnCheck(message)) return;
        if (OrWordCheck(message)) return;

        var systemReply = CmdRegister.RegisterArrayCategory(message, _currentFilterCondition.Softs.ToArray()
            , registerVal => { _currentFilterCondition.Softs = registerVal.ToList(); });
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
            _cmdSceneManager.OutPutManager.ReceiveMessage("orの使用には対応していません", OutPutTextLogColorSets.AccentDefault);
            return true;
        }
        return false;
    }
}