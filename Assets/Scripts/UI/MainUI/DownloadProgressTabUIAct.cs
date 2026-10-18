using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DownloadProgressTabUIAct : UIActBase
{
    [SerializeField] private float SlideWidth = 0f;
    [SerializeField] private float SlideAnimSeconds = 0f;

    private RectTransform thisRectTransform;
    private Vector2 _firstPos;

    [SerializeField] private Color _targetColor;
    [SerializeField] private float _inColorSpeed;
    [SerializeField] private float _outColorSpeed;
    private TransferColor _transferColor;

    //現在実行しているdotween
    private Tween _runTweenForSlide;
    
    private void Awake()
    {
        thisRectTransform = this.gameObject.GetComponent<RectTransform>();
        _firstPos = thisRectTransform.anchoredPosition;

        _transferColor = new TransferColor(this.GetComponent<Image>(), _targetColor, _inColorSpeed, _outColorSpeed);
    }

    public override void OnPointerEnter()
    {
        _transferColor.TranferCustomizeColor();
        base.OnPointerEnter();
    }

    public override void OnPointerExit()
    {
        _transferColor.TransferOriginColor();
        base.OnPointerExit();
    }

    public void OpeningAct()
    {
        thisRectTransform.anchoredPosition = _firstPos;
        _runTweenForSlide = thisRectTransform.DOAnchorPosX(_firstPos.x + SlideWidth, SlideAnimSeconds);
    }

    public void HideThisTab()
    {
        //実行しているアニメーションを終了させる
        _runTweenForSlide?.Complete();
        //初期位置に戻す
        thisRectTransform.anchoredPosition = _firstPos;
    }
}
