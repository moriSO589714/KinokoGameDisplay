using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UninstallPanelManager : UIPanel
{
    [SerializeField] private Text _description;
    [SerializeField] private UIActBase _closeButton;
    [SerializeField] private UIActBase _yesButton;
    [SerializeField] private UIActBase _noButton;
    [SerializeField] private GameObject _loadingSquarePref;

    private const string _descriptionText = "をアンインストールしますか？";

    private GameData _currentGameData;
    private GameDeleteManager _gameDeleteManager;

    [SerializeField] private Vector2 _squarePos;
    [SerializeField] private float _squareSize;
    private GameObject _loadingSquare;    

    protected override void Awake()
    {
        base.Awake();
        _gameDeleteManager = new GameDeleteManager();
    }

    public void SetData(GameData currentGameData)
    {
        _currentGameData = currentGameData;
        _description.text = _currentGameData.GameTitle + _descriptionText;
        _closeButton.ClickAct = OnCloseProc;
        _noButton.ClickAct = OnCloseProc;
        _yesButton.ClickAct = () => DoUninstall().Forget();
    }

    protected override void OnCloseProc()
    {
        SetInitMode();
        base.OnCloseProc();
    }

    private async UniTask DoUninstall()
    {
        SetLoadingUI();

        try
        {
            await UniTask.RunOnThreadPool(() => _gameDeleteManager.UninstallGame(_currentGameData));
            OnCloseProc();
        }
        catch (System.Exception e)
        {
            SetErrorMode();
            Debug.LogException(e);
            _description.text = "アンインストールに失敗しました";
        }
    }

    private void SetLoadingUI()
    {
        _description.text = "アンインストールを実行中";
        _closeButton.gameObject.SetActive(false);
        _yesButton.gameObject.SetActive(false);
        _noButton.gameObject.SetActive(false);

        _loadingSquare = Instantiate(_loadingSquarePref, parent: gameObject.transform);
        _loadingSquare.transform.localScale *= _squareSize;
        RectTransform squareRect = _loadingSquare.GetComponent<RectTransform>();
        squareRect.anchoredPosition += _squarePos;
    }

    private void SetInitMode()
    {
        _closeButton.gameObject.SetActive(true);
        _yesButton.gameObject.SetActive(true);
        _noButton.gameObject.SetActive(true);
        Destroy(_loadingSquare);
    }

    private void SetErrorMode()
    {
        _closeButton.gameObject.SetActive(true);
    }
}
