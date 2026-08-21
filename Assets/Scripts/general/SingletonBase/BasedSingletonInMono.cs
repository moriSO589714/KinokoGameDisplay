using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Monoを継承した
/// シングルトン用の基底クラス
/// ※正常に動作しない場合(pureC#のコンストラクタからの呼び出し。Awakeからの呼び出し)
/// </summary>
/// <typeparam name="T"></typeparam>
public class BasedSingletonInMono<T> : MonoBehaviour where T : MonoBehaviour
{
    //シングルトンのインスタンス
    private static T instance;

    public static T Instance
    {
        get
        {
            string typeName = typeof(T).Name;
            if (instance == null)
            {
                //シーン内からゲームオブジェクトを検索
                instance = FindObjectOfType<T>();

                if (instance == null)
                {
                    //インスタンスがシーン内に存在しない場合、新しく作成
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<T>();
                    singletonObject.name = typeof(T).ToString() + " (Singleton)";
                }
            }
            return instance;
        }
    }

    //重複していないかを確認
    protected virtual void Awake()
    {
        if (instance == null)
        {
            string typeName = typeof(T).Name;
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            string typeName = typeof(T).Name;
            Destroy(gameObject);
            return;
        }
    }
}