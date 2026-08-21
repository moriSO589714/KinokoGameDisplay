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

    [SerializeField] private Color _userMessageTextColor;
    [SerializeField] private Color _accentMessageTextColor;
    [SerializeField] private Color _defaultTextColor;

    public string ReceiveMessage(string message,Color textColor, bool isUserMessage = false, string specifiedUUID = null)
    {
        return Output(message, textColor, isUserMessage, specifiedUUID);
    }

    public string ReceiveMessage(string message, OutPutTextLogColorSets outPutTextLogColorSets, bool isUserMessage = false, string specifiedUUID = null)
    {
        Color textColor = GetTextColor(outPutTextLogColorSets);
        return Output(message, textColor, isUserMessage, specifiedUUID);
    }

    public string ReturnText(string uuid)
    {
        OutputTextBox outputTextBox = SearchSpecifiedTextBoxFromStuck(uuid);
        return outputTextBox._thisMessage;
    }

    private string Output(string message,Color textColor, bool isUserMessage, string specifiedUUID = null)
    {
        string returnUUID = "";
        if(specifiedUUID == null)
        {
            //テキストボックスを生成
            OutputTextBox outputTextBox = InstantiateTextBox(message, textColor, isUserMessage);
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
            outputTextBox.ActivateThis(message, textColor, outputTextBox._isUserMessage, specifiedUUID);
            //高さの変化量
            float changeHeight = outputTextBoxRect.sizeDelta.y - previousHeight;
            TransferActiveTextBox(changeHeight, _activeStuckPool.IndexOf(outputTextBox) - 1);
        }

        return returnUUID;
    }

    private OutputTextBox InstantiateTextBox(string message, Color textColor, bool isUserMessage = false)
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
        target.ActivateThis(message, textColor, isUserMessage, uuid);

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


    /// <summary>
    /// enumに対応した色を返す
    /// </summary>
    private Color GetTextColor(OutPutTextLogColorSets outPutTextLogColorSets)
    {
        switch (outPutTextLogColorSets)
        {
            case OutPutTextLogColorSets.SystemDefault:
                return _defaultTextColor;
            case OutPutTextLogColorSets.UserDefault:
                return _userMessageTextColor;
            case OutPutTextLogColorSets.AccentDefault:
                return _accentMessageTextColor;
            case OutPutTextLogColorSets.Black:
                return Color.black;
            case OutPutTextLogColorSets.White:
                return Color.white;
            case OutPutTextLogColorSets.Red:
                return Color.red;
            case OutPutTextLogColorSets.Blue:
                return Color.blue;
            case OutPutTextLogColorSets.Yellow:
                return Color.yellow;
            default:
                return _defaultTextColor;
        }
    }
}

public enum OutPutTextLogColorSets
{
    SystemDefault,
    UserDefault,
    AccentDefault,
    Black,
    White,
    Red,
    Blue,
    Yellow
}