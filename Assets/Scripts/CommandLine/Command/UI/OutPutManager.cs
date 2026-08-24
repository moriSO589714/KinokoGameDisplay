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

    [SerializeField] private float _onetimeMovePanelHeight;
    [SerializeField] private float _panelCeilingBuffer; //下にスクロールした際の上部分のバッファ幅

    [SerializeField] private MonitorPlayerInput _monitorPlayerInput;

    //テキストボックス全てを合計した高さ
    private float _overallHeigth = 0;
    //ボックスプールの初期位置
    private float _poolFirstPositionY = 0;
    private RectTransform _boxPoolRect = new RectTransform();

    private void Awake()
    {
        _boxPoolRect = _outputTextBoxPool.GetComponent<RectTransform>();
        _poolFirstPositionY = _boxPoolRect.anchoredPosition.y;

        _monitorPlayerInput.OnMouseScroll += MoveOverallPanel;
    }

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
            _overallHeigth += slideHeight;
            _overallHeigth += _outputTextInterval;
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
            _overallHeigth += changeHeight;
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
    /// ログ全体をスクロールする
    /// </summary>
    private void MoveOverallPanel(float direction)
    {
        //移動幅
        float moveHeight = _onetimeMovePanelHeight;
        //移動幅に移動方向を適用させる
        moveHeight *= direction;

        //現在のPoolの位置
        float currentPositionY = _boxPoolRect.anchoredPosition.y;
        //移動後の位置
        float forwardPositionY = currentPositionY + moveHeight;
        //現在のPool位置とスクロール制限を加味した実際に移動させる座標を計算
        float nextPositionY = MoveOverallLiminalManage(forwardPositionY, direction);

        //実際に移動
        _boxPoolRect.anchoredPosition = new Vector2(_boxPoolRect.anchoredPosition.x, nextPositionY);
    }

    /// <summary>
    /// ログ全体をスクロールする際のスクロール制限を管理
    /// </summary>
    private float MoveOverallLiminalManage(float forwardPositionY, float direction)
    {
        if (direction < 0)
        {
            //余裕幅を適用させたログオブジェクトの高さ
            float inBufferLogHeight = _overallHeigth - _panelCeilingBuffer;
            if (inBufferLogHeight < 0)
            {
                inBufferLogHeight = 0;
            }
            
            //どこまで下にいけるかの制限値(Y座標の初期値 - 現在移動させることの可能な幅の最大値)
            float ceilingLimit = _poolFirstPositionY - inBufferLogHeight;
            if(forwardPositionY <= ceilingLimit)
            {
                return ceilingLimit;
            }
            return forwardPositionY;
        }
        else
        {
            if(forwardPositionY >= _poolFirstPositionY)
            {
                return _poolFirstPositionY;
            }
            else
            {
                return forwardPositionY;
            }
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