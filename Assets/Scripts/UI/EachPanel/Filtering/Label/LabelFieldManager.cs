using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ラベルフィールドの管理
/// </summary>
public class LabelFieldManager : ObjectStuckPool<Label>
{
    [SerializeField] private GameObject _labelPref;
    [SerializeField] private GameObject _labelFieldObj;
    [SerializeField] private GameObject _whenEmptyDisplay = null;

    //ラベルフィールドと実際にラベルを生成するエリアの余白サイズ
    [SerializeField] private Vector2 _fieldMargin;
    //ラベル同士の間隔
    [SerializeField] private Vector2 _labelSpacing;

    private RectTransform _labelFieldRect;
    private float _labelHeight;
    private Vector2 _firstFieldSize = Vector2.zero;
    private Vector2 _innerFieldSize = Vector2.zero;
    private Vector2 _firstLabelPos = Vector2.zero;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _labelFieldRect = _labelFieldObj.GetComponent<RectTransform>();

        //ラベルフィールド(後ろの灰色)の横幅と縦幅を取得する
        _firstFieldSize = _labelFieldRect.sizeDelta;
        //fieldMarginを加味して生成するエリアのサイズを計算する
        _innerFieldSize = _firstFieldSize - _fieldMargin * 2;

        //ラベルの縦幅
        _labelHeight = _labelFieldRect.sizeDelta.y;
        //fieldMarginから初めに生成されるラベルの場所を決定
        _firstLabelPos = new Vector2(_fieldMargin.x, -_fieldMargin.y);
    }

    public override void ClearActiveStucks()
    {
        base.ClearActiveStucks();

        //ラベルフィール(後ろの灰色)のサイズを初期値に戻す
        _labelFieldRect.sizeDelta = _firstFieldSize;
    }

    /// <summary>
    /// 現在のラベルの最後尾に新しくラベルを追加する
    /// </summary>
    public void AddLabel(string labelName)
    {
        //ラベルの生成
        Label addLabel = InstantiateLabel(labelName);
        //リストに追加
        _activeStuckPool.Add(addLabel);
        //ui位置座標を取得
        Vector2 addLabelPos = CalcLabelPos(_activeStuckPool.Count - 1, _activeStuckPool);
        //生成したラベルを移動させてアクティブ化
        addLabel.GetComponent<RectTransform>().anchoredPosition = addLabelPos;
        addLabel.gameObject.SetActive(true);
        FlexibleLabelFieldHeight(CalcLabelRowLength());

        if(_whenEmptyDisplay != null)
        {
            _whenEmptyDisplay.gameObject.SetActive(false);
        }
    }

    public override void RemoveActiveObject(Label targetObj)
    {
        int removeIndex = _activeStuckPool.IndexOf(targetObj);

        //オブジェクトの削除処理
        base.RemoveActiveObject(targetObj);
        targetObj.gameObject.SetActive(false);

        //残ったオブジェクトの再配置
        for (int i = removeIndex; i <= _activeStuckPool.Count - 1; i++)
        {
            //再配置を行う
            Vector2 replacePos = CalcLabelPos(i, _activeStuckPool);
            _activeStuckPool[i].MyRect.anchoredPosition = replacePos;
        }

        //列数計算
        int rowLength = CalcLabelRowLength();
        if (rowLength == -1)
        {
            if (_whenEmptyDisplay != null)
            {
                _whenEmptyDisplay.gameObject.SetActive(true);
            }
            rowLength = 1;
        }
        FlexibleLabelFieldHeight(rowLength);
    }

    public List<string> ReturnActiveLabelTxts()
    {
        List<string> activeLabelTxts = _activeStuckPool.Select(x => x.MyLabelName).ToList();
        return activeLabelTxts;
    }

    /// <summary>
    /// ラベルを生成するか現在使用していないラベルを割り当てる
    /// </summary>
    private Label InstantiateLabel(string labelName)
    {
        //生成済みのラベル(_labelPool)から現在利用していないものを探す
        Label notUsedLabel = ReturnNotUsedObject();

        Label targetLabel = null;
        if(notUsedLabel == null)
        {
            GameObject instanced = Instantiate(_labelPref, parent: _labelFieldObj.transform);
            targetLabel = instanced.GetComponent<Label>();
            instanced.SetActive(false);
        }
        else
        {
            targetLabel = notUsedLabel;
        }
        targetLabel.ActivateLabel(labelName, RemoveActiveObject);

        return targetLabel;
    }

    /// <summary>
    /// リストのインデックス値からui生成座標を計算
    /// </summary>
    private Vector2 CalcLabelPos(int labelIndexInList, List<Label> referPool)
    {
        Vector2 newPos = Vector2.zero;
        if(labelIndexInList == 0)
        {
            newPos.x = _fieldMargin.x;
            newPos.y = -_fieldMargin.y;
        }
        else
        {
            RectTransform justBeforeLabelRect = referPool[labelIndexInList - 1].MyRect;

            newPos.x = justBeforeLabelRect.anchoredPosition.x;
            newPos.x += justBeforeLabelRect.sizeDelta.x;
            newPos.x += _labelSpacing.x;
            newPos.y = justBeforeLabelRect.anchoredPosition.y;

            //後ろの背景からはみ出さない限界値
            float limitLabelXPos = _labelFieldRect.sizeDelta.x - _fieldMargin.x - referPool[labelIndexInList].MyRect.sizeDelta.x;
            if(newPos.x > limitLabelXPos)
            {
                newPos.x = _fieldMargin.x;
                newPos.y -= _labelSpacing.y;
            }
        }

        return newPos;
    }

    /// <summary>
    /// 有効な一番最後のラベルの座標からラベルの列数を求める
    /// </summary>
    private int CalcLabelRowLength()
    {
        if (_activeStuckPool.Count == 0) return -1;
        float lastLabelYPos = _activeStuckPool[_activeStuckPool.Count - 1].MyRect.anchoredPosition.y;
        //一番上列の余白分を修正する
        lastLabelYPos += _fieldMargin.y;
        float rowLength = -(lastLabelYPos / _labelSpacing.y);
        int result = Mathf.FloorToInt(rowLength);
        //一番上の余白と一番下の余白で消された行1つぶんを加算
        result++;
        return result;
    }

    /// <summary>
    /// ラベルの行数に合わせて、フィールドの縦幅を調整する
    /// </summary>
    private void FlexibleLabelFieldHeight(int rowLength)
    {
        //必要な高さ = ラベル同士の間隔 * ラベル数　- 1 + ラベルと背景のマージン * 上下の２つぶん
        float fieldHeight = _labelSpacing.y * (rowLength - 1) + _fieldMargin.y * 2;
        //初期の高さが必要な高さより高い場合は調整を行わない
        if (_firstFieldSize.y >= fieldHeight) return;
        //現在の高さが必要な高さと同じ場合は調整を行わない
        Vector2 currentSize = _labelFieldRect.sizeDelta;
        if (currentSize.y == fieldHeight) return;

        //サイズを変更
        _labelFieldRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, fieldHeight);
    }
}
