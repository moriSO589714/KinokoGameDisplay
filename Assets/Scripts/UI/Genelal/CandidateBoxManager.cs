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

    private List<string> _currentEstimateWordsList = new List<string>();
    private int _currentFirstDisplayIndex = -1;
    private List<CandidateBox> _createdBoxPool = new List<CandidateBox>();
    
    /// <summary>
    /// 初めてbox群を生成する時に実行する処理
    /// </summary>
    public void InstCandidateBoxs(List<string> estimateWords, Vector2 createPos)
    {
        _startAnchoredPos = createPos;
        _currentEstimateWordsList = estimateWords;
        _currentFirstDisplayIndex = 0;

        CreateCandidateBox(estimateWords);
    }
    
    /// <summary>
    /// 表示している要素を１つずらす
    /// </summary>
    /// <param name="pn">true = indexの深いほうへずらす false = indexの浅いほうへ</param>
    public void SlidePerUnit(bool pn)
    {
        int slide = 0;
        if (pn)
        {
            slide = 1;
        }
        else
        {
            slide = -1;
        }

        SlideDisplayRange(slide);
    }

    /// <summary>
    /// firstPosを基点にボックス群を生成する
    /// </summary>
    private void CreateCandidateBox(List<string> estimateWords)
    {
        int currentCreateCounts = 0;
        //与えられたリストの最初から表示するぶんboxを生成する
        for(int i = 0; i < _oneTimeWords && i < estimateWords.Count; i++)
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
                    previousFloorPos = new Vector2(previousPos.x ,previousPos.y - previousRectTransform.sizeDelta.y);
                }
                InstCandidateBox(previousFloorPos);
            }
            _createdBoxPool[i].SetLabel(estimateWords[i]);
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

    /// <summary>
    /// 指定の個数表示している範囲をずらす
    /// </summary>
    private void SlideDisplayRange(int slideRange)
    {
        if (_currentFirstDisplayIndex == -1) return;

        //ずらした後に1番上に表示される要素のリスト内でのindex
        int firstDisplayIndex = _currentFirstDisplayIndex + slideRange;
        if(firstDisplayIndex <= -1)//最初のページまで既に達している場合
        {
            return;
        }
        else if(firstDisplayIndex >= _currentEstimateWordsList.Count)//始めに与えられた文字列リストの要素数以上のページを指定している場合
        {
            //1ページ目に戻す
            firstDisplayIndex = 0;
        }
        //1つ目の要素が1番上に表示する要素になるリストを作成
        List<string> displayList = _currentEstimateWordsList.GetRange(firstDisplayIndex, _currentEstimateWordsList.Count - firstDisplayIndex);
        //生成
        CreateCandidateBox(displayList);
        _currentFirstDisplayIndex = firstDisplayIndex;

    }
}
