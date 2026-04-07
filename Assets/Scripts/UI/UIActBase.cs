using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract class UIActBase : MonoBehaviour
{
    public abstract void OnClickAct();
    public abstract void OnPointerEnter();
    public abstract void OnPointerExit();
}
