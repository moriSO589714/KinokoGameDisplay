using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransferColor
{
    private Color _originColor;
    private Tween _runTimeTween;

    private Image _targetImage;
    private Color _customizeColor;
    private float _whenSelectedSpeed;
    private float _whenExitSpeed;
    public TransferColor(Image targetImage, Color customizeColor, float whenSelectedSpeed, float whenExitSpeed)
    {
        _originColor = targetImage.color;
        _targetImage = targetImage;
        _customizeColor = customizeColor;
        _whenSelectedSpeed = whenSelectedSpeed;
        _whenExitSpeed = whenExitSpeed;
    }

    //対象の色を変更する
    public void TranferCustomizeColor()
    {
        if (_runTimeTween != null) _runTimeTween.Kill();
        _runTimeTween = _targetImage.DOColor(_customizeColor, _whenSelectedSpeed);
    }

    public void TransferOriginColor()
    {
        if (_runTimeTween != null) _runTimeTween.Kill();
        _runTimeTween = _targetImage.DOColor(_originColor, _whenExitSpeed);
    }
}
