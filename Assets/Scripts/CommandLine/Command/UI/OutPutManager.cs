using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutPutManager : MonoBehaviour
{
    public void ReceiveMessage(string message)
    {
        OutPut(message);
    }

    private void OutPut(string message)
    {
        Debug.Log(message);
    }
}
