using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 2個目以降のパネル用クラス
/// ※ステートの変更を行わない
/// </summary>
public class OverrapUIPanel : UIPanel
{
    /// <summary>
    /// 初期化用処理
    /// </summary>
    public override void InitPanel()
    {
        
    }

    protected override void OnCloseProc()
    {
        this.gameObject.SetActive(false);
    }
}
