using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EstimateCmdLibManager : MonoBehaviour
{
    [SerializeField] private GameObject _cmdLibObj;

    public readonly string separateMark = " ";
    private WordEmtCell _cmdEstimateLib = null;

    public WordEmtCell GetEstimateCmdLib()
    {
        if(_cmdEstimateLib == null)
        {
            _cmdEstimateLib = CollectCmdWord();
        }

        return _cmdEstimateLib;
    }

    private WordEmtCell CollectCmdWord()
    {
        List<string> cmdWords = new List<string>();
        RecursiveCollect(_cmdLibObj, "", ref cmdWords);

        //コマンドが一つも存在しない場合
        if(cmdWords.Count == 0)
        {
            throw new System.Exception("登録されているコマンドが存在しません");
        }

        WordEmtCell wecLib = WECLibCreater.CreateLibFromStrList(cmdWords, separateMark);
        return wecLib;
    }

    private void RecursiveCollect(GameObject parentObj ,string currentStr,ref List<string> cmds)
    {
        List<GameObject> childrenObjects = GetChildren(parentObj);
        //子オブジェクトが存在しない = これ以上の文節が存在しないとして処理を終了
        if(childrenObjects.Count == 0)
        {
            cmds.Add(currentStr);
            return;
        }

        foreach(GameObject child in childrenObjects)
        {
            string nextStr = "";
            if(currentStr == "")
            {
                nextStr = child.name;
            }
            else
            {
                nextStr = currentStr + separateMark + child.name;
            }

            RecursiveCollect(child, nextStr, ref cmds);
        }
    }

    private List<GameObject> GetChildren(GameObject target)
    {
        List<GameObject> childrenList = new List<GameObject>();
        foreach (Transform childTransform in target.transform)
        {
            childrenList.Add(childTransform.gameObject);
        }
        return childrenList;
    }
}
