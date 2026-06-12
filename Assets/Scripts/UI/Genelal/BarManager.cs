using UnityEngine;
using UnityEngine.UI;

public class BarManager
{
    Image _barImage;
    RectTransform _rectTransform;
    float _perWidth = -1;
    public BarManager(Image barImage)
    {
        _barImage = barImage;
        _rectTransform = _barImage.GetComponent<RectTransform>();
        //1パーセントあたりの幅
        _perWidth = _rectTransform.sizeDelta.x / 100;
    }

    public void SetPercentage(float percentage)
    {
        _rectTransform.sizeDelta = new Vector2((float)(percentage * _perWidth), _rectTransform.sizeDelta.y);
    }
}
