using UnityEngine;
using UnityEngine.UI;

public class ActivateAnotherUIAct : UIActBase
{
    [SerializeField] GameObject AnotherObject;
    [SerializeField] float _targetChangeAlpha;
    [SerializeField] float _activateTime;
    [SerializeField] float _deactivateTime;

    private SlowlyChangeAlpha slowlyChangeAlpha;

    private void Awake()
    {
        slowlyChangeAlpha = new SlowlyChangeAlpha(AnotherObject.GetComponent<Image>());
    }

    public override void OnPointerEnter()
    {
        slowlyChangeAlpha.ChangeAlpha(_targetChangeAlpha, _activateTime);
        base.OnPointerEnter();
    }

    public override void OnPointerExit()
    {
        slowlyChangeAlpha.ChangeOrigin(_deactivateTime);
        base.OnPointerExit();
    }
}