using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TagInputManager : FreeInputManager
{
    [SerializeField] private LabelFieldManager _labelFieldManager;

    public void RegisterInput()
    {
        string inputTxt = _myInputField.text;
        _labelFieldManager.AddLabel(inputTxt);
        _myInputField.text = "";
    }
}
