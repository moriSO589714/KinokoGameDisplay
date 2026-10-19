using System.Reflection;
using System;
using System.IO;

/// <summary>
/// 自由に変更することができるディレクトリ(ゲームが入っているフォルダなど)の場所を取得する
/// </summary>
public class LoadFlexibleDir
{
    //jsonファイルからDirectoryPathsに各フォルダのパスをセットする
    public void SetFlexibleDirByJson()
    {
        AllDirs dirPaths = AllDirs.GetInstance();
        FlexibleDirs flexibleDirs = new FlexibleDirs();
        //パスが記載されたjsonファイルのパスを取得する
        string jsonFilePath = new AllDirs().DefinedDataPath;
        try
        {
            //jsonデータを読み取って代入
            flexibleDirs = JSONTools.ReadJSON<FlexibleDirs>(jsonFilePath);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogException(e);
        }

        if(flexibleDirs == null)
        {
            flexibleDirs = new FlexibleDirs();
            //読み取れなかった場合はデフォルト値でjsonファイルを作成する
            JSONTools.SerializeJson<FlexibleDirs>( flexibleDirs, dirPaths.DefinedDataPath);
            SetFlexibleDirByJson();
            return;
        }

        //typeof演算子を使い、System.Type(型情報)としてクラスを取得、GetFields()でフィールドを取得する。
        FieldInfo[] fields = typeof(FlexibleDirs).GetFields();
        

        //FlexibleDirsから取得できたフィールドを利用し、AllDirsクラスに値を代入していく
        foreach (FieldInfo field in fields)
        {
            var targetValue = field.GetValue(flexibleDirs);
            if (targetValue != "" || targetValue != null)
            {
                field.SetValue(dirPaths, targetValue);
            }
        }

        CheckDirsExist(dirPaths);
    }

    /// <summary>
    /// ディレクトリの存在確認、なければ作る
    /// </summary>
    private void CheckDirsExist(AllDirs allDirs)
    {
        DirectoryActs.CreateAndCheckDir(allDirs.GameFilePath);
        DirectoryActs.CreateAndCheckDir(allDirs.JsonsDirPath);
        DirectoryActs.CreateAndCheckDir(allDirs.ImageFolderPath);
        DirectoryActs.CreateAndCheckDir(allDirs.TmpDLPath);
        DirectoryActs.CreateAndCheckDir(allDirs.TmpUpPath);
    }
}
