using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class WifiPanelManager : UIPanel
{
    [SerializeField] private UIActBase CloseMark;
    [SerializeField] private UIActBase YesMark;
    [SerializeField] private UIActBase NoMark;
    [SerializeField] private GameObject LoadingSquare;
    [SerializeField] private Text Description;

    [SerializeField] private Vector2 _squarePos;
    private GameObject _gameBoxsManager = null;
    private GameObject _loadingSquare = null;
    private CancellationTokenSource _torkenSorce = new CancellationTokenSource();
    public override void InitPanel()
    {
        base.InitPanel();
        _gameBoxsManager = GameObject.FindGameObjectWithTag("GameBoxsManager");

        //停止ボタンを押したときの処理をボタンオブジェクトのデリゲートに設定
        CloseMark.ClickAct = OnCloseProc;
        NoMark.ClickAct = OnCloseProc;
        //yesボタンを押したときの処理をデリゲートに設定
        YesMark.ClickAct = () => SetGameObjectProc().Forget();
    }

    protected override void OnCloseProc()
    {
        base.OnCloseProc();
        _torkenSorce.Cancel();
    }

    //キャンセルトークンわたすように！
    private async UniTask SetGameObjectProc()
    {
        if (_gameBoxsManager == null) return;

        //ボタン等を非表示にする
        SetLoadingMode();

        SetGameBoxs setGameBoxs = new SetGameBoxs(_gameBoxsManager.GetComponent<GameBoxsManager>());
        try
        {
            await setGameBoxs.SetAllGameBoxfromNet();
            OnCloseProc();
        }
        catch (Exception e)
        {
            SetErrorMode();
            Description.text = "通信中にエラーが発生しました。Log>>> " + e;
        }
    }

    private void SetLoadingMode()
    {
        Description.text = "スプレッドシートからロードを取得しています";
        CloseMark.gameObject.SetActive(false);
        YesMark.gameObject.SetActive(false);
        NoMark.gameObject.SetActive(false);
        _loadingSquare = Instantiate(LoadingSquare, parent: gameObject.transform);
    }

    private void SetErrorMode()
    {
        if (_loadingSquare != null) _loadingSquare.SetActive(false);
        CloseMark.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        OnCloseProc();
    }
}
