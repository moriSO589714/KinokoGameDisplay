using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class GameBox : Box
{
    [SerializeField] Text Title;
    [SerializeField] int TitleWordsRemit;
    [SerializeField] Text DescriptionField;
    [SerializeField] int DescriptionWordsRemit;
    [SerializeField] UIActBase GameImage;
    [SerializeField] GameStartButton StartButton;
    [SerializeField] Text GameDirName;
    [SerializeField] int GameDirNameWordsRemit;

    [SerializeField] Sprite NoImageSprite;

    [SerializeField] GameObject _loadingAnimPref;
    private GameObject _instantiatedAnimObj;
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
        GameDataSetterForUI.SetPlainStrData(originGameData.GameDirName, GameDirNameWordsRemit, GameDirName);
        SetImage(originGameData.GameID);
        SetStartButton();
    }

    public void SetClickButtonAct(Action<GameData ,GameStartButton> clickStartButtonAct)
    {
        StartButton.ClickAct += () => clickStartButtonAct(_myGameData, StartButton);
    }

    public void SetClickImageAct(Action<GameBox> dlImageAct)
    {
        GameImage.ClickAct += () => dlImageAct(this);
        GameImage.ClickAct += () 
            => { GameObject instantiated = Instantiate(_loadingAnimPref);
                instantiated.GetComponent<LoadingSquare>().ChangeParent(GameImage.gameObject);
                _instantiatedAnimObj = instantiated;};
        EndImageLoading += () => { Destroy(_instantiatedAnimObj); };
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

        Sprite imageSprite = LoadPicSpriteByPath(gameId);
        if (imageSprite != null)
        {
            setSprite = imageSprite;
        }

        gameImageSource.sprite = setSprite;
    }

    /// <summary>
    /// ローカルパスから画像データをスプライトとして返す
    /// </summary>
    /// <param name="path"></param>
    /// <returns>画像のスプライト、読み取れない場合nulを返す</returns>
    private Sprite LoadPicSpriteByPath(string imageName)
    {
        AllDirs allDirs = AllDirs.GetInstance();
        string imageExtention = allDirs.ImageExtention;
        //gameIDから画像のパスにする
        string imageFilePath = Path.Combine(allDirs.ImageFolderPath, imageName + imageExtention);
        Sprite sprite = null;
        if (File.Exists(imageFilePath))
        {
            //画像データをバイト配列として読み込む
            byte[] imageFileBytes = File.ReadAllBytes(imageFilePath);
            //空のテクスチャを作成する
            Texture2D texture = new Texture2D(0, 0);
            //テクスチャにファイルデータをロードする
            texture.LoadImage(imageFileBytes);
            //スプライトデータに変換する
            sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
        }

        return sprite;
    }
}
