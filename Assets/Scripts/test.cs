using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class test : MonoBehaviour
{
    private string path = "C:/Users/souza/Downloads/testtest.txt";

    // Start is called before the first frame update
    void Start()
    {
            Debug.Log("READING");
        using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
        {
            int ret;
            byte[] buf = new byte[1024];

            ret = reader.Read(buf, 0, 1024);
            for (int i = 0; i < ret; i++)
            {
                Debug.Log(buf[i].ToString());
            }
        }    
    }
}
