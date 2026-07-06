using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandidateBoxManager : ObjectStuckPool<CandidateBox>
{
    //一度に表示する単語数
    [SerializeField] int _oneTimeWords;
    [SerializeField] GameObject _candidateBoxPref;
    [SerializeField] Canvas _canvas;

    private Vector2 _startAnchoredPos = Vector2.zero;

    //oneTimeWordsずつ分割したワードリスト(1ページぶんづつ)
    private List<List<string>> _pages = new List<List<string>>();
    private int _currentPageIndex = -1;

    public int _currentSelectBoxIndex { get; private set; } = -1;

    //マネージャーの初期化処理
    private void Init()
    {
        ClearActiveStucks();
        _currentPageIndex = -1;
        _currentSelectBoxIndex = -1;
        _pages = new List<List<string>>();
    }

    public void ClearBoxs()
    {
        Init();
    }

    public override void RemoveActiveObject(CandidateBox targetObj)
    {
        targetObj.LeaveThis();
        targetObj.gameObject.SetActive(false);
        base.RemoveActiveObject(targetObj);
    }

    /// <summary>
    /// box群を生成する時に実行する処理
    /// </summary>
    public void InstCandidateBoxs(List<string> estimateWords, Vector2 createPos)
    {
        //マネージャーの初期化
        Init();
        
        //各種値の設定
        _startAnchoredPos = createPos;
        _pages = DivisionWordsList(estimateWords);

        //ボックス群の生成
        CreateCandidateBox(_pages[0]);
        _currentPageIndex = 0;
    }

    /// <summary>
    /// 選択状態の語をずらしていく
    /// </summary>
    /// <param name="pn">true = 下の語へ false = 上の語へ</param>
    public void MovePerSelectBox(bool pn)
    {
        //表示している語が無い場合は実行しない
        if (_pages == null || _pages.Count == 0) return;

        int move = 0;
        if (pn)
        {
            move = 1;
        }
        else
        {
            move = -1;
        }

        MoveSelectBox(move);
    }

    /// <summary>
    /// 表示している要素を１つずらす
    /// </summary>
    /// <param name="pn">true = 下の語へ false = 上の語へ</param>
    public void MovePerPage(bool pn)
    {
        //表示している語が無い場合は実行しない
        if (_pages == null || _pages.Count == 0) return;

        int move = 0;
        if (pn)
        {
            move = 1;
        }
        else
        {
            move = -1;
        }

        MovePage(move);
    }

    /// <summary>
    /// 現在選択中のboxに割り当てられているテキストを返す
    /// </summary>
    public string ReturnSelectedTxt()
    {
        if(_currentSelectBoxIndex == -1)
        {
            return null;
        }

        return _activeStuckPool[_currentSelectBoxIndex]._pureTxt;
    }

    /// <summary>
    /// firstPosを基点にボックス群を生成する
    /// </summary>
    private void CreateCandidateBox(List<string> page)
    {
        //現在生成されているボックスがあれば全て消す
        ClearActiveStucks();

        //与えられたリストの最初から表示するぶんboxを生成する
        for(int i = 0; i < page.Count; i++)
        {
            //CandidateBoxの生成
            CandidateBox targetBox = null;
            if (_notUsedStuckPool.Count > 0) //使用されていないboxが存在する
            {
                targetBox = ReturnNotUsedObject();
                _activeStuckPool.Add(targetBox);
                targetBox.gameObject.SetActive(true);
            }
            else
            {
                //プールのボックスが不足している場合はインスタンスする
                targetBox = InstCandidateBox();
            }
            targetBox.SetLabel(page[i]);

            //生成したラベルを正しい位置に移動させる
            Vector2 createPos = new Vector2();
            if(i == 0)
            {
                createPos = _startAnchoredPos;
            }
            else
            {
                //現在生成されている最新のcandidateBox(一番下)
                GameObject previousObject = _activeStuckPool[i - 1].gameObject;
                RectTransform previousRectTransform = previousObject.GetComponent<RectTransform>();
                Vector2 previousPos = previousRectTransform.anchoredPosition;
                createPos = new Vector2(previousPos.x, previousPos.y - previousRectTransform.sizeDelta.y * previousRectTransform.transform.localScale.y);
            }
            MoveCandidateBoxPos(targetBox, createPos);
        }
    }

    /// <summary>
    /// 実際にオブジェクトを生成してアクティブリストに追加する
    /// </summary>
    /// <param name="previousFloorPos">開始点のAnchoredPosition</param>
    private CandidateBox InstCandidateBox()
    {
        GameObject inst =  Instantiate(_candidateBoxPref, parent: this.gameObject.transform);
        CandidateBox candidateBox = inst.GetComponent<CandidateBox>();
        _activeStuckPool.Add(candidateBox);
        return candidateBox;
    }

    private void MoveCandidateBoxPos(CandidateBox candidateBox, Vector2 targetPos)
    {
        candidateBox.GetComponent<RectTransform>().anchoredPosition = targetPos;
    }

    private List<List<string>> DivisionWordsList(List<string> estimateWords)
    {
        List<List<string>> ReturnList = new List<List<string>>();
        int i = 0;
        do
        {
            int restCount = estimateWords.Count - i;
            if(_oneTimeWords <= restCount)
            {
                restCount = _oneTimeWords;
            }

            List<string> div = estimateWords.GetRange(i, restCount);
            ReturnList.Add(div);

            i += _oneTimeWords;
        }
        while (i < estimateWords.Count - 1);

        return ReturnList;
    }

    private void MovePage(int move)
    {
        int nextPage = _currentPageIndex + move;
        if(_pages.Count <= nextPage)
        {
            nextPage -= _pages.Count;
        }
        else if (nextPage < 0)
        {
            nextPage = _pages.Count - 1;
        }

        CreateCandidateBox(_pages[nextPage]);
        _currentPageIndex = nextPage;
    }

    private void MoveSelectBox(int move)
    {
        if(_currentSelectBoxIndex != -1)
        {
            _activeStuckPool[_currentSelectBoxIndex].LeaveThis();
        }
        RecursiveMoveSelectBox(move);
    }

    private void RecursiveMoveSelectBox(int move)
    {
        int nextSelect = _currentSelectBoxIndex + move;
        //次のページに行く必要がある場合
        if(nextSelect >= _pages[_currentPageIndex].Count)
        {
            int end = _pages[_currentPageIndex].Count - _currentSelectBoxIndex;
            MovePerPage(true);
            _currentSelectBoxIndex = 0;
            RecursiveMoveSelectBox(move - end);
            return;
        }
        else if(nextSelect < 0)//前のページに行く必要がある場合
        {
            int end = _currentSelectBoxIndex + 1;
            MovePerPage(false);
            _currentSelectBoxIndex = _pages[_currentPageIndex].Count - 1;
            RecursiveMoveSelectBox(move + end);
            return;
        }

        _activeStuckPool[nextSelect].SelectThis();
        _currentSelectBoxIndex = nextSelect;
    }
}
