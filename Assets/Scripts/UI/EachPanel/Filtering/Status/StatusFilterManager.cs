using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public void PanelCloseProc()
    {
        ResetFirstState();
    }

    private void Awake()
    {
        //デリゲート系のアクションをセット
        _notDownloadCheckBox.ClickAct = () => { 
            SwitchFilterFlag(GameStatus.NotDownloaded);
            SwitchFilterFlag(GameStatus.Downloading);
        };
        _byLocalCheckBox.ClickAct = () => { SwitchFilterFlag(GameStatus.ByLocal); };
        _downloadedCheckBox.ClickAct = () => { SwitchFilterFlag(GameStatus.Downloaded); };
        _updateAvailableCheckBox.ClickAct = () => { SwitchFilterFlag(GameStatus.UpdateAvailable); };
    }

    private void OnEnable()
    {
        Init();
    }

    /// <summary>
    /// パネルを表示する際に行う初期化処理
    /// FilterConditionsから現在設定されているフィルタリングを適用させる
    /// </summary>
    private void Init()
    {
        GameDatasSingleton gameDatasSingleton = GameDatasSingleton.Instance;
        FilterCondition filterCondition = gameDatasSingleton.CurrentFilterCondition;
        if (filterCondition != null)
        {
            List<GameStatus> statuses = filterCondition.Statuses;
            //フィルターで除去される項目のみを抽出
            List<GameStatus> offStatuses = StatusFiltering
                .Where(x => !statuses.Contains(x.Key)).Select(x => x.Key).ToList();

            foreach(GameStatus gs in offStatuses)
            {
                //フィールド値の切り替え
                SwitchFilterFlag(gs);

                //表示の切り替え
                CheckBox targetCheckBox = null;
                switch (gs)
                {
                    case GameStatus.ByLocal:
                        targetCheckBox = _byLocalCheckBox;
                        break;
                    case GameStatus.NotDownloaded:
                        targetCheckBox = _notDownloadCheckBox;
                        break;
                    case GameStatus.Downloaded:
                        targetCheckBox = _downloadedCheckBox;
                        break;
                    case GameStatus.UpdateAvailable:
                        targetCheckBox = _updateAvailableCheckBox;
                        break;
                }
                if(targetCheckBox != null)
                {
                    targetCheckBox.CheckMarkSwitchActive(false);
                }
            }
        }
    }

    /// <summary>
    /// パネル表示を初期状態に戻す処理(終了時などに実行する)
    /// </summary>
    private void ResetFirstState()
    {
        foreach(GameStatus key in StatusFiltering.Keys.ToList())
        {
            StatusFiltering[key] = true;
        }
        _byLocalCheckBox.CheckMarkSwitchActive(true);
        _notDownloadCheckBox.CheckMarkSwitchActive(true);
        _downloadedCheckBox.CheckMarkSwitchActive(true);
        _updateAvailableCheckBox.CheckMarkSwitchActive(true);
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
