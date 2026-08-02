using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FreeInputManager : MonoBehaviour
{
    [SerializeField] protected InputField _myInputField;
    [SerializeField] private CandidateBoxManager _candidateBoxManager;
    [SerializeField] private MonitorPlayerInput _monitorPlayerInput;

    protected PickUpCandidateElementProc _pickUpCandidateElementProc;

    private FreeInputEnterController _freeInputEnterController;

    private RectTransform _myFieldRectTransform;
    private string _recordLastInput = "";

    private Action<string> _sendInputValue;
    private Action _nextSelectBox;
    private Action _previousSelectBox;

    private void Awake()
    {
        _freeInputEnterController = new FreeInputEnterController(RegisterInput, RefrelctCandidateTxt, _candidateBoxManager);
        _myFieldRectTransform = _myInputField.GetComponent<RectTransform>();

        _nextSelectBox = () => _candidateBoxManager.MovePerSelectBox(true);
        _previousSelectBox = () => _candidateBoxManager.MovePerSelectBox(false);
    }

    /// <summary>
    /// オブジェクトがアクティブになった際に実行
    /// </summary>
    private void OnEnable()
    {
        //パネルが消えている間にゲーム情報が更新されている可能性があるため、これだけ再ロードする
        ActivatePickUpCandidateProc();
    }

    /// <summary>
    /// 終了時処理
    /// </summary>
    public void RefleshField()
    {
        _recordLastInput = "";
        ClearBox();
        _myInputField.text = "";
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

    public void OnValueSubmit()
    {
        _freeInputEnterController.WhenSubmitInputField();
    }

    /// <summary>
    /// フィールドから入力が離れた際に発火させる
    /// </summary>
    public void OnEndEdit()
    {
        ClearBox();
    }

    public void SetSendInputValueAct(Action<string> sendInputValueAct)
    {
        _sendInputValue = sendInputValueAct;
    }

    protected virtual void RegisterInput()
    {
        string inputTxt = _myInputField.text;
        _sendInputValue(inputTxt);
        _myInputField.text = "";
        ClearBox();
    }

    private void RefrelctCandidateTxt()
    {
        string candidateTxt = _candidateBoxManager.ReturnSelectedTxt();
        _myInputField.text = candidateTxt;
    }

    protected virtual void ActivatePickUpCandidateProc()
    {

    }

    private void CreateEstimate(string inputTxt)
    {
        //前回の入力から入力値が変更されている場合
        if(_recordLastInput != inputTxt)
        {
            _recordLastInput = inputTxt;
            //予測単語群を取得
            List<string> estimateWords = _pickUpCandidateElementProc.CreateCandidates(inputTxt);

            //予測単語が無い場合はパネルを初期化して終了
            if(estimateWords == null || estimateWords.Count == 0)
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
