using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilterManagerFreeInput : MonoBehaviour
{
    [SerializeField] protected LabelFieldManager _labelFieldManager;
    [SerializeField] protected FreeInputManager _inputManager;

    protected void OnEnable()
    {
        _inputManager.SetSendInputValueAct(_labelFieldManager.AddLabel);
    }
}
