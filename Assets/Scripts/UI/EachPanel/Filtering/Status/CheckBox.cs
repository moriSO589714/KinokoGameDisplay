using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckBox : UIActBase
{
    [SerializeField] GameObject CheckMark;

    private void OnEnable()
    {
        CheckMark.SetActive(true);
    }

    public override void OnClickAct()
    {
        SwitchCheckMark();
        base.OnClickAct();
    }

    public void CheckMarkSwitchActive(bool isActive)
    {
        if (isActive)
        {
            CheckMark.SetActive(true);
        }
        else
        {
            CheckMark.SetActive(false);
        }
    }

    private void SwitchCheckMark()
    {
        if (CheckMark.activeInHierarchy)
        {
            CheckMarkSwitchActive(false);
        }
        else
        {
            CheckMarkSwitchActive(true);
        }
    }

}
