using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class CommandManager : MonoBehaviour
{
    [SerializeField] CommandNode _commandLibrary;

    [SerializeField] string _notFindCommandMessage = "";
    private const char _sequence = ' ';
    
    private CmdSceneManager _cmdSceneManager = new CmdSceneManager();
    private void Awake()
    {
        _cmdSceneManager = CmdSceneManager.Instance;
    }

    public void ReceiveCommand(string command)
    {
        string[] splitCommand = command.Split(_sequence);
        UnityEvent executeCommand = SearchMethodFromLibray(splitCommand, _commandLibrary);

        //対応するコマンドが存在しない場合は何も処理を行わない
        if (executeCommand == null)
        {
            _cmdSceneManager.OutPutManager.ReceiveMessage(_notFindCommandMessage);
            return;
        }
        executeCommand.Invoke();
    }

    private UnityEvent SearchMethodFromLibray(string[] splitCommandArray, CommandNode currentNode)
    {
        //引数のチェック
        if(splitCommandArray == null || splitCommandArray.Count() == 0 || currentNode.ChildrenNodes == null)
        {
            return null;
        }

        UnityEvent executeCommand = SearchInternal(splitCommandArray, currentNode, 0);
        return executeCommand;
    }

    private UnityEvent SearchInternal(string[] splitCommandArray, CommandNode currentNode, int currentIndex)
    {
        //現在のノードの子要素からコマンドにあうものを探索
        foreach(CommandNode child in currentNode.ChildrenNodes)
        {
            if(child.gameObject.name == splitCommandArray[currentIndex])
            {
                //コマンド文の最後まで到達した場合
                if(currentIndex == splitCommandArray.Count() - 1)
                {
                    return child.OnAction;
                }

                //次の階層を探索する
                UnityEvent executeCommand = SearchInternal(splitCommandArray, child, currentIndex + 1 );
                //見つかった場合上の階層に持っていく
                if(executeCommand != null)
                {
                    return executeCommand;
                }
            }
        }

        //一致するコマンドが無かった場合
        return null;
    }
}
