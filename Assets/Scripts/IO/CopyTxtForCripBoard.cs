using UnityEngine;

public class CopyTxtForCripBoard
{
    public void CopyTxt(string txt)
    {
        GUIUtility.systemCopyBuffer = txt;
    }
}
