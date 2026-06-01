using UnityEngine;
using UnityEngine.UI;

public class CandidateBox : MonoBehaviour
{
    [SerializeField] private Text _txtBox;
    private string _labelTxt;

    public void SetLabel(string txt)
    {
        _labelTxt = txt;
        _txtBox.text = txt;
    }
}
