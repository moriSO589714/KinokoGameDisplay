using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateUIAct : UIActBase
{
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _rotationAngle;

    private bool _isRotated = false;
    private RotationAnim _rotationAnim;

    private void Awake()
    {
        _rotationAnim = new RotationAnim(this.gameObject, _rotationSpeed, _rotationAngle);
    }

    public void SwitchRotate()
    {
        if (_isRotated)
        {
            _rotationAnim.RotateReturn();
            _isRotated = false;
        }
        else
        {
            _rotationAnim.Rotate();
            _isRotated = true;
        }
    }
}
