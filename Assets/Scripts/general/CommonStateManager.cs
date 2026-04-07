using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneStates
{
    First, //アプリ起動時のみ入るシーン
    Library,
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
    //ステート切り替え時に実行するメソッド(通常終了処理が記述される)
    private List<Func<CancellationToken, UniTask>> _onChangeActions = new();

    protected override void Awake()
    {
        base.Awake();
        _currentState = SceneStates.First;
    }

    public async UniTask SetCurrentState(SceneStates state)
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
            case SceneStates.First:
                break;
            case SceneStates.Library:
                SceneManager.LoadScene("Main");
                break;
            case SceneStates.Admin:
                break;
        }
    }
    
    public void AddOnChangeAction(Func<CancellationToken, UniTask> action)
    {
        _onChangeActions.Add(action);
    }
}
