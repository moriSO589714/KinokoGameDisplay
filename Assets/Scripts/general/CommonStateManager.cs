using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum CommonStates
{
    Load,
    Library,
    Admin,
}
public class CommonStateManager : BasedSingleton<CommonStateManager>
{
    //現在の状態
    [HideInInspector] public CommonStates _currentState { get; private set; }
    //ステート切り替え時に実行するメソッド(通常終了処理が記述される)
    private List<Func<CancellationToken, UniTask>> _onChangeActions = new();

    protected override void Awake()
    {
        base.Awake();
        _currentState = CommonStates.Load;
    }

    public async UniTask SetCurrentState(CommonStates state)
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        
        //現在のステートの終了処理をする。
        await UniTask.WhenAll(_onChangeActions.Select(f => f(cts.Token)));
        //タスクの破棄
        cts.Cancel();
        _onChangeActions = new();


        //シーン遷移を伴うなら、シーンを遷移させる（シーン遷移はここ以外から行わない）
        switch (state)
        {
            case CommonStates.Load:
                break;
            case CommonStates.Library:
                SceneManager.LoadScene("Main");
                break;
            case CommonStates.Admin:
                break;
        }
    }
    
    public void AddOnChangeAction(Func<CancellationToken, UniTask> action)
    {
        _onChangeActions.Add(action);
    }
}
