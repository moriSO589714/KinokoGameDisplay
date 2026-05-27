using DG.Tweening;
using UnityEngine;

/// <summary>
/// DoTweenを用い、オブジェクトの拡大/縮小を行うクラス 
/// </summary>
public class ZoomInOut
{
    private GameObject _target;
    private Vector2 _firstScale;
    //拡大率
    private float _zoomScale;
    private float _zoomInSpeed;
    private float _zoomOutSpeed;
    private Tween _runtimeTween;
    public ZoomInOut(GameObject obj, float zoomScale, float zoomInSeconds, float zoomOutSeconds)
    {
        _firstScale = obj.transform.localScale;
        _target = obj;
        _zoomScale = zoomScale;
        _zoomInSpeed = zoomInSeconds;
        _zoomOutSpeed = zoomOutSeconds;
    }

    //対象のオブジェクトを拡大する
    public void ZoomIn()
    {
        if (_runtimeTween != null) _runtimeTween.Kill();
        //拡大時のスケールを計算
        Vector2 size = _firstScale * _zoomScale;
        _runtimeTween = _target.transform.DOScale(size, _zoomInSpeed);
    }

    public void ZoomOut()
    {
        if (_runtimeTween != null) _runtimeTween.Kill();
        _runtimeTween = _target.transform.DOScale(_firstScale, _zoomOutSpeed);
    }
}
