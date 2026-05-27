using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownTab : UIActBase
{
    [SerializeField] Vector2 _moveDistance;
    [SerializeField] float _moveSeconds;
    [SerializeField] float _removeSeconds;

    SimpleDownAndUp simpleDownAndUp;
    private void Awake()
    {
        simpleDownAndUp = new SimpleDownAndUp(gameObject, _moveDistance, _moveSeconds, _removeSeconds);
    }

    public override void OnPointerEnter()
    {
        simpleDownAndUp.MoveObject();
        base.OnPointerEnter();
    }

    public override void OnPointerExit()
    {
        simpleDownAndUp.RemoveObject();
        base.OnPointerExit();
    }
}
