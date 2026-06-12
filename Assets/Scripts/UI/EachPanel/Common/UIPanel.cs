using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanel : MonoBehaviour
{
    protected CommonStateManager stateManager;

    protected virtual void Awake()
    {
        stateManager = CommonStateManager.Instance;
    }

    protected virtual void OnEnable()
    {
        InitPanel();
    }

    //初期化用処理
    public virtual void InitPanel()
    {
        //ステートの変更
        stateManager.SetCurrentLoad(LoadStates.MiniLoading);
    }

    //パネルを閉じる際に行う処理
    protected virtual void OnCloseProc()
    {
        //ロードステートを変更する
        stateManager.SetCurrentLoad(LoadStates.NoLoading);

        //オブジェクトを消す
        this.gameObject.SetActive(false);
    }
}
