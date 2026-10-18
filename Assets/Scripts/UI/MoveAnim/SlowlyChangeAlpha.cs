
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SlowlyChangeAlpha
{
    private Image _chageAlphaTarget;

    private float _firstAlpha;
    private Tween _runtimeTween;

    public SlowlyChangeAlpha(Image targetImage)
    {
        _firstAlpha = targetImage.color.a;
        _chageAlphaTarget = targetImage;
    }

    public void ChangeAlpha(float targetAlpha, float fadeTime)
    {
        if (_runtimeTween != null) _runtimeTween.Kill();
        _runtimeTween = _chageAlphaTarget.DOFade(targetAlpha, fadeTime);
    }

    public void ChangeOrigin(float fadeTime)
    {
        if (_runtimeTween != null) _runtimeTween.Kill();
        _runtimeTween = _chageAlphaTarget.DOFade(_firstAlpha, fadeTime);
    }
}
