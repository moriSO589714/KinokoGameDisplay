using System.IO;
using UnityEngine;

public static class GetThumbnailImageSprite
{
    public static Sprite LoadPicSpriteByImageName(string imageName)
    {
        AllDirs allDirs = AllDirs.GetInstance();
        string imageExtention = allDirs.ImageExtention;
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
