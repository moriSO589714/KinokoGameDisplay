using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonitorPlayerInput : MonoBehaviour
{
    public Action<float> onMouseScroll;
    void Update()
    {
        float scrollDelta = Input.mouseScrollDelta.y;
        if(scrollDelta != 0)
        {
            if (scrollDelta > 0) onMouseScroll(1);
            else if (scrollDelta < 0) onMouseScroll(-1);
        }
    }
}
