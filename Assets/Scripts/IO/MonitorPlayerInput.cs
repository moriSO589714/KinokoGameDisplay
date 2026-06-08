using System;
using UnityEngine;

public class MonitorPlayerInput : MonoBehaviour
{
    public Action TabButtonAct;
    public Action UpArrowAct;
    public Action DownArrowAct;
    public Action EscapeAct;
    public Action<float> OnMouseScroll;
    void Update()
    {
        float scrollDelta = Input.mouseScrollDelta.y;
        if(scrollDelta != 0)
        {
            if (OnMouseScroll != null && scrollDelta > 0) OnMouseScroll(1);
            else if (OnMouseScroll != null && scrollDelta < 0) OnMouseScroll(-1);
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if(TabButtonAct != null) TabButtonAct();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if(UpArrowAct != null) UpArrowAct();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (DownArrowAct != null) DownArrowAct();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (EscapeAct != null) EscapeAct();
        }
    }
}
