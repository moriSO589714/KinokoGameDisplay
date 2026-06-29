using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxManager : MonoBehaviour
{
    [SerializeField] float _boxInterval;
    [SerializeField] float _scrollWidth;

    protected RectTransform _myRectTransform;
    protected float _firstTimeMyYPos;
    protected List<Box> _boxPool = new List<Box>();
    protected float _lastBoxYPos = 0;

    protected virtual void Awake()
    {
        _myRectTransform = this.GetComponent<RectTransform>();
        _firstTimeMyYPos = _myRectTransform.anchoredPosition.y;
        _lastBoxYPos = _firstTimeMyYPos;
    }

    /// <summary>
    /// スクロールさせるメソッド
    /// インプットを管理しているクラスにデリゲートとして呼ばせる
    /// </summary>
    /// <param name="scrollDirection">スクロール方向</param>
    public virtual void OnScroll(float scrollDirection)
    {
        float targetYPos = 0;
        if(scrollDirection < 0)//上へスクロール
        {
            targetYPos = _myRectTransform.anchoredPosition.y + _scrollWidth;
            float limitYPos = (_boxPool.Count - 1) * _boxInterval;
            if(targetYPos >= limitYPos)
            {
                targetYPos = limitYPos;
            }
        }
        else if(scrollDirection > 0)//下へ
        {
            targetYPos = _myRectTransform.anchoredPosition.y - _scrollWidth;
            if(targetYPos <= _firstTimeMyYPos)
            {
                targetYPos = _firstTimeMyYPos;
            }
        }

        _myRectTransform.anchoredPosition = new Vector2(_myRectTransform.anchoredPosition.x, targetYPos);
    }

    protected virtual GameObject InstanceBox<T>(T originData, float lastBoxYPos, GameObject boxPref)
    {
        float createYPos = lastBoxYPos - _boxInterval;
        GameObject instancedBox = Instantiate(boxPref);
        RectTransform instancedRectTransform = instancedBox.GetComponent<RectTransform>();
        instancedRectTransform.anchoredPosition = new Vector2(instancedRectTransform.anchoredPosition.x, createYPos);
        instancedRectTransform.transform.SetParent(this.transform, false);
        Box box = instancedBox.GetComponent<Box>();
        box.SetDataMyBox(originData);

        _boxPool.Add(box);
        return instancedBox;
    }

    public virtual void ClearField()
    {
        List<Box> copyPool = new List<Box>(_boxPool);
        for(int i = _boxPool.Count - 1; i >= 0; i--)
        {
            GameObject.Destroy(_boxPool[i].gameObject);
            _boxPool.RemoveAt(i);
        }

        _lastBoxYPos = _firstTimeMyYPos;
    }
}
