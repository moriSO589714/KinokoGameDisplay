using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIActBase : MonoBehaviour
{
    public Action ClickAct;
    public Action PointerEnterAct;
    public Action PointerExitAct;

    public virtual void OnClickAct()
    {
        ClickAct();
    }
    public virtual void OnPointerEnter()
    {
        PointerEnterAct();
    }
    public virtual void OnPointerExit()
    {
        PointerExitAct();
    }
}
