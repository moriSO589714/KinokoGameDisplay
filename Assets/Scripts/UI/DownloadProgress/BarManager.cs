using UnityEngine;

public class BarManager : MonoBehaviour
{
    RectTransform _rectTransform;
    float _perWidth = -1;

    private bool isInit = false;
    private void Awake()
    {
        if(!isInit) Init();
    }

    public void InitFromOther()
    {
        if(!isInit) Init();
    }

    private void Init()
    {
        _rectTransform = this.GetComponent<RectTransform>();
        _perWidth = _rectTransform.sizeDelta.x / 100;
        isInit = true;
    }

    public void SetPercentage(float percentage)
    {
        _rectTransform.sizeDelta = new Vector2((float)(percentage * _perWidth), _rectTransform.sizeDelta.y);
    }
}
