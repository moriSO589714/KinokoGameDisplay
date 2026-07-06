using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilterManagerFreeInput : MonoBehaviour
{
    [SerializeField] protected LabelFieldManager _labelFieldManager;
    [SerializeField] protected FreeInputManager _inputManager;

    protected GameDatasSingleton _gameDatasSingleton = null;

    private void Awake()
    {
        _gameDatasSingleton = GameDatasSingleton.Instance;
        _inputManager.SetSendInputValueAct(_labelFieldManager.AddLabel);
    }

    private void OnEnable()
    {
        Init();
    }

    public void PanelCloseProc()
    {
        ResetFirstState();
    }

    protected virtual void Init()
    {

    }

    protected void AddSetedConditions(List<List<string>> doubleConditions)
    {
        List<string> plain = new List<string>();
        for(int i = 0; i <= doubleConditions.Count - 1; i++)
        {
            plain.AddRange(doubleConditions[i]);
            if(i < doubleConditions.Count - 1)
            {
                plain.Add("or");
            }
        }
        AddSetedConditions(plain);
    }

    protected void AddSetedConditions(List<string> plain)
    {
        foreach(string label in plain)
        {
            _labelFieldManager.AddLabel(label);
        }
    }

    private void ResetFirstState()
    {
        //ラベルフィールドを初期化
        _labelFieldManager.ClearActiveStucks();

        //inputManagerの初期化
        _inputManager.RefleshField();
    }
}
