using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneStates
{
    Main,
    Command,
}
public enum PanelStates 
{
    NoPanel,　//パネルを表示していない
    LargePanel, //パネルを表示している
}

public class CommonStateManager : BasedSingletonInMono<CommonStateManager>
{
    //現在の状態
    [HideInInspector] public SceneStates _currentState { get; private set; }
    [HideInInspector] public PanelStates _currentPanelState { get; private set; }

    //ステート切り替え時に実行するメソッド(通常終了処理が記述される)
    private List<Func<CancellationToken, UniTask>> _onChangeFuncs = new();

    //パネルを表示させる際に実行するメソッド
    private List<Func<CancellationToken, UniTask>> _inPanelFunc = new();
    private List<UIPanel> _currentDisplayPanel = new List<UIPanel>();

    //ロードから出る時に実行するメソッド
    private List<Func<CancellationToken, UniTask>> _outPanelFunc = new();

    public async UniTask SetCurrentState(SceneStates state)
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        
        _inPanelFunc.Clear();
        _outPanelFunc.Clear();

        //現在のステートの終了処理をする。
        await UniTask.WhenAll(_onChangeFuncs.Select(f => f(cts.Token)));
        //パネルステートの初期化処理を行う
        List<UIPanel> copyedCurrentPanels = new List<UIPanel>(_currentDisplayPanel);
        foreach(UIPanel panel in copyedCurrentPanels)
        {
            panel.OnCloseProc();
        }
        _currentDisplayPanel.Clear();
        
        //タスクの破棄
        cts.Cancel();
        _onChangeFuncs = new();

        //シーン遷移を伴うなら、シーンを遷移させる（シーン遷移はここ以外から行わない）
        switch (state)
        {
            case SceneStates.Main:
                SceneManager.LoadScene("Main");
                break;
            case SceneStates.Command:
                SceneManager.LoadScene("Command");
                break;
        }

        _currentState = state;
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public async UniTask SetExistPanelState(PanelStates state, UIPanel panel)
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        switch (state) 
        {
            case PanelStates.LargePanel:
                //現在表示しているパネルとしてリストに追加しておく
                _currentDisplayPanel.Add(panel);                
                await UniTask.WhenAll(_inPanelFunc.Select(f => f(cts.Token)));
                _currentPanelState = state;
                break;
            case PanelStates.NoPanel:
                _currentDisplayPanel.Remove(panel);
                if(_currentDisplayPanel.Count == 0)
                {
                    await UniTask.WhenAll(_outPanelFunc.Select(f => f(cts.Token)));
                    _currentPanelState = state;
                }
                break;
        }

        cts.Cancel();
    }

    public void AddOnChangeFunc(Func<CancellationToken, UniTask> func)
    {
        _onChangeFuncs.Add(func);
    }

    public void AddInPanelFunc(Func<CancellationToken, UniTask> func)
    {
        _inPanelFunc.Add(func);
    }

    public void AddInPanelFunc(Action act)
    {
        _inPanelFunc.Add(x =>
        {
            act?.Invoke();
            return UniTask.CompletedTask;
        });
    }

    public void AddOutPanelFunc(Func<CancellationToken, UniTask> func)
    {
        _outPanelFunc.Add(func);
    }

    public void AddOutPanelFunc(Action act)
    {
        _outPanelFunc.Add(x =>
        {
            act?.Invoke();
            return UniTask.CompletedTask;
        });
    }
}
