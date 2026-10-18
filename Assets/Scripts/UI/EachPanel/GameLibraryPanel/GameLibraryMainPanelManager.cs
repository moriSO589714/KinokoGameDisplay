using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLibraryMainPanelManager : MonoBehaviour
{
    [SerializeField] private Text _titleText;
    [SerializeField] private Text _devNameText;
    [SerializeField] private Text _tagsText;
    [SerializeField] private Text _descriptionText;

    [SerializeField] private GameImageDownloaderForUI _gameImageDownloaderForUI;
    [SerializeField] private UIActBase _thumbnail;
    [SerializeField] private Sprite _noImageSprite;
    [SerializeField] private GameObject _loadingAnimPref;
    private GameObject _instantiatedLoadingAnim;

    [SerializeField] private DetailInfoManager _detailInfoManager;
    [SerializeField] private UIPanel _checkDlPanel;
    [SerializeField] private UIPanel _checkUpdatePanel;
    [SerializeField] private GameDlCue _gameDlCue;
    [SerializeField] private WatchingGameDlCueForUI _watchingGameDlCueForUI;
    [SerializeField] private GameStartButton _startButton;
    [SerializeField] private GameLibrarySubPanelManager _gameLibrarySubPanel;
    [SerializeField] private UIActBase _subPanelButton;

    [SerializeField] private GameLibrarySizeFitter _gameLibrarySizeFitter;

    private int _assignNolimit = -1;

    private GameStartButtonClickSwitcher _gameStartButtonClickChanger;
    private GameData _currentGameData;
    private Vector2 _currentFloorPos;

    private void Awake()
    {
        _gameStartButtonClickChanger = new GameStartButtonClickSwitcher(_checkDlPanel, _checkUpdatePanel, _gameDlCue, _watchingGameDlCueForUI);
    }

    public void SetData(GameData newGameData)
    {
        EndCurrentGameData();

        _currentGameData = newGameData;

        GameDataSetterForUI.SetPlainStrData(_currentGameData.GameTitle, _assignNolimit, _titleText);
        GameDataSetterForUI.SetPlainStrData(string.Join(",", _currentGameData.GameDevelopper), _assignNolimit, _devNameText);
        GameDataSetterForUI.SetPlainStrData(_currentGameData.GameDescription, _assignNolimit, _descriptionText);
        GameDataSetterForUI.SetPlainStrData("#" + string.Join(",#", _currentGameData.GameTags), _assignNolimit, _tagsText);
        _detailInfoManager.SetData(_currentGameData);
        _startButton.ClickAct = () => _gameStartButtonClickChanger.OnClickAction(newGameData, _startButton);
        _startButton.NewSetGameData(newGameData);
        _startButton.ChangeButtonImage();
        SetImage(_currentGameData.GameID);

        _subPanelButton.ClickAct = () =>
        {
            _gameLibrarySubPanel.gameObject.SetActive(true);
            _gameLibrarySubPanel.SetData(_currentGameData);
        };

        _thumbnail.ClickAct += InstantiateLoadingAnim;
        _thumbnail.ClickAct += () => _gameImageDownloaderForUI.OnClickAction(_currentGameData, (string gameId) => SetImage(gameId), () => Destroy(_instantiatedLoadingAnim));

        //UIの変更を即時実行
        Canvas.ForceUpdateCanvases();

        _currentFloorPos = _gameLibrarySizeFitter.AutoFit();
    }

    private void SetImage(string gameId)
    {
        //同じオブジェクトを違うものにも使うため
        if (gameId != _currentGameData.GameID)
        {
            return;
        }

        Image thumbnailImage = _thumbnail.gameObject.GetComponent<Image>();
        if(gameId == "" || gameId == null)
        {
            thumbnailImage.sprite = _noImageSprite;
            return;
        }

        Sprite setSprite = GetThumbnailImageSprite.LoadPicSpriteByImageName(gameId);
        if(setSprite != null)
        {
            thumbnailImage.sprite = setSprite;
        }
        else
        {
            thumbnailImage.sprite = _noImageSprite;
        }
    }

    private void InstantiateLoadingAnim()
    {
        _instantiatedLoadingAnim = Instantiate(_loadingAnimPref);
        _instantiatedLoadingAnim.GetComponent<LoadingSquare>().ChangeParent(_thumbnail.gameObject);
    }

    //ゲームデータが入れ替わる際の終了処理(_currentGameDataを入れ替える前に実行する)
    private void EndCurrentGameData()
    {
        if(_instantiatedLoadingAnim != null)
        {
            Destroy(_instantiatedLoadingAnim);
        }
    }
}
