using System;

/// <summary>
/// コマンドシーンで利用する、1つ前に戻る機能の実装
/// </summary>
public class CmdReturn
{
    public readonly string ReturnWord = "return";
    private Action ReturnAct;

    public CmdReturn(Action returnAct)
    {
        ReturnAct = returnAct;
    }

    public bool ReturnCheck(string message)
    {
        if(message == ReturnWord)
        {
            ReturnAct.Invoke();
            return true;
        }
        return false;
    }
}