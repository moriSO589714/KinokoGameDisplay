using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// オブジェクトのプール機能を実装するクラス
/// </summary>
/// <typeparam name="T">スタックするオブジェクトのクラス</typeparam>
public class ObjectStuckPool<T> : MonoBehaviour where T : class
{
    protected List<T> _activeStuckPool = new List<T>();
    protected List<T> _notUsedStuckPool = new List<T>();

    /// <summary>
    /// 現在アクティブ状態のオブジェクトを全て削除する
    /// </summary>
    public virtual void ClearActiveStucks()
    {
        //生成されているオブジェクトを全て削除する
        List<T> copyList = new List<T>(_activeStuckPool);
        foreach(T targetObj in copyList)
        {
            RemoveActiveObject(targetObj);
        }
    }

    /// <summary>
    /// 任意のオブジェクトをアクティブプールから削除
    /// </summary>
    public virtual void RemoveActiveObject(T targetObj)
    {
        //リスト上のインデックスを取得
        int removeIndex = _activeStuckPool.IndexOf(targetObj);
        if (removeIndex == -1) return;
        //スタックプールから削除
        _activeStuckPool.RemoveAt(removeIndex);
        //再利用のスタックプールに移動する
        _notUsedStuckPool.Add(targetObj);
    }

    /// <summary>
    /// 使用されていないオブジェクトを返す
    /// (notUsedからの削除も行う)
    /// </summary>
    protected virtual T ReturnNotUsedObject()
    {
        if(_notUsedStuckPool.Count != 0)
        {
            T notUsed = _notUsedStuckPool[_notUsedStuckPool.Count - 1];
            _notUsedStuckPool.RemoveAt(_notUsedStuckPool.Count - 1);
            return notUsed;
        }

        return null;
    }
}
