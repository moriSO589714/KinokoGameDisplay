using SFB;
using System.IO;
using System.Text;

public class CmdEncrypt : CmdAct
{
    public override void FirstCall()
    {
        base.FirstCall();
        CmdReturn cmdReturn = new CmdReturn(ReturnCmdReceiveMode);
        _cmdSceneManager.InputFieldManager.ChangeAction((string message) => ReceiveJsonKeyPath(message, cmdReturn));
        _cmdSceneManager.OutPutManager.SendLogMessage("暗号化したいjsonファイルのパスを送信してください", OutPutTextLogColorSets.SystemDefault);
        ExtensionFilter filter = new ExtensionFilter("JSON FILE", "json");
        string selectedPath = new OpenFilePanel().OpenFilePanelAndReturnPath(new ExtensionFilter[1] { filter });
        if (selectedPath != null) _cmdSceneManager.InputFieldManager.ChangeInputfieldVal(selectedPath);
    }

    private void ReceiveJsonKeyPath(string message, CmdReturn cmdReturn)
    {
        if (cmdReturn.ReturnCheck(message)) return;

        //ファイルが存在するかの確認
        if (!File.Exists(message))
        {
            _cmdSceneManager.OutPutManager.SendLogMessage("送信されたファイルは存在しません。パスを送信し直してください", OutPutTextLogColorSets.AccentDefault);
            return;
        }

        PathKeyManager keyManager = new PathKeyManager();
        string jsonText = "";
        try
        {
            jsonText = File.ReadAllText(message);
        }
        catch
        {
            _cmdSceneManager.OutPutManager.SendLogMessage("送信されたファイルは読み取り不可能です。パスを送信し直してください", OutPutTextLogColorSets.AccentDefault);
            return;
        }

        string encryptText = "";
        try
        {
            encryptText = keyManager.EncryptJsonKey(jsonText);
        }
        catch
        {
            _cmdSceneManager.OutPutManager.SendLogMessage("暗号化に失敗しました。パスを送信し直してください", OutPutTextLogColorSets.AccentDefault);
            return;
        }


        _cmdSceneManager.OutPutManager.SendLogMessage("暗号化に成功しました。保存する場所のパスを送信してください", OutPutTextLogColorSets.SystemDefault);
        _cmdSceneManager.InputFieldManager.ChangeAction((string message) => ReceiveSavedPath(message, cmdReturn, encryptText));
        ExtensionFilter filter = new ExtensionFilter("TEXT FILE", "txt");
        string selectedPath = new OpenFilePanel().OpenFilePanelAndReturnPath(new ExtensionFilter[1] { filter });
        if (selectedPath != null) _cmdSceneManager.InputFieldManager.ChangeInputfieldVal(selectedPath);
    }

    private void ReceiveSavedPath(string message, CmdReturn cmdReturn, string encryptText)
    {
        if (cmdReturn.ReturnCheck(message)) return;

        try
        {
            Encoding enc = Encoding.UTF8;
            using (StreamWriter writer = new StreamWriter(message, false, enc))
            {
                writer.WriteLine(encryptText);
            }
        }
        catch(System.Exception e)
        {
            _cmdSceneManager.OutPutManager.SendLogMessage("テキストファイルの出力に失敗しました", OutPutTextLogColorSets.AccentDefault);
        }

        _cmdSceneManager.OutPutManager.SendLogMessage("パスキーの出力に成功しました", OutPutTextLogColorSets.SystemDefault);
        ReturnCmdReceiveMode();
    }
}
