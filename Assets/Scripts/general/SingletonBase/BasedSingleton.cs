using System;

/// <summary>
/// Lazy<T>を用いたシングルトンクラスの基底クラス
/// マルチスレッドに対応
/// </summary>
public class BasedSingleton<T> where T : class //where T : class　は型参照制約。Tにclass以外が入らないことを宣言
{
    //コンストラクタ(外部からのインスタンス生成を防ぐためprotectedで宣言するための記述)
    protected BasedSingleton() { }

    //スレッドセーフな遅延初期化インスタンスの保持処理
    //Lazy<T>を利用することにより、最初に値にアクセスされるまでインスタンスの生成を遅らせることができる。
    //Activatorの部分は、通常のnew T()だとコンストラクタがprivateになるため必要
    private static Lazy<T> _instance
        = new Lazy<T>(() => Activator.CreateInstance(typeof(T), true) as T);
    
    //外部からインスタンスを取得するためのプロパティ
    public static T Instance {  get { return _instance.Value; } }
}
