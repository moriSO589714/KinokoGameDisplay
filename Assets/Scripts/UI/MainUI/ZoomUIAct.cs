using UnityEngine;

/// <summary>
/// 拡大縮小を行うUI用のコンポーネント
/// </summary>
public class ZoomUIAct : UIActBase
{
    [SerializeField] private float _zoomScale;
    [SerializeField] private float _zoomInSpeed;
    [SerializeField] private float _zoomOutSpeed;

    ZoomInOut zoomInOut;
    private void Awake()
    {
        zoomInOut = new ZoomInOut(gameObject, _zoomScale, _zoomInSpeed, _zoomOutSpeed);
    }

    //アニメーションの発火処理をオーバーライドして追加する
    public override void OnPointerEnter()
    {
        zoomInOut.ZoomIn();
        base.OnPointerEnter();
    }

    public override void OnPointerExit()
    {
        zoomInOut.ZoomOut();
        base.OnPointerExit();
    }
}
