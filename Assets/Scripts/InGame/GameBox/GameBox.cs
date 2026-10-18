using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class GameBox : Box
{
    [SerializeField] Text Title;
    [SerializeField] int TitleWordsRemit;
    [SerializeField] Text DescriptionField;
    [SerializeField] int DescriptionWordsRemit;
    [SerializeField] UIActBase GameImage;
    [SerializeField] UIActBase ToLibraryButton;
    [SerializeField] GameStartButton StartButton;

    [SerializeField] Sprite NoImageSprite;

    [SerializeField] GameObject _loadingAnimPref;
    private GameObject _instantiatedAnimObj;
    private GameImageDownloaderForUI _gameImageDownloaderForUI;

    public Action EndImageLoading;

    public GameData _myGameData { get; private set; }

    private ForceReplaceWord _forceReplaceWord = new ForceReplaceWord();
    /// <summary>
    /// 各種データのセット
    /// </summary>
    public override void SetDataMyBox<T>(T originData)
    {
        base.SetDataMyBox(originData);

        //型のキャスト
        GameData originGameData = originData as GameData;
        _myGameData = originGameData;

        GameDataSetterForUI.SetPlainStrData(originGameData.GameTitle, TitleWordsRemit, Title);
        GameDataSetterForUI.SetPlainStrData(originGameData.GameDescription, DescriptionWordsRemit, DescriptionField);        
        SetImage(originGameData.GameID);
        SetStartButton();
    }

    //Displayの中で一番のクソコード。もっと分かりやすい方法は無かったのか
    public void SetImageDownloader(GameImageDownloaderForUI gameImageDownloaderForUI)
    {
        _gameImageDownloaderForUI = gameImageDownloaderForUI;
        GameImage.ClickAct += InstantiateLoadingAnimPref;
        GameImage.ClickAct += () => gameImageDownloaderForUI.OnClickAction
            (_myGameData, (string gameId) => SetImage(gameId), () => Destroy(_instantiatedAnimObj));
    }

    private void InstantiateLoadingAnimPref()
    {
        _instantiatedAnimObj = Instantiate(_loadingAnimPref);
        _instantiatedAnimObj.GetComponent<LoadingSquare>().ChangeParent(GameImage.gameObject);
    }

    public void SetStartButtonAct(Action<GameData ,GameStartButton> clickStartButtonAct)
    {
        StartButton.ClickAct = () => clickStartButtonAct(_myGameData, StartButton);
    }

    public void SetToLibraryButtonAct(Action<GameData> clickToLibraryButtonAct)
    {
        ToLibraryButton.ClickAct = () => clickToLibraryButtonAct(_myGameData); 
    }

    private void SetStartButton()
    {
        StartButton.NewSetGameData(_myGameData);
        StartButton.ChangeButtonImage();
    }

    public void SetImage(string gameId)
    {
        Image gameImageSource = GameImage.GetComponent<Image>();
        Sprite setSprite = NoImageSprite;
        if(gameId == "" || gameId == null)
        {
            gameImageSource.sprite = setSprite;
            return;
        }

        Sprite imageSprite = GetThumbnailImageSprite.LoadPicSpriteByImageName(gameId);
        if (imageSprite != null)
        {
            setSprite = imageSprite;
        }

        gameImageSource.sprite = setSprite;
    }
}
