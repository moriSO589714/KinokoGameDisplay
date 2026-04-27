using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 現在の環境が特定環境での実行かどうかを確認する
/// </summary>
public static class CheckInEnvironment
{
    //インターネット利用時に実際に通信を行うか
    private const bool isOnNet = false;

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
