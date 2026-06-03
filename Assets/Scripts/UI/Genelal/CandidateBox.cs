using UnityEngine;
using UnityEngine.UI;

public class CandidateBox : MonoBehaviour
{
    [SerializeField] private Text _txtBox;
    [SerializeField] private int _limitLabelLength = 17;
    public string _pureTxt { get; private set; }
    private string _labelTxt;
    private bool _isActive = false;

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

    private void SetActive()
    {
        _txtBox.color = Color.red;
        _isActive = true;
    }

    private void CancellActive()
    {
        _txtBox.color = Color.black;
        _isActive = false;
    }
}
