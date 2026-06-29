using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorBox : WaitBox
{
    [SerializeField] Text _errorDescriptionTxt;
    [SerializeField] UIActBase _recoveryButton;
    [SerializeField] UIActBase _deleteErrorButton;

    public GameDlError _myGameDlError { get; private set; }

    public override void SetDataMyBox<T>(T originData)
    {
        GameDlError thisGameDlError = originData as GameDlError;
        _myGameDlError = thisGameDlError;

        SetGameName(_myGameDlError.Task.TaskInstance.GameData.GameTitle);
        ChoseErrorResponse(_myGameDlError.DlException.GameDlErrorType);
    }

    public void SetButtonActs(Action<string> recoveryAct, Action<string> errorDeleteAct)
    {
        string taskName = _myGameDlError.Task.TaskName;
        _recoveryButton.ClickAct = () =>
        {
            recoveryAct(taskName);
            _errorDescriptionTxt.text = "復旧処理実行中";
            _recoveryButton.gameObject.SetActive(false);
        };

        _deleteErrorButton.ClickAct = () =>
        {
            errorDeleteAct(taskName);
        };
    }

    private void ChoseErrorResponse(GameDlErrorType errorType)
    {
        string errorMessage = "";
        switch (errorType) 
        {
            case GameDlErrorType.Unknown:
                _recoveryButton.gameObject.SetActive(false);
                break;
            case GameDlErrorType.NeedCleanDirectory:
                _recoveryButton.gameObject.SetActive(true);
                errorMessage = "ディレクトリ系のエラーが発生しました。再実行してください";
                break;
            case GameDlErrorType.NeedRetryAccessDrive:
                _recoveryButton.gameObject.SetActive(true);
                errorMessage = "ネットでのエラーが発生しました。時間を置いてみて再実行してみてください";
                break;
            case GameDlErrorType.ImpossibleRecoveryErrorOnDrive:
                _recoveryButton.gameObject.SetActive(false);
                errorMessage = "回復不可能なネットでのエラーが発生しました。解決するには管理者に問い合わせてください";
                break;
            case GameDlErrorType.ImpossibleRecoveryErrorOnRuntime:
                _recoveryButton.gameObject.SetActive(false);
                errorMessage = "回復不可能なラインタイム上でのエラーが発生しました。解決するには管理者に問い合わせてください";
                break;
            case GameDlErrorType.Others:
                _recoveryButton.gameObject.SetActive(false);
                break;
            default:
                _recoveryButton.gameObject.SetActive(false);
                errorMessage = "想定されていないエラーが発生しました。解決するには管理者に問い合わせください";
                break;
        }

        SetErrrorDescription(errorMessage);
    }

    private void SetErrrorDescription(string errorDescription)
    {
        _errorDescriptionTxt.text = errorDescription;
    }
}
