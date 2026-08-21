using UnityEngine;
using UnityEngine.UI;

public class OutputTextBox : MonoBehaviour
{
    [SerializeField] private Text _txtObject;

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
}
