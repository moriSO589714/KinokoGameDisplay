using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneStates
{
    Main,
    Admin,
}
public enum LoadStates 
{
    NoLoading,　//ロードを行っていない
    MainLoading, //パネルを用い全画面を伴うロード
    MiniLoading, //画面の一部分のみを伴うロード
    BackLoading,　//画面上では分からないロード
}


public class CommonStateManager : BasedSingleton<CommonStateManager>
{
    //現在の状態
    [HideInInspector] public SceneStates _currentState { get; private set; }
    [HideInInspector] public LoadStates _currentLoadState { get; private set; }

    //ステート切り替え時に実行するメソッド(通常終了処理が記述される)
    private List<Func<CancellationToken, UniTask>> _onChangeFuncs = new();

    //ロードに入る際に実行するメソッド
    private List<Func<CancellationToken, UniTask>> _onMainLoadingFuncs = new();
    private List<Func<CancellationToken, UniTask>> _onMiniLoadingFuncs = new();
    private List<Func<CancellationToken, UniTask>> _onBackLoadingFuncs = new();

    //ロードから出る時に実行するメソッド
    private List<Func<CancellationToken, UniTask>> _outLoadFuncs = new();

    protected override void Awake()
    {
        base.Awake();
    }

    public async UniTask SetCurrentState(SceneStates state)
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        
        //現在のステートの終了処理をする。
        await UniTask.WhenAll(_onChangeFuncs.Select(f => f(cts.Token)));
        //タスクの破棄
        cts.Cancel();
        _onChangeFuncs = new();


        //シーン遷移を伴うなら、シーンを遷移させる（シーン遷移はここ以外から行わない）
        switch (state)
        {
            case SceneStates.Main:
                SceneManager.LoadScene("Main");
                break;
            case SceneStates.Admin:
                break;
        }

        _currentState = state;
    }

    public async UniTask SetCurrentLoad(LoadStates state)
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        switch (state) 
        {
            case LoadStates.MainLoading:
                await UniTask.WhenAll(_onMainLoadingFuncs.Select(f => f(cts.Token)));
                break;
            case LoadStates.MiniLoading:
                await UniTask.WhenAll(_onMiniLoadingFuncs.Select(f => f(cts.Token)));
                break;
            case LoadStates.BackLoading:
                await UniTask.WhenAll(_onBackLoadingFuncs.Select(f => f(cts.Token)));
                break;
            case LoadStates.NoLoading:
                await UniTask.WhenAll(_outLoadFuncs.Select(f => f(cts.Token)));
                break;
        }

        cts.Cancel();
        _currentLoadState = state;
    }

    public void AddOnChangeAction(Func<CancellationToken, UniTask> action)
    {
        _onChangeFuncs.Add(action);
    }
}
