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
    [SerializeField] ManageMainUI _managedMainUI;
    private CommonStateManager _commonStateManager;

    //Mainシーンに入った時に行う処理
    private void Awake()
    {
        //ステート管理クラスの取得、ステート変更時に行う処理を登録
        _commonStateManager = CommonStateManager.Instance;
        _commonStateManager.AddOnChangeFunc(ToMainScene);

        //ローディング移行時の処理をStateManagerに代入===============


        //==========================================================

        //FirstLoad画面を作成するならそっちに移植する
        _eachDataLoad.InitLoad();
        //ローカルで保存されているゲームのロード＋UI反映
        _eachDataLoad?.LoadLocalData();
        //UI関係のロード
        _managedMainUI?.InitMainUI();
    }
    
    
    
    //ステート変更時に一緒に行う処理(後処理、シーン遷移アニメーションとか)
    private async UniTask ToMainScene(CancellationToken token)
    {

    }
}
