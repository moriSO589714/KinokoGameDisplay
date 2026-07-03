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

    private void SwitchCheckMark()
    {
        if (CheckMark.activeInHierarchy)
        {
            CheckMark.SetActive(false);
        }
        else
        {
            CheckMark.SetActive(true);
        }
    }
}
