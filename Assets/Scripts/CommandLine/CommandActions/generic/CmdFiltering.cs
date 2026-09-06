
using System;
using System.Collections.Generic;

public class CmdFiltering
{
    private FilterCondition _currentFilterCondition;
    private CmdSceneManager _cmdSceneManager;

    private string _returnWord;

    private Action<FilterCondition> _sendCondition;

    public CmdFiltering(Action<FilterCondition> conditionSend)
    {
        Init();
        Action<FilterCondition> _conditionSend = conditionSend;
    }

    private void Init()
    {
        if(_currentFilterCondition == null)
        {
            _currentFilterCondition = new FilterCondition();
        }
        _cmdSceneManager = CmdSceneManager.Instance;
    }

    private void End()
    {

    }

    public void WaitSendCategory()
    {
        string checkSentence = $"設定する項目名を送信してください({_returnWord}で1つ前に戻れます)";

    }

    private void ReceiveFilteringCategory(string message)
    {
        string sendMessage = "";

        if(message == _currentFilterCondition._filteringCategory[0])
        {

        }
        else if (message == _currentFilterCondition._filteringCategory[1])
        {
            
        }
        else if(message == _currentFilterCondition._filteringCategory[2])
        {

        }
        else if(message == _currentFilterCondition._filteringCategory[3])
        {

        }
        else if(message == _currentFilterCondition._filteringCategory[4])
        {

        }
    } 
}
