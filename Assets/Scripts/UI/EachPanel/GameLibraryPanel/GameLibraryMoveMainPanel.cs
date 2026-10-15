using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLibraryMoveMainPanel : MonoBehaviour
{
    [SerializeField] private GameLibrarySizeFitter _sizeFitter;
    [SerializeField] private RayPassHitPanel _hitPanel;
    [SerializeField] private float _floorMargin;
    [SerializeField] private float _scrollWidth;
    private RectTransform _myRectTransform;
    private float _firstTimeMyYPos;

    private void Awake()
    {
        _myRectTransform = GetComponent<RectTransform>();
        _firstTimeMyYPos = _myRectTransform.anchoredPosition.y;
    }

    public void OnScroll(float scrollDirection)
    {
        if (!_hitPanel._isPointing) return;

        float targetYPos = 0;
        if (scrollDirection < 0) //上方向
        {
            targetYPos = _myRectTransform.anchoredPosition.y + _scrollWidth;
            float limitYPos = _sizeFitter._currentFloorPos.y * -1 + _floorMargin; //FloorPosは必ず負の値となるため-1をかける
            if (targetYPos >= limitYPos)
            {
                targetYPos = limitYPos;
            }
        }
        else if (scrollDirection > 0) //下方向
        {
            targetYPos = _myRectTransform.anchoredPosition.y - _scrollWidth;
            if (targetYPos <= _firstTimeMyYPos)
            {
                targetYPos = _firstTimeMyYPos;
            }
        }

        _myRectTransform.anchoredPosition = new Vector2(_myRectTransform.anchoredPosition.x, targetYPos);
    }
}
