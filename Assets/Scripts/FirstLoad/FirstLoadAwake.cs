using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// FirstLoadシーンで最初に動くスクリプト、初回のロード等。
/// </summary>
public class FirstLoadAwake : MonoBehaviour
{
    private CommonStateManager _commonStateManager;
    private void Awake()
    {
        //ステート管理クラスの取得、ステート変更時に行う処理を登録
        _commonStateManager = CommonStateManager.Instance;
        _commonStateManager.AddOnChangeAction(ToMainScene);
        
        
        
        //ステートを変更
        _commonStateManager.SetCurrentState(CommonStates.Library);
    }
    
    
    
    //ステート変更時に一緒に行う処理(後処理、シーン遷移アニメーションとか)
    private async UniTask ToMainScene(CancellationToken token)
    {

    }
    
}
