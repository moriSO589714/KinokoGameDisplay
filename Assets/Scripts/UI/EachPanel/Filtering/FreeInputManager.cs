using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FreeInputManager : MonoBehaviour
{
    [SerializeField] protected InputField _myInputField;
    [SerializeField] private CandidateBoxManager _candidateBoxManager;
    [SerializeField] private MonitorPlayerInput _monitorPlayerInput;

    private WordEstimater _wordEstimater;

    private RectTransform _myFieldRectTransform;
    private string _recordLastInput = "";

    private Action _nextSelectBox;
    private Action _previousSelectBox;

    /// <summary>
    /// オブジェクトがアクティブになった際に実行
    /// </summary>
    private void OnEnable()
    {
        GameDatasSingleton gameDatasSingleton = GameDatasSingleton.Instance;
        _wordEstimater = new WordEstimater(gameDatasSingleton.ReturnTagDictionary(), " ");
        _recordLastInput = "";
        _myFieldRectTransform = _myInputField.GetComponent<RectTransform>();

        _nextSelectBox = () => _candidateBoxManager.MovePerSelectBox(true);
        _previousSelectBox = () => _candidateBoxManager.MovePerSelectBox(false);
    }

    /// <summary>
    /// フィールドの値が変更された時に発火させる
    /// </summary>
    public void OnValueChange()
    {
        string inputTxt = _myInputField.text;
        //単語予測の表示
        CreateEstimate(inputTxt);
    }

    /// <summary>
    /// フィールドから入力が離れた際に発火させる
    /// </summary>
    public void OnEndEdit()
    {
        ClearBox();
    }

    private void CreateEstimate(string inputTxt)
    {
        //前回の入力から入力が変更されている場合
        if(_recordLastInput != inputTxt)
        {
            //予測単語群を取得
            List<string> estimateWords = _wordEstimater.ReturnEstimatedStrs(inputTxt, 0);

            //予測単語が無い場合はパネルを初期化して終了
            if(estimateWords == null || estimateWords.Count() == 0)
            {
                ClearBox();
                return;
            }

            CreateEstimateBox(estimateWords);
        }
        else
        {
            //パネルを初期化して非表示にする
            ClearBox();
        }
    }

    private void CreateEstimateBox(List<string> estimateWords)
    {
        //パネルの生成位置を取得
        Vector2 createPos = ReturnEstimatePanelPos();
        //生成
        _candidateBoxManager.InstCandidateBoxs(estimateWords, createPos);

        //入力の割り当て
        _monitorPlayerInput.TabButtonAct += _nextSelectBox;
        _monitorPlayerInput.DownArrowAct += _nextSelectBox;
        _monitorPlayerInput.UpArrowAct += _previousSelectBox;
    }

    private void ClearBox()
    {
        _candidateBoxManager.ClearBoxs();
        _monitorPlayerInput.TabButtonAct -= _nextSelectBox;
        _monitorPlayerInput.DownArrowAct -= _nextSelectBox;
        _monitorPlayerInput.UpArrowAct -= _previousSelectBox;
    }

    /// <summary>
    /// 現在の入力量から予測変換を出す場所を算出する
    /// </summary>
    private Vector2 ReturnEstimatePanelPos()
    {
        float panelYPos = - _myFieldRectTransform.sizeDelta.y;
        return new Vector2(0, panelYPos);
    }
}
