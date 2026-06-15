using DG.Tweening;
using UnityEngine;

/// <summary>
/// 左から右にズレるタイプのUIアニメーション
/// </summary>
public class SlideSideUIAct : UIActBase
{
    [SerializeField] private float SlideWidth = 0f;
    [SerializeField] private float SlideAnimSeconds = 0f;

    private RectTransform thisRectTransform;
    private Vector2 _firstPos;
    //現在実行しているdotween
    private Tween _runTween;
    private void Awake()
    {
        thisRectTransform = this.gameObject.GetComponent<RectTransform>();
        _firstPos = thisRectTransform.anchoredPosition;
    }

    public void OpeningAct()
    {
        thisRectTransform.anchoredPosition = _firstPos;
        _runTween = thisRectTransform.DOAnchorPosX(_firstPos.x + SlideWidth, SlideAnimSeconds);
    }

    public void HideThisTab()
    {
        //実行しているアニメーションを終了させる
        _runTween?.Complete();
        //初期位置に戻す
        thisRectTransform.anchoredPosition = _firstPos;
    }
}
