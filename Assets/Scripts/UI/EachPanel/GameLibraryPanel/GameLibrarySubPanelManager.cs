using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameLibrarySubPanelManager : UIPanel
{
    [SerializeField] private Text _currentTitleText;
    [SerializeField] private UIActBase _closeMrk;
    [SerializeField] private UIActBase _openGameFolder;
    [SerializeField] private UIActBase _openGameExeFolder;
    [SerializeField] private UIActBase _uninstallGameButton;

    [SerializeField] private UninstallPanelManager _uninstallPanelManager;

    private GameData _myGameData;

    public void SetData(GameData setGameData)
    {
        _myGameData = setGameData;
        _currentTitleText.text = _myGameData.GameTitle;
        _closeMrk.ClickAct = OnCloseProc;
        AllDirs allDirs = AllDirs.GetInstance();
        _openGameFolder.ClickAct = () => FileOpenByExplorer.OpenFolder(CreateDirPath.GameDataPath(allDirs.GameFilePath, _myGameData.GameID, _myGameData.GameDirName));
        _openGameExeFolder.ClickAct = () => FileOpenByExplorer.OpenFile(Path.Combine(allDirs.GameFilePath, _myGameData.GameID, _myGameData.GameDirName, _myGameData.GameExeName));

        _uninstallGameButton.ClickAct = () => 
        {
            OnCloseProc();
            _uninstallPanelManager.gameObject.SetActive(true);
            _uninstallPanelManager.SetData(_myGameData);
        };
    }
}