using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class RayPassEventSystem : MonoBehaviour
{
    public Action<GameObject> PointerEnterAct;
    public Action<GameObject> PointerExitAct;

    private List<GameObject> _newHitObject = new List<GameObject>();
    private List<GameObject> _lastHitObject = new List<GameObject>();
    private List<RaycastResult> raycastResults = new List<RaycastResult>();

    void Update()
    {
        //現在のマウスポインタの場所を取得
        Vector2 screenMousePos = Input.mousePosition;
        PointerEventData eventDataCurrent = new PointerEventData(EventSystem.current);
        eventDataCurrent.position = screenMousePos;

        EventSystem.current.RaycastAll(eventDataCurrent, raycastResults);

        _newHitObject.Clear();
        foreach (RaycastResult result in raycastResults)
        {
            if (!_lastHitObject.Contains(result.gameObject))
            {
                _newHitObject.Add(result.gameObject);
                PointerEnterAct?.Invoke(result.gameObject);
            }
            else
            {
                _newHitObject.Add(result.gameObject);
                _lastHitObject.Remove(result.gameObject);
            }          
        }

        foreach(GameObject _notHit in _lastHitObject)
        {
            PointerExitAct?.Invoke(_notHit);
        }

        _lastHitObject.Clear();
        _lastHitObject.AddRange(_newHitObject);
        raycastResults.Clear();
    }
}