using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

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
    //ステート切り替え時に実行するメソッド
    private List<Func<CancellationToken, UniTask>> _onChangeActions = new();

    protected override void Awake()
    {
        base.Awake();
        _currentState = CommonStates.Load;
    }

    public async UniTask SetCurrentState(CommonStates staet)
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        
        //現在のステートの終了処理をする。
        await UniTask.WhenAll(_onChangeActions.Select(f => f(cts.Token)));
        //タスクの破棄
        cts.Cancel();
        _onChangeActions = new();
        
        
    }
    
    public void AddOnChangeAction(Func<CancellationToken, UniTask> action)
    {
        _onChangeActions.Add(action);
    }
}
