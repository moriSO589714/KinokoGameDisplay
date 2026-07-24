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
    [SerializeField] Image GameImage;
    [SerializeField] UIActBase StartButton;
    [SerializeField] Text GameDirName;
    [SerializeField] int GameDirNameWordsRemit;

    [SerializeField] Sprite NoImageSprite;
    [SerializeField] Sprite StartButtonSprite;
    [SerializeField] Sprite DownloadButtonSprite;
    [SerializeField] Sprite DownloadingButtonSprite;
    public GameData _myGameData { get; private set; }

    /// <summary>
    /// 各種データのセット
    /// </summary>
    public override void SetDataMyBox<T>(T originData)
    {
        base.SetDataMyBox(originData);

        //型のキャスト
        GameData originGameData = originData as GameData;
        _myGameData = originGameData;
        SetTitle(originGameData.GameTitle);
        SetDescription(originGameData.GameDescription);
        SetImage(originGameData.GameID);
        SetGameDirName(originGameData.GameDirName);
        ChangeButtonImage();
    }

    public void SetClickButtonAct(Action<GameBox> clickStartButtonAct)
    {
        StartButton.ClickAct += () => clickStartButtonAct(this);
    }

    private void SetTitle(string gameTitle)
    {
        if (gameTitle == null || gameTitle == "") return;
        string setStr = StrTools.ReplaceOverWords(gameTitle, TitleWordsRemit);
        Title.text = setStr;
    }
    private void SetDescription(string description)
    {
        if (description == "" || description == null) return;
        string setDescription = StrTools.ReplaceOverWords(description, DescriptionWordsRemit);
        DescriptionField.text = setDescription;
    }
    private void SetImage(string gameId)
    {
        Sprite setSprite = NoImageSprite;
        if(gameId == "" || gameId == null)
        {
            GameImage.sprite = setSprite;
            return;
        }

        Sprite imageSprite = LoadPicSpriteByPath(gameId);
        if (imageSprite != null)
        {
            setSprite = imageSprite;
        }

        GameImage.sprite = setSprite;
    }

    public void ChangeButtonImage()
    {
        Image buttonImage = StartButton.gameObject.GetComponent<Image>();
        switch (_myGameData.Status) 
        {
            case GameStatus.ByLocal:
                buttonImage.sprite = StartButtonSprite;
                break;
            case GameStatus.Downloaded:
                buttonImage.sprite = StartButtonSprite;
                break;
            case GameStatus.UpdateAvailable:
                buttonImage.sprite = StartButtonSprite;
                break;
            case GameStatus.NotDownloaded:
                buttonImage.sprite = DownloadButtonSprite;
                break;
            case GameStatus.Downloading:
                buttonImage.sprite = DownloadingButtonSprite;
                break;
        }
    }

    private void SetGameDirName(string gameDirName)
    {
        if (gameDirName == null || gameDirName == "") return;
        string setGameDirName = StrTools.ReplaceOverWords(gameDirName, GameDirNameWordsRemit);
        GameDirName.text = setGameDirName;
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
