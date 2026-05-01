using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class GameBox : MonoBehaviour
{
    [SerializeField] Text Title;
    [SerializeField] int TitleWordsRemit;
    [SerializeField] Text DescriptionField;
    [SerializeField] int DescriptionWordsRemit;
    [SerializeField] Image GameImage;
    [SerializeField] GameObject StartButton;
    [SerializeField] Text GameDirName;
    [SerializeField] int GameDirNameWordsRemit;

    [SerializeField] Sprite NoImageSprite;
    public GameData _thisGameData { get; private set; }

    /// <summary>
    /// ゲームボックスに各種データをセットする
    /// </summary>
    public void SetDataByGameData(GameData gameData)
    {
        _thisGameData = gameData;
        SetTitle(gameData.GameTitle);
        SetDescription(gameData.GameDescription);
        SetImage(gameData.GameImageName);
        SetGameDirName(gameData.GameDirName);
    }

    /// <summary>
    /// タイトルをロードする
    /// </summary>
    /// <param name="gameTitle"></param>
    public void SetTitle(string gameTitle)
    {
        if (gameTitle == null || gameTitle == "") return;
        string setStr = ReplaceOverWords(gameTitle, TitleWordsRemit);
        Title.text = setStr;
    }
    public void SetDescription(string description)
    {
        if (description == "" || description == null) return;
        string setDescription = ReplaceOverWords(description, DescriptionWordsRemit);
        DescriptionField.text = setDescription;
    }
    public void SetImage(string imageName)
    {
        Sprite setSprite = NoImageSprite;
        if(imageName == "" || imageName == null)
        {
            GameImage.sprite = setSprite;
            return;
        }

        Sprite imageSprite = LoadPicSpriteByPath(imageName);
        if (imageSprite != null)
        {
            setSprite = imageSprite;
        }

        GameImage.sprite = setSprite;
    }
    public void SetButton(GameStatus gameStatus)
    {
        if(gameStatus == GameStatus.Downloaded)
        {

        }
        else if(gameStatus == GameStatus.UpdateAvailable)
        {

        }
        else if(gameStatus == GameStatus.NotDownloaded)
        {

        }
    }

    public void SetGameDirName(string gameDirName)
    {
        if (gameDirName == null || gameDirName == "") return;
        string setGameDirName = ReplaceOverWords(gameDirName, GameDirNameWordsRemit);
        GameDirName.text = setGameDirName;
    }

    /// <summary>
    /// ローカルパスから画像データをスプライトとして返す
    /// </summary>
    /// <param name="path"></param>
    /// <returns>画像のスプライト、読み取れない場合nulを返す</returns>
    Sprite LoadPicSpriteByPath(string imageName)
    {
        AllDirs allDirs = AllDirs.GetInstance();
        //画像ファイルの名前からパスに変換する
        string imageFilePath = Path.Combine(allDirs.ImageFolderPath, imageName);
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

    /// <summary>
    /// 文字数以上の部分を削除して...に置き換える。wordLimitsは...分の3文字も含まれる
    /// </summary>
    /// <param name="wordLimits"></param>
    /// <returns></returns>
    string ReplaceOverWords(string targetStr, int wordLimits)
    {
        string returnStr = targetStr;
        //制限された文字数以上であった場合以下の処理を実行する。
        if(targetStr.Length > wordLimits)
        {
            //余剰分以外を抽出
            string splited = targetStr.Substring(0, wordLimits - 3);
            //...を文章に付け加える
            returnStr = splited + "...";
        }

        return returnStr;
    }
}
