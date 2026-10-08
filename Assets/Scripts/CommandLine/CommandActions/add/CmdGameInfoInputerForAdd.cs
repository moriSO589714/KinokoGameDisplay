using System;

public class CmdGameInfoInputerForAdd : CmdGameInfoInputer
{
    protected readonly new string _decisionWord = "upload";

    //CmdGameInfoInputerのコンストラクタに値を渡す
    public CmdGameInfoInputerForAdd(Action<GameData, string, string> sendGameDataAct) : base(sendGameDataAct)
    {
        
    }

    public CmdGameInfoInputerForAdd(){}

    public override void StartInputData(CmdReturn returnCmdReceive)
    {
        base.StartInputData(returnCmdReceive);

        //アップロードに必要なデータが最低限セットされているかを確認する
        if (GameDataForUpload.QualityCheck(_currentGameData, _userGamePath))
        {
            _allowDecision = true;
            _sceneManager.OutPutManager.SendLogMessage
                ($"※※アップロードが行えます。アップロードを実行する場合は「{_decisionWord}」を送信してください※※", OutPutTextLogColorSets.Blue);
        }
    }

    protected override bool CheckDecision(string message)
    {
        if(message == _decisionWord && _allowDecision)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
