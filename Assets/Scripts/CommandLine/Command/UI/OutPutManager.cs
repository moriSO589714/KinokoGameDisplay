using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class OutputManager : ObjectStuckPool<OutputTextBox>
{
    [SerializeField] private GameObject _outputTextBoxPool;
    [SerializeField] private GameObject _outputTextBoxPref;
    [SerializeField] private float _outputTextInterval;

    [SerializeField] private Vector2 _firstCreatePos;

    public string ReceiveMessage(string message, bool isUserMessage, string specifiedUUID = null)
    {
        return Output(message, isUserMessage, specifiedUUID);
    }

    public string ReturnText(string uuid)
    {
        OutputTextBox outputTextBox = SearchSpecifiedTextBoxFromStuck(uuid);
        return outputTextBox._thisMessage;
    }

    private string Output(string message, bool isUserMessage, string specifiedUUID = null)
    {
        string returnUUID = "";
        if(specifiedUUID == null)
        {
            //テキストボックスを生成
            OutputTextBox outputTextBox = InstantiateTextBox(message, isUserMessage);
            returnUUID = outputTextBox._identificationUUID;
            float slideHeight = CalcSlideHeight(outputTextBox);
            TransferActiveTextBox(slideHeight);

            _activeStuckPool.Add(outputTextBox);
        }
        else
        {
            OutputTextBox outputTextBox = SearchSpecifiedTextBoxFromStuck(specifiedUUID);
            if(outputTextBox == null)
            {
                throw new System.Exception("指定されたuuidを持つテキストは生成されていません");
            }
            returnUUID = specifiedUUID;
            //元の高さを取得
            RectTransform outputTextBoxRect = outputTextBox.GetComponent<RectTransform>();
            float previousHeight = outputTextBoxRect.sizeDelta.y;
            outputTextBox.ActivateThis(message, outputTextBox._isUserMessage, specifiedUUID);
            //高さの変化量
            float changeHeight = outputTextBoxRect.sizeDelta.y - previousHeight;
            TransferActiveTextBox(changeHeight, _activeStuckPool.IndexOf(outputTextBox) - 1);
        }

        return returnUUID;
    }

    private OutputTextBox InstantiateTextBox(string message, bool isUserMessage)
    {
        OutputTextBox notUsed = ReturnNotUsedObject();

        OutputTextBox target = null;
        if(notUsed == null)
        {
            GameObject instanced = Instantiate(_outputTextBoxPref, parent: _outputTextBoxPool.transform);
            target = instanced.GetComponent<OutputTextBox>();
        }
        else
        {
            target = notUsed;
        }

        target.gameObject.SetActive(true);
        string uuid = UUIDGenerator.GenerateUUID();
        target.ActivateThis(message, isUserMessage, uuid);

        //位置を初期位置に移動
        target.gameObject.GetComponent<RectTransform>().anchoredPosition = _firstCreatePos;
        return target;
    }

    /// <summary>
    /// 現在生成しているスタックからUUIDが一致しているインスタンスを検索する
    /// </summary>
    private OutputTextBox SearchSpecifiedTextBoxFromStuck(string UUID)
    {
        List<OutputTextBox> matchTextBoxs = _activeStuckPool.Where(x => x._identificationUUID == UUID).ToList();
        //複数一致するテキストが存在する場合
        if(matchTextBoxs.Count > 1)
        {
            throw new System.Exception("対象のテキストボックスが複数生成されています");
        }
        else if(matchTextBoxs.Count == 1)
        {
            return matchTextBoxs[0];
        }
        else
        {
            return null;
        }
    }

    private float CalcSlideHeight(OutputTextBox outputTextBox)
    {
        float textBoxHeight = outputTextBox.GetComponent<RectTransform>().sizeDelta.y;
        float slideHeight = textBoxHeight + _outputTextInterval;
        return slideHeight;
    }

    private void TransferActiveTextBox(float slideHeight, int centralIndex = -1)
    {
        if(centralIndex == -1)
        {
            centralIndex = _activeStuckPool.Count - 1;
        }

        for(int i = centralIndex; i >= 0; i--)
        {
            RectTransform rectTransform = _activeStuckPool[i].GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y + slideHeight);
        }
    }
}