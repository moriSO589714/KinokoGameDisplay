using System;
using UnityEngine;

/// <summary>
/// ボタンが重なっており、同時に判定を取りたい時用のクラス
/// このクラスが付いたオブジェクトを最も下のレイヤーに位置させる必要がある
/// </summary>
public class RayPassHitPanel : MonoBehaviour
{
    [SerializeField] private RayPassEventSystem _rayPassEventSystem;

    [SerializeField] private Action _pointerEnterAct;
    [SerializeField] private Action _pointerExitAct;
    [SerializeField] private Action _pointerClick;

    public bool _isPointing { get; private set; } = false;

    private void OnEnable()
    {
        _rayPassEventSystem.PointerEnterAct += PointerEnter;
        _rayPassEventSystem.PointerExitAct += PointerExit;
    }

    private void OnDisable()
    {
        _rayPassEventSystem.PointerEnterAct -= PointerEnter;
        _rayPassEventSystem.PointerExitAct -= PointerExit;
    }

    private void PointerEnter(GameObject hitGameObject)
    {
        //当たったオブジェクトが自身かどうか判定
        if (hitGameObject != this.gameObject) return;
        _isPointing = true;

        _pointerEnterAct?.Invoke();
    }

    private void PointerExit(GameObject hitGameObject)
    {
        if (hitGameObject != this.gameObject) return;
        _isPointing = false;

        _pointerExitAct?.Invoke();
    }
}
