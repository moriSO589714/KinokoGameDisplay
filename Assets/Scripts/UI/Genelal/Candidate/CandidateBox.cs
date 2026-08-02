using System;
using UnityEngine;
using UnityEngine.UI;

public class CandidateBox : MonoBehaviour
{
    [SerializeField] private Text _txtBox;
    [SerializeField] private int _limitLabelLength = 17;
    [SerializeField] private Sprite _selectedSprite;

    public string _pureTxt { get; private set; }

    private Sprite _normalSprite;
    private string _labelTxt;

    private void Awake()
    {
        _normalSprite = gameObject.GetComponent<Image>().sprite;
    }

    public void SetLabel(string txt)
    {
        _pureTxt = txt;
        _labelTxt = StrTools.ReplaceOverWords(txt, _limitLabelLength);
        _txtBox.text = _labelTxt;
    }

    public void SelectThis()
    {
        SetActive();
    }

    public void LeaveThis()
    {
        CancellActive();
    }

    protected virtual void SetActive()
    {
        gameObject.GetComponent<Image>().sprite = _selectedSprite;
        _txtBox.color = Color.white;
    }

    protected virtual void CancellActive()
    {
        gameObject.GetComponent<Image>().sprite = _normalSprite;
        _txtBox.color = Color.black;
    }
}
