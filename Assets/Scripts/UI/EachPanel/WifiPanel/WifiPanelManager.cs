using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class WifiPanelManager : UIPanel
{
    [SerializeField] private UIActBase _closeMark;
    [SerializeField] private UIActBase _yesMark;
    [SerializeField] private UIActBase _noMark;
    [SerializeField] private GameObject _loadingSquarePref;
    [SerializeField] private Text _description;

    [SerializeField] private Vector2 _squarePos;
    private GameObject _gameBoxsManager = null;
    private GameObject _loadingSquare = null;
    private CancellationTokenSource _torkenSorce = new CancellationTokenSource();

    //初期状態を保存しておく
    private string _firstDescription;

    protected override void Awake()
    {
        _firstDescription = _description.text;
        base.Awake();
    }

    public override void InitPanel()
    {
        base.InitPanel();
        _gameBoxsManager = GameObject.FindGameObjectWithTag("GameBoxsManager");

        //停止ボタンを押したときの処理をボタンオブジェクトのデリゲートに設定
        _closeMark.ClickAct = OnCloseProc;
        _noMark.ClickAct = OnCloseProc;
        //yesボタンを押したときの処理をデリゲートに設定
        _yesMark.ClickAct = () => SetGameObjectProc().Forget();
    }

    protected override void OnCloseProc()
    {
        SetInitMode();
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
            _description.text = "通信中にエラーが発生しました。Log>>> " + e;
        }
    }

    private void SetInitMode()
    {
        _description.text = _firstDescription;
        _closeMark.gameObject.SetActive(true);
        _yesMark.gameObject.SetActive(true);
        _noMark.gameObject.SetActive(true);
        Destroy(_loadingSquare);
    }

    private void SetLoadingMode()
    {
        _description.text = "スプレッドシートからロードを取得しています";
        _closeMark.gameObject.SetActive(false);
        _yesMark.gameObject.SetActive(false);
        _noMark.gameObject.SetActive(false);
        _loadingSquare = Instantiate(_loadingSquarePref, parent: gameObject.transform);
    }

    private void SetErrorMode()
    {
        if (_loadingSquare != null) _loadingSquare.SetActive(false);
        _closeMark.gameObject.SetActive(true);
    }
}
