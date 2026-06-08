using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandidateBoxManager : MonoBehaviour
{
    //一度に表示する単語数
    [SerializeField] int _oneTimeWords;
    [SerializeField] GameObject _candidateBoxPref;
    [SerializeField] Canvas _canvas;

    private Vector2 _startAnchoredPos = Vector2.zero;
    private List<CandidateBox> _createdBoxPool = new List<CandidateBox>();

    //oneTimeWordsずつ分割したワードリスト(1ページぶんづつ)
    private List<List<string>> _pages = new List<List<string>>();
    private int _currentPageIndex = -1;
    private int _currentSelectBoxIndex = -1;

    //マネージャーの初期化処理
    private void Init()
    {
        foreach (CandidateBox cb in _createdBoxPool)
        {
            cb.LeaveThis();
            cb.gameObject.SetActive(false);
        }
        _currentPageIndex = -1;
        _currentSelectBoxIndex = -1;
        _pages = new List<List<string>>();
    }

    public void ClearBoxs()
    {
        Init();
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
    /// <returns></returns>
    public string ReturnSelectedTxt()
    {
        if(_currentSelectBoxIndex == -1)
        {
            return null;
        }

        return _createdBoxPool[_currentSelectBoxIndex]._pureTxt;
    }

    /// <summary>
    /// firstPosを基点にボックス群を生成する
    /// </summary>
    private void CreateCandidateBox(List<string> page)
    {
        int currentCreateCounts = 0;
        //与えられたリストの最初から表示するぶんboxを生成する
        for(int i = 0; i < page.Count; i++)
        {
            if(_createdBoxPool.Count - 1 >= i)
            {
                _createdBoxPool[i].gameObject.SetActive(true);
            }
            else //プールのボックスが不足している場合はインスタンスする
            {
                //生成する座標を求める
                Vector2 previousFloorPos;
                if(i == 0)
                {
                    previousFloorPos = _startAnchoredPos;
                }
                else
                {
                    GameObject previousObject = _createdBoxPool[i - 1].gameObject;
                    RectTransform previousRectTransform = previousObject.GetComponent<RectTransform>();
                    Vector2 previousPos = previousRectTransform.anchoredPosition;
                    previousFloorPos = new Vector2(previousPos.x ,previousPos.y - previousRectTransform.sizeDelta.y * previousRectTransform.transform.localScale.y);
                }
                InstCandidateBox(previousFloorPos);
            }
            _createdBoxPool[i].SetLabel(page[i]);
            currentCreateCounts++;
        }

        //使用しなかったpool内のboxを非アクティブにする
        for(int i = _createdBoxPool.Count; i > currentCreateCounts; i--)
        {
            _createdBoxPool[i - 1].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 実際にオブジェクトを生成
    /// </summary>
    /// <param name="previousFloorPos">開始点のAnchoredPosition</param>
    private void InstCandidateBox(Vector2 previousFloorPos)
    {
        GameObject inst =  Instantiate(_candidateBoxPref, parent: this.gameObject.transform);
        inst.GetComponent<RectTransform>().anchoredPosition = previousFloorPos;
        _createdBoxPool.Add(inst.GetComponent<CandidateBox>());
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
        while (i < estimateWords.Count);

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
            _createdBoxPool[_currentSelectBoxIndex].LeaveThis();
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

        _createdBoxPool[nextSelect].SelectThis();
        _currentSelectBoxIndex = nextSelect;
    }
}
