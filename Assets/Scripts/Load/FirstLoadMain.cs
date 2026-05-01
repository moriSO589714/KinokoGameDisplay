using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Mainシーンで最初に動く初期化処理
/// </summary>
public class FirstLoadMain : MonoBehaviour
{
    [SerializeField] EachDataLoad _eachDataLoad;
    private CommonStateManager _commonStateManager;
    //Mainシーンに入った時に行う処理
    private void Awake()
    {
        //ステート管理クラスの取得、ステート変更時に行う処理を登録
        _commonStateManager = CommonStateManager.Instance;
        _commonStateManager.AddOnChangeAction(ToMainScene);

        //ローディング移行時の処理をStateManagerに代入===============


        //==========================================================

        //ディレクトリ系の初期化
        new LoadFlexibleDir().SetFlexibleDirByJson();
        //ローカルで保存されているゲームのロード＋UI反映
        _eachDataLoad?.LoadLocalData();
    }
    
    
    
    //ステート変更時に一緒に行う処理(後処理、シーン遷移アニメーションとか)
    private async UniTask ToMainScene(CancellationToken token)
    {

    }
    
}
