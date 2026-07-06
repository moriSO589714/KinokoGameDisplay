using System;
using UnityEngine;
using UnityEngine.UI;

public class PullDownCell : MonoBehaviour
{
    [SerializeField] private Text _txtBox;
    [SerializeField] private int _limitTxtLength = 17;
    [SerializeField] private Sprite _selectedSprite;
    [SerializeField] private UIActBase _myUIAct;

    public string _settingMyTxt { get; private set; }
    public Action<string> OnCliledCellAct;

    private Image _myImage;
    private Sprite _normalSprite;

    private void Awake()
    {
        _myImage = this.gameObject.GetComponent<Image>();
        _normalSprite = _myImage.sprite;
        _myUIAct.PointerEnterAct += SetActive;
        _myUIAct.PointerExitAct += CancellActive;
        _myUIAct.ClickAct += () => OnCliledCellAct.Invoke(_settingMyTxt);
    }

    public void SetText(string txt)
    {
        _settingMyTxt = txt;
        _txtBox.text = StrTools.ReplaceOverWords(_settingMyTxt, _limitTxtLength);
    }

    public void End()
    {
        CancellActive();
    }

    private void SetActive()
    {
        _myImage.sprite = _selectedSprite;
        _txtBox.color = Color.white;
    }

    private void CancellActive()
    {
        _myImage.sprite = _normalSprite;
        _txtBox.color = Color.black;
    }
}
