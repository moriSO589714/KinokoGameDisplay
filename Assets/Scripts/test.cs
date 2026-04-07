using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField] GameObject obj;

    // Start is called before the first frame update
    void Start()
    {
        /*
        new EachDataLoad().LocalDataLoad();
        obj.GetComponent<GameBoxsManager>().SetGameBoxsByGameDataList(GameDatasSingleton.Instance.GameDatas);
        Debug.Log("End");
        */
    }
}
