using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class GameStartButton : ZoomUIAct
{
    private Image _myImage;
    public GameData MyGameData { get; private set; }

    [SerializeField] private Sprite _startButtonSprite;
    [SerializeField] private Sprite _downloadButtonSprite;
    [SerializeField] private Sprite _downloadingButtonSprite;
    [SerializeField] private GameObject _existNewVersionMrk;

    protected override void Awake()
    {
        base.Awake();
        _myImage = this.GetComponent<Image>();
    }

    public void NewSetGameData(GameData newGameData)
    {
        MyGameData = newGameData;
        ChangeButtonImage();
    }

    public void ChangeButtonImage()
    {
        switch (MyGameData.Status)
        {
            case GameStatus.ByLocal:
                _myImage.sprite = _startButtonSprite;
                break;
            case GameStatus.Downloaded:
                _myImage.sprite = _startButtonSprite;
                break;
            case GameStatus.UpdateAvailable:
                _myImage.sprite = _startButtonSprite;
                break;
            case GameStatus.NotDownloaded:
                _myImage.sprite = _downloadButtonSprite;
                break;
            case GameStatus.Downloading:
                _myImage.sprite = _downloadingButtonSprite;
                break;
        }
        ActivateExistNewVersionMrk(MyGameData.Status);
    }

    private void ActivateExistNewVersionMrk(GameStatus gameStatus)
    {
        if (gameStatus == GameStatus.UpdateAvailable)
        {
            _existNewVersionMrk.SetActive(true);
        }
        else
        {
            _existNewVersionMrk.SetActive(false);
        }
    }
}
