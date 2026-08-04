using UnityEngine;
using UnityEngine.UI;

public class OutputTextBox : MonoBehaviour
{
    [SerializeField] private Text _txtObject;
    [SerializeField] private Color _userMessageTextColor;
    private Color _defaultTextColor;

    //falseはシステムからのメッセージ
    public bool _isUserMessage { get; private set; }
    public string _thisMessage { get; private set; }
    public string _identificationUUID { get; private set; }

    private void Awake()
    {
        _defaultTextColor = _txtObject.color;
    }

    public void ActivateThis(string message, bool isUserMessage, string UUID = null)
    {
        _thisMessage = message;
        _txtObject.text = message;
        if(UUID != null)
        {
            _identificationUUID = UUID;
        }

        _isUserMessage = isUserMessage;
        if (_isUserMessage)
        {
            _txtObject.color = _userMessageTextColor;
        }
        else
        {
            _txtObject.color = _defaultTextColor;
        }

        //オブジェクトサイズを強制再読み込み
        ContentSizeFitter csf = GetComponent<ContentSizeFitter>();
        csf.SetLayoutHorizontal();
        csf.SetLayoutVertical();
        LayoutRebuilder.ForceRebuildLayoutImmediate(csf.GetComponent<RectTransform>());
    }
}