using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// API通信に使うjsonPathKey関連の処理をまとめたクラス
/// </summary>
public class PathKeyManager
{
    /// <summary>
    /// 暗号化された文字列が書かれたテキストファイルのパスから復号してkey文字列を取得する
    /// </summary>
    public string GetKeyFromCipherTxtPath(string path)
    {
        string cipherTxt;

        using (StreamReader sr = new StreamReader(path, Encoding.GetEncoding("utf-8")))
        {
            cipherTxt = sr.ReadToEnd();
        }

        string keyStr = new EncryptAES().TxtToDecryptTxt(cipherTxt);
        return keyStr;
    }

    /// <summary>
    /// 暗号化された文字列を保存する
    /// </summary>
    public void ActivateKeyCode(string cipherTxt)
    {
        AllDirs allDirs = AllDirs.GetInstance();
        string savePath = allDirs.JsonPathKey;

        //コードが正しいかどうかの確認(最低限)
        if(cipherTxt == "")
        {
            throw new System.Exception("コードが空白です");
        }
        else if(cipherTxt.Count() <= 500)
        {
            throw new System.Exception("コードが短すぎます");
        }
        else if(cipherTxt.Contains(" ") || cipherTxt.Contains("　"))
        {
            throw new System.Exception("コードに含まれてはいけない文字が含まれています");
        }

        using (StreamWriter sw = new StreamWriter(savePath, false, Encoding.GetEncoding("utf-8")))
        {
            sw.Write(cipherTxt);
        }
    }

    /// <summary>
    /// utf-8でエンコードされた文字列のjsonKeyを暗号化する
    /// </summary>
    public string EncryptJsonKey(string jsonKey)
    {
        string cipherTxt = new EncryptAES().TxtToEncryptTxt(jsonKey);
        return cipherTxt;
    }
}
