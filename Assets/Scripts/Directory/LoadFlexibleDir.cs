using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 自由に変更することができるディレクトリ(ゲームが入っているフォルダなど)の場所を取得する
/// </summary>
public class LoadFlexibleDir
{
    //jsonファイルからDirectoryPathsに各フォルダのパスをセットする
    public void SetFlexibleDirByJson()
    {
        FlexibleDirs flexibleDirs = new FlexibleDirs();
        //パスが記載されたjsonファイルのパスを取得する
        string jsonFilePath = new AllDirs().DefinedDataPath;
        //jsonデータを読み取って代入
        flexibleDirs = JSONTools.ReadJSON<FlexibleDirs>(jsonFilePath);

        //typeof演算子を使い、System.Type(型情報)としてクラスを取得、GetFields()でフィールドを取得する。
        FieldInfo[] fields = typeof(FlexibleDirs).GetFields();
        
        AllDirs dirPaths = AllDirs.GetInstance();

        //FlexibleDirsから取得できたフィールドを利用し、AllDirsクラスに値を代入していく
        foreach (FieldInfo field in fields)
        {
            var targetValue = field.GetValue(flexibleDirs);
            if (targetValue != "" || targetValue != null)
            {
                field.SetValue(dirPaths, targetValue);
            }
        }
    }

}
