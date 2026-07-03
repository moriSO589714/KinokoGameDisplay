using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusFilterManager : MonoBehaviour
{
    [SerializeField] private  CheckBox _byLocalCheckBox;
    [SerializeField] private  CheckBox _notDownloadCheckBox;
    [SerializeField] private  CheckBox _downloadedCheckBox;
    [SerializeField] private  CheckBox _updateAvailableCheckBox;

    public Dictionary<GameStatus, bool> StatusFiltering { get; private set; } = new Dictionary<GameStatus, bool>()
    {
        { GameStatus.ByLocal, true},
        { GameStatus.NotDownloaded, true},
        { GameStatus.Downloading, true},
        { GameStatus.Downloaded, true},
        { GameStatus.UpdateAvailable, true },
    };

    private void Awake()
    {
        _notDownloadCheckBox.ClickAct = () => { 
            SwitchFilterFlag(GameStatus.NotDownloaded);
            SwitchFilterFlag(GameStatus.Downloading);
        };
        _byLocalCheckBox.ClickAct = () => { SwitchFilterFlag(GameStatus.ByLocal); };
        _downloadedCheckBox.ClickAct = () => { SwitchFilterFlag(GameStatus.Downloaded); };
        _updateAvailableCheckBox.ClickAct = () => { SwitchFilterFlag(GameStatus.UpdateAvailable); };
    }

    private void SwitchFilterFlag(GameStatus switchedStatus)
    {
        if (StatusFiltering[switchedStatus])
        {
            StatusFiltering[switchedStatus] = false;
        }
        else
        {
            StatusFiltering[switchedStatus] = true;
        }
    }
}
