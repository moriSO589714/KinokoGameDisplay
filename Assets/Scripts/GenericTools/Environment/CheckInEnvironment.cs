using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 現在の環境が特定環境での実行かどうかを確認する
/// </summary>
public static class CheckInEnvironment
{
    //インターネット利用時に実際に通信を行うか
    public static bool isOnNet = false;
    //テスト環境でダウンロード処理時に発生させる遅延(秒)
    public static float waitSecondsOnDownload = 0.8f;
    //テスト環境でスプレッドシートの情報取得時に発生させる遅延(秒)
    public static float waitSecondsOnGetSheetData = 0.5f;

    /// <summary>
    /// 実行環境がUnityEditorかどうか、Editorならtrueを返す
    /// </summary>
    public static bool CheckInEditor()
    {
        #if UNITY_EDITOR
                return true;
        #else
                return false;
        #endif
    }

    public static bool CheckDoingNet()
    {
        #if UNITY_EDITOR
                if (isOnNet) return true;
                else return false;
        #else
                return true;
        #endif
    }
}
