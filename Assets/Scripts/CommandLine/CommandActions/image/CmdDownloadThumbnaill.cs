using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Ookii.Dialogs;
using Unity.VisualScripting;
using UnityEngine;

public class CmdDownloadThumbnaill : CmdActForUseNetwork
{
    private FilterCondition _currentFilterCondition = new FilterCondition();
    private CmdFiltering _filtering;
    private List<GameData> _candidateGameDatas;
    private OnNetDriveGetFile _onNetDriveGetFile;

    private CancellationTokenSource _ctsForDownloadImages;

    protected override void Init()
    {
        base.Init();
        if (CheckInEnvironment.isOnNet)
        {
            NetworksSingleton singleton = NetworksSingleton.Instance;
            _onNetDriveGetFile = new OnNetDriveGetFilefromDv(singleton.ReturnDriveService());
        }
        else
        {
            _onNetDriveGetFile = new OnNetDriveGetFilefromTest();
        }
    }

    public override void FirstCall()
    {
        base.FirstCall();
        _cmdSceneManager.OutPutManager.SendMessage("サムネイルダウンロードモードに変更します", OutPutTextLogColorSets.SystemDefault);
        _ctsForDownloadImages = new CancellationTokenSource();

        try
        {
            PrepareUsingNetwork();
        }
        catch(System.Exception e)
        {
            if (_ctsForLoadSpreadSheet.IsCancellationRequested) return;

            _cmdSceneManager.OutPutManager.SendMessage("ゲーム情報の取得に失敗しました。モードを終了します", OutPutTextLogColorSets.AccentDefault);
            ReturnCmdReceiveMode();
            Debug.LogError(e);
            return;
        }
    }

    protected override async UniTask LoadSpreadSheetData()
    {
        await base.LoadSpreadSheetData();
        if (_ctsForLoadSpreadSheet.IsCancellationRequested) return;
        _filtering = new CmdFiltering(ReceiveCmdFiltering);

        DoFiltering();
    }

    private void DoFiltering()
    {
        _cmdSceneManager.OutPutManager.SendMessage("サムネイルのダウンロードを行うゲームのフィルタリング情報を送信してください", OutPutTextLogColorSets.SystemDefault);
        //ローカル追加のゲームはサムネイルのダウンロードが行えないため、フィルタリング設定を修正
        _filtering.FixStatusList(new List<GameStatus>() { GameStatus.NotDownloaded, GameStatus.Downloaded, GameStatus.Downloading, GameStatus.UpdateAvailable });
        _filtering.WaitSendCategory();
    }

    private void ReceiveCmdFiltering(FilterCondition sendedFilterCondition)
    {
        _currentFilterCondition = sendedFilterCondition;
        _cmdSceneManager.OutPutManager.SendMessage("当てはまるゲームを検索中", OutPutTextLogColorSets.SystemDefault);
        //一致するGameDataクラスの取得
        List<GameData> matchGameDatas = PickUpMatchGameData(_currentFilterCondition);

        if(matchGameDatas != null && matchGameDatas.Count > 0)
        {
            string checkMessage = $"当てはまるゲームが{matchGameDatas.Count}個存在します";
            foreach(GameData gameData in matchGameDatas)
            {
                checkMessage += $"\nタイトル名：{gameData.GameTitle}, id：{gameData.GameID}";
            }
            _cmdSceneManager.OutPutManager.SendMessage(checkMessage, OutPutTextLogColorSets.SystemDefault);

            _cmdSceneManager.OutPutManager.SendMessage("サムネイルのダウンロードを実行しますか？", OutPutTextLogColorSets.SystemDefault);
            _cmdSceneManager.InputFieldManager.ChangeAction(CheckThumbnailDownload);
        }
        else
        {
            _cmdSceneManager.OutPutManager.SendMessage("当てはまるゲームが存在しません。フィルタリング情報を修正してください", OutPutTextLogColorSets.SystemDefault);
            DoFiltering();
        }
    }

    private List<GameData> PickUpMatchGameData(FilterCondition filterCondition)
    {
        GameDatasSingleton gameDatasSingleton = GameDatasSingleton.Instance;
        List<GameData> matchGameDatas = GameBoxFilter.FilteringGameDatas(_currentFilterCondition, gameDatasSingleton.AllGameDatas);
        _candidateGameDatas = matchGameDatas;

        return matchGameDatas;
    }

    private void CheckThumbnailDownload(bool isDownload)
    {
        if (!isDownload)
        {
            DoFiltering();
            return;
        }
        _cmdSceneManager.OutPutManager.SendMessage("サムネイルのダウンロードを開始します", OutPutTextLogColorSets.SystemDefault);
        //コマンドを受け付けないようにする
        _cmdSceneManager.InputFieldManager.ChangeAction(new CmdNothing().MessageGird);
        DoThumbnailDownload();
    }

    private async UniTask DoThumbnailDownload()
    {
        int counter = 0;
        string baseLog = "サムネイルのダウンロード中\n【進捗】";
        int totalDataCounts = _candidateGameDatas.Count;
        string logId = _cmdSceneManager.OutPutManager.SendMessage(baseLog + $"{counter}/{totalDataCounts}", OutPutTextLogColorSets.SystemDefault);
        List<string> isErrorList = new List<string>();

        foreach(GameData targetGameData in _candidateGameDatas)
        {
            if (_ctsForDownloadImages.IsCancellationRequested)
            {
                return;
            }

            ImageDlProc imageDlProc = new ImageDlProc(_onNetDriveGetFile, targetGameData);
            try
            {
                await imageDlProc.DLImageInUniTask(_ctsForDownloadImages.Token);
            }
            catch(System.Exception e)
            {
                isErrorList.Add($"タイトル名：{targetGameData.GameTitle},  詳細：{e.Message}");
                if (_ctsForDownloadImages.IsCancellationRequested)
                {
                    return;
                }
            }

            counter++;
            _cmdSceneManager.OutPutManager.SendMessage(baseLog + $"{counter}/{totalDataCounts}", OutPutTextLogColorSets.SystemDefault, specifiedUUID: logId);
        }

        if (!_ctsForDownloadImages.IsCancellationRequested)
        {
            _cmdSceneManager.OutPutManager.SendMessage("指定された全てのゲームのサムネイルをダウンロードしました", OutPutTextLogColorSets.SystemDefault);

            if(isErrorList.Count > 0)
            {
                string errorLogTxt = "エラーが発生した画像が存在します。\n【一覧】";
                foreach(string errorLog in isErrorList)
                {
                    errorLogTxt = errorLogTxt + "\n" + "・" + errorLog;
                }
                _cmdSceneManager.OutPutManager.SendMessage(errorLogTxt, OutPutTextLogColorSets.AccentDefault);
            }

            _cmdSceneManager.OutPutManager.SendMessage("サムネイルダウンロードモードを終了します", OutPutTextLogColorSets.SystemDefault);
            ReturnCmdReceiveMode();
        }
    }

    protected override void ReturnCmdReceiveMode()
    {
        _ctsForDownloadImages?.Cancel();
        base.ReturnCmdReceiveMode();
    }
}
