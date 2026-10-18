using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransferColorUIAct : UIActBase
{
    [SerializeField] private Color _targetColor;
    [SerializeField] private float _inSpeed;
    [SerializeField] private float _outSpeed;

    private TransferColor _transferColor;
    protected virtual void Awake()
    {
        _transferColor = new TransferColor(this.GetComponent<Image>(), _targetColor, _inSpeed, _outSpeed);
    }

    public override void OnPointerEnter()
    {
        _transferColor.TranferCustomizeColor();
        base.OnPointerEnter();
    }

    public override void OnPointerExit()
    {
        _transferColor.TransferOriginColor();
        base.OnPointerExit();
    }
}
