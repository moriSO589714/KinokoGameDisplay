using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// 解凍・圧縮作業の中で共通して使われる処理をまとめたもの
/// </summary>
public class FreezingGeneric
{
    /// <summary>
    /// 指定されたフォルダに必要な全てのファイル群が揃っているかを確認するメソッド
    /// </summary>
    /// <param name="splitedFilesPath">分割されたファイル群が入っているフォルダのパス</param>
    /// <param name="dlData">DLDataクラスに確認したかを代入する必要のある場合(代入が不必要な場合nullで可能)</param>
    /// <returns>不足しているデータのインデックス値の配列が返される。揃っている場合は要素数0の配列を返す</returns>
    public MistakeFiles hasAllRequiredData(string splitedFilesPath)
    {
        //不足しているファイルのインデックス値を格納するリスト
        List<long> lackFilesList = new List<long>();
        //不足以外に問題のあるファイル(フォルダ内に存在するが、存在するべきでないファイル)の"パス"を格納するリスト
        List<string> errorFilesList = new List<string>();
        string[] files = sortingFilesByPath(splitedFilesPath);

        //拡張子の最後が0で終わるファイルをLINQで検索
        var dLDataFiles = files.Where(x => {
            //拡張子から.を除いて取得
            string ext = Path.GetExtension(x).Substring(1);

            //linqのAll()は要素全てが一致している場合trueを返す, Length > 0は拡張子が無かった場合を避けるため
            return ext.Length > 0 && ext.All(c => c == '0');
        });
        dLDataFiles.ToArray();

        if(dLDataFiles.Count() > 1)
        {
            UnityEngine.Debug.Log("DLDataファイルが複数存在します。" + dLDataFiles.Count());
            throw new Exception();
        }
        else if(dLDataFiles.Count() == 0)
        {
            UnityEngine.Debug.Log("DLDataファイルが存在しません");
            throw new Exception();
        }

        //ファイルチェック時に使用するゲーム名を格納するためのDLDataクラスを作成して、.00ファイルのバイト配列からデータを入れる。
        byte[] dlDatabytes = File.ReadAllBytes(files[0]);
        DLData usedDLData = new DLData(dlDatabytes);

        //ファイルが重複している時にどうするかの判断、送信時ももしかしたら上手く行かないかもしれない
        long checkCounter = 0;
        List<long> lackFileNumber = new List<long>();
        for (int i = 0; i < files.Length; i++)
        {
            //サーチしているファイルのファイル名の取得
            string fileName = Path.GetFileNameWithoutExtension(files[i]);
            //ファイルの番号を取得する(拡張子の部分)
            string fileNumberString = Path.GetExtension(files[i]).Replace(".", "");
            long fileNumber = long.Parse(fileNumberString);

            //ファイル名が異なっていた場合
            if(fileName != usedDLData.FileName)
            {
                //ファイルのパスをリストに追加
                errorFilesList.Add(files[i]);
                //カウンターが次のループ時に1進んでしまうため、減算しておく
                checkCounter--;
            }
            //同じファイル番号を持つファイルが連続していた場合
            else if (fileNumber < checkCounter)
            {
                //ファイルのパスをリストに追加
                errorFilesList.Add(files[i]);
                checkCounter--;
            }
            //ファイルが一つ飛んでいる場合
            else if (fileNumber > checkCounter)
            {
                lackFileNumber.Add(fileNumber);
                i--;
            }
            checkCounter++;
        }
        MistakeFiles mistakeFiles = new MistakeFiles(lackFileNumber,errorFilesList);
        return mistakeFiles;
    }

    /// <summary>
    /// 引数のパスに入っているフォルダを拡張子の大きさでソートする
    /// </summary>
    /// <param name="splitedFilesPath">ファイル群が入っているフォルダのパス</param>
    /// <returns></returns>
    public string[] sortingFilesByPath(string splitedFilesPath)
    {
        //対象ディレクトリ内のファイル群のパスを取得してくる
        string[] splitedFiles = Directory.GetFiles(splitedFilesPath);
        //取得したファイルパス群から、語尾の数値でソートを行う(数値以外の部分は共通しているため、単純にソートを行うことが可能)
        //ToArray()はキャッシュ化してLINQの遅延実行を無視するため
        string[] sortedFiles = splitedFiles.OrderBy(f => f).ToArray();

        return sortedFiles;
    }

}

/// <summary>
/// ファイル群が全て揃っているかを確認した際に、不備のあるファイルが存在した場合に不備のあるファイル群の番号やパスを代入するクラス
/// </summary>
public class MistakeFiles
{
    //不足しているファイルの番号(拡張子部分の数字)
    public List<long> LackFiles { private set; get; }
    //問題のあるファイルのパス(ファイル名が異なる、番号が重複している等)
    public List<string> ErrorFilePathes { private set; get; }

    public MistakeFiles(List<long> lack, List<string> error)
    {
        SetDatas(lack, error);
    }

    public void SetDatas(List<long> lack, List<string> error)
    {
        LackFiles = lack;
        ErrorFilePathes = error;
    }
}

