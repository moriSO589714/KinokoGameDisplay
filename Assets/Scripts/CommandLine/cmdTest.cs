using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cmdTest : MonoBehaviour
{
    [SerializeField] GameObject obj;
    void Start()
    {
        obj.GetComponent<CmdShowTxt>().ShowInputWord();
    }
}
