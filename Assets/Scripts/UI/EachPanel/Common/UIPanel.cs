using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanel : MonoBehaviour
{
    CommonStateManager stateManager;

    private void Awake()
    {
        stateManager = CommonStateManager.Instance;
    }

    //パネルを作成した時に実行する処理
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
        Destroy(this.gameObject);
    }
}
