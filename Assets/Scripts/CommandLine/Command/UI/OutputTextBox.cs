using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class OutputTextBox : MonoBehaviour
{
    [SerializeField] private Text _txtObject;
    [SerializeField] private Text _timeTxtObject;

    //falseはシステムからのメッセージ
    public bool _isUserMessage { get; private set; }
    public string _thisMessage { get; private set; }
    public string _textColor { get; private set; }
    public string _identificationUUID { get; private set; }

    public void ActivateThis(string message, Color textColor, bool isUserMessage, string UUID = null)
    {
        _thisMessage = message;
        _txtObject.text = message;
        _isUserMessage = isUserMessage;
        _timeTxtObject.text = ReturnTime();
        if(UUID != null)
        {
            _identificationUUID = UUID;
        }

        //テキストの色の設定
        _txtObject.color = textColor;

        //オブジェクトサイズを強制再読み込み
        ContentSizeFitter csf = GetComponent<ContentSizeFitter>();
        csf.SetLayoutHorizontal();
        csf.SetLayoutVertical();
        LayoutRebuilder.ForceRebuildLayoutImmediate(csf.GetComponent<RectTransform>());
    }

    private string ReturnTime()
    {
        DateTime dateTime = DateTime.Now;
        return dateTime.ToString("HH:mm:ss");
    }
}
