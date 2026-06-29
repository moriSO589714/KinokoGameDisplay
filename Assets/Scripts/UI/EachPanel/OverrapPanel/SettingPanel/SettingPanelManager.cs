using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanelManager : OverrapUIPanel
{
    [SerializeField] private UIActBase _closeButton;
    [SerializeField] private InputField _activationCodeInputField;
    [SerializeField] private UIActBase _activationButton;
    [SerializeField] private UIActBase _onDownloadPanelButton;
    [SerializeField] private UIPanel _downloadPanel;
    [SerializeField] private GameObject _loadingPanelPref;

    public override void InitPanel()
    {
        InitPanelUIObj();
        //デリゲート設定
        _closeButton.ClickAct = OnCloseProc;
        _activationButton.ClickAct = ActivationKeyCode;
        _onDownloadPanelButton.ClickAct = () => { _downloadPanel.gameObject.SetActive(true); };
    }

    private void InitPanelUIObj()
    {
        _activationCodeInputField.text = "";
    }

    private void ActivationKeyCode()
    {
        string code = _activationCodeInputField.text;
        InitPanelUIObj();

        GameObject loadingPanelObj = Instantiate(_loadingPanelPref, parent: this.gameObject.transform);
        LoadingPanel loadingPanel = loadingPanelObj.GetComponent<LoadingPanel>();
        loadingPanel.SetLogTxt("コードの確認中");

        try
        {
            new PathKeyManager().ActivateKeyCode(code);
            loadingPanel.SetLogTxt("アクティベート完了");
        }
        catch(System.Exception e)
        {
            Debug.LogError(e);
            loadingPanel.SetLogTxt("コードを認識できませんでした");
        }

        loadingPanel.AvailableClose();
    }
}
