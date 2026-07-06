using DG.Tweening;
using UnityEngine;

/// <summary>
/// UIを半回転させるアニメーション
/// </summary>
public class RotationAnim
{
    private GameObject _targetObj;
    private float _animationSpeed;
    private float _rotateAngle;
    private float _firstAngle;
    private RectTransform _targetObjRect;
    private Tween _runtimeTween;

    public RotationAnim(GameObject targetObj, float animationSpeed, float rotateAngle)
    {
        _targetObj = targetObj;
        _animationSpeed = animationSpeed;
        _rotateAngle = rotateAngle;
        _targetObjRect = _targetObj.GetComponent<RectTransform>();
    }

    public void Rotate()
    {
        if (_runtimeTween != null) _runtimeTween.Kill();
        _runtimeTween = _targetObjRect.DOLocalRotate(new Vector3(0, 0, _targetObjRect.localEulerAngles.z + _rotateAngle), _animationSpeed);
    }

    public void RotateReturn()
    {
        if (_runtimeTween != null) _runtimeTween.Kill();
        _runtimeTween = _targetObjRect.DOLocalRotate(new Vector3(0, 0, _targetObjRect.localEulerAngles.z - _rotateAngle), _animationSpeed);
    }
}