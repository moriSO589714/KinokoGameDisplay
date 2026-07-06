using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullDown : ObjectStuckPool<PullDownCell>
{
    [SerializeField] GameObject _pullDownCellPref;

    private Vector2 _firstCreatePos = new Vector2();
    public Action<string> OnClikedCellAct;

    private void Awake()
    {
        //インスタンス時に親をPullDownPool(このクラスがアタッチされたオブジェクト)にするので、
        //このオブジェクトが一番上のセルの表示座標に置かれている限り、初めの生成座標は(0,0)
        _firstCreatePos = Vector2.zero;
    }

    public void ClosePullDownList()
    {
        ClearActiveStucks();
    }

    public void CreatePullDownList(List<string> pullDownElements)
    {
        for(int i = 0; i <= pullDownElements.Count - 1; i++)
        {
            PullDownCell targetPullDownCell = null;
            if(_notUsedStuckPool.Count > 0)
            {
                targetPullDownCell = ReturnNotUsedObject();
                _activeStuckPool.Add(targetPullDownCell);
                targetPullDownCell.gameObject.SetActive(true);
            }
            else
            {
                targetPullDownCell = InstPullDownCell();
            }
            targetPullDownCell.SetText(pullDownElements[i]);

            //生成したラベルを正しい位置に移動
            Vector2 createPos = new Vector2();
            if(i == 0)
            {
                createPos = _firstCreatePos;
            }
            else
            {
                //現在生成されているオブジェクトの最も下の座標を計算
                GameObject previousObj = _activeStuckPool[i - 1].gameObject;
                RectTransform previousObjRect = previousObj.GetComponent<RectTransform>();
                Vector2 previousPos = previousObjRect.anchoredPosition;
                createPos = new Vector2(previousPos.x, previousPos.y - previousObjRect.sizeDelta.y * previousObjRect.transform.localScale.y);
            }
            MovePullDownCell(targetPullDownCell, createPos);
        }
    }

    public override void RemoveActiveObject(PullDownCell targetObj)
    {
        targetObj.gameObject.SetActive(false);
        targetObj.End();
        base.RemoveActiveObject(targetObj);
    }

    /// <summary>
    /// Instantiateでオブジェクトを生成する
    /// </summary>
    /// <returns></returns>
    private PullDownCell InstPullDownCell()
    {
        GameObject instanced = Instantiate(_pullDownCellPref, parent: this.gameObject.transform);
        PullDownCell pullDownCell = instanced.GetComponent<PullDownCell>();
        _activeStuckPool.Add(pullDownCell);
        pullDownCell.OnCliledCellAct += OnClikedCellAct;
        return pullDownCell;
    }

    private void MovePullDownCell(PullDownCell target, Vector2 targetPos)
    {
        target.GetComponent<RectTransform>().anchoredPosition = targetPos;
    }
}
