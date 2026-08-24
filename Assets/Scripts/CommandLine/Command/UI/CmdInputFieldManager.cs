using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// コマンドラインのインプットマネージャーから入力が来た際に入力を送る
/// </summary>
public class CmdInputFieldManager : MonoBehaviour
{
    [SerializeField] private InputField _myInputField;
    private Action<string> _throwMessageMethod;
    private FreeInputEnterController _freeInputEnterController;
    private CmdSceneManager _cmdSceneManager = null;

    [SerializeField] private string _resetWord;
    //コマンド受信に切り替えるメソッド
    public Action _setCommandReceiver;
    //現在登録されているアクションが切り替えられる際に実行されるアクション
    public Action _whenEndCurrentAction;
    //コマンド受信モードに切り替える際に実行するアクション。モードの終了処理など
    public Action _endModeAction;

    //予測変換に利用する各インスタンス
    [SerializeField] private CandidateBoxManager _candidateBoxManager;
    [SerializeField] private MonitorPlayerInput _monitorPlayerInput;
    [SerializeField] private Vector2 _candidateStartPos;

    //予測変換に利用するフィールド
    private string _recordLastInput;
    private bool _isWordEstimateActive = false;
    private WordEstimater _currentWordEstimater;
    private Action _nextSelectBox;
    private Action _previousSelectBox;

    private void Awake()
    {
        _nextSelectBox = () => _candidateBoxManager.MovePerSelectBox(true);
        _previousSelectBox = () => _candidateBoxManager.MovePerSelectBox(false);

        _freeInputEnterController = new FreeInputEnterController(TryAction, ReflectCandidiateValue, _candidateBoxManager);     
    }

    public void ChangeAction(Action<string> tryAct, WordEmtCell newLibrary = null)
    {
        ClearCandidateBox();
        _whenEndCurrentAction?.Invoke();
        _whenEndCurrentAction = null;
        _throwMessageMethod = tryAct;
        
        if(newLibrary == null)
        {
            _isWordEstimateActive = false;
            return;
        }

        _isWordEstimateActive = true;
        _currentWordEstimater = new WordEstimater(newLibrary, " ");
    }

    public void ChangeInputfieldVal(string val)
    {
        _myInputField.text = val;
    }

    public void OnValueChange()
    {
        if (_isWordEstimateActive == false) return;

        string inputTxt = _myInputField.text;
        if(_recordLastInput != inputTxt)
        {
            _recordLastInput = inputTxt;
            //ここのコマンド探索を行う深さの引数が定数になっちゃっているの改善
            List<string> estimateSentenced = _currentWordEstimater.ReturnEstimatedStrs(inputTxt, 10);

            //予測コマンドがヒットしない場合
            if(estimateSentenced == null || estimateSentenced.Count == 0)
            {
                ClearCandidateBox();
                return;
            }
            CreateCandidateBox(estimateSentenced);
        }
        else
        {
            ClearCandidateBox();
        }
    }

    public void OnEndEdit()
    {
        ClearCandidateBox();
    }

    public void OnValueSubmit()
    {
        _freeInputEnterController.WhenSubmitInputField();
    }

    public void ReturnCommandReceive()
    {
        _endModeAction?.Invoke();
        _setCommandReceiver?.Invoke();
    }

    private void TryAction()
    {
        if (_cmdSceneManager == null) _cmdSceneManager = CmdSceneManager.Instance;
        string inputFieldTxt = _myInputField.text;
        ClearInputField();
        _cmdSceneManager.OutPutManager.ReceiveMessage(inputFieldTxt, OutPutTextLogColorSets.UserDefault, true);

        //強制終了時用(強制的にデフォルトに戻る)
        if(inputFieldTxt == _resetWord)
        {
            ReturnCommandReceive();
        }
        else//現在の受信メソッドへ入力内容を送る
        {
            _throwMessageMethod?.Invoke(inputFieldTxt);
        }
    }

    private void ClearInputField()
    {
        ClearCandidateBox();
        _myInputField.text = "";
        _recordLastInput = "";
    }

    private void CreateCandidateBox(List<string> estimateWords)
    {
        _candidateBoxManager.InstCandidateBoxs(estimateWords, _candidateStartPos);

        _monitorPlayerInput.TabButtonAct += _nextSelectBox;
        _monitorPlayerInput.UpArrowAct += _nextSelectBox;
        _monitorPlayerInput.DownArrowAct += _previousSelectBox;
    }

    private void ClearCandidateBox()
    {
        _candidateBoxManager.ClearBoxs();

        _monitorPlayerInput.TabButtonAct -= _nextSelectBox;
        _monitorPlayerInput.UpArrowAct -= _nextSelectBox;
        _monitorPlayerInput.DownArrowAct -= _previousSelectBox;
    }

    private void ReflectCandidiateValue()
    {
        string selectedTxt = _candidateBoxManager.ReturnSelectedTxt();
        _myInputField.text = selectedTxt;
    }
}