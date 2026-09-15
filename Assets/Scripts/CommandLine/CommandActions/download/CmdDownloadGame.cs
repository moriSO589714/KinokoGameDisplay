using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdDownloadGame : CmdActForUseNetwork
{
    private FilterCondition _currentFilterCondition = new FilterCondition();
    private CmdFiltering _filtering;

    public override void FirstCall()
    {
        base.FirstCall();
        _cmdSceneManager.OutPutManager.ReceiveMessage("ダウンロードモードに変更します", OutPutTextLogColorSets.SystemDefault);
        _filtering = new CmdFiltering(ReceiveCmdFiltering);

        try
        {
            PrepareUsingNetwork();
        }
        catch (Exception e)
        {
            if (_ctsForLoadSpreadSheet.IsCancellationRequested) return;

            _cmdSceneManager.OutPutManager.ReceiveMessage("ゲーム情報の取得に失敗しました。モードを終了します。", OutPutTextLogColorSets.AccentDefault);
            ReturnCmdReceiveMode();
            Debug.LogException(e);
            return;
        }
    }

    protected override async UniTask LoadSpreadSheetData()
    {
        await base.LoadSpreadSheetData();
        if (_ctsForLoadSpreadSheet.IsCancellationRequested) return;

        DoFiltering();
    }

    private void DoFiltering()
    {
        _cmdSceneManager.OutPutManager.ReceiveMessage("ダウンロードするゲームのフィルタリング情報を送信してください", OutPutTextLogColorSets.SystemDefault);
        _filtering.WaitSendCategory();
    }

    private void ReceiveCmdFiltering(FilterCondition sendedFilterCondition)
    {
        _currentFilterCondition = sendedFilterCondition;

        _cmdSceneManager.OutPutManager.ReceiveMessage("当てはまるゲームを検索中", OutPutTextLogColorSets.SystemDefault);
        //一致するGameDataクラスを取得してくる
        GameDatasSingleton gameDatasSingleton = GameDatasSingleton.Instance;
        List<GameData> matchGameDatas = GameBoxFilter.FilteringGameDatas(_currentFilterCondition, gameDatasSingleton.AllGameDatas);
        
        if(matchGameDatas != null && matchGameDatas.Count > 0)
        {
            string checkMessage = $"当てはまるゲームが{matchGameDatas.Count}個存在します。";
            foreach(GameData gameData in matchGameDatas)
            {
                checkMessage += $"\n・{gameData.GameTitle}| (id){gameData.GameID}";
            }
            _cmdSceneManager.OutPutManager.ReceiveMessage(checkMessage, OutPutTextLogColorSets.SystemDefault);

            _cmdSceneManager.OutPutManager.ReceiveMessage($"ダウンロードを実行しますか？", OutPutTextLogColorSets.SystemDefault);
            _cmdSceneManager.InputFieldManager.ChangeAction(CheckDownload);
        }
        else
        {
            _cmdSceneManager.OutPutManager.ReceiveMessage("当てはまるゲームが存在しません。フィルタリング設定を修正してください", OutPutTextLogColorSets.AccentDefault);
            DoFiltering();
        }
    }

    private void CheckDownload(bool isDownload)
    {
        _cmdSceneManager.OutPutManager.ReceiveMessage("ゲームのダウンロードを開始します。", OutPutTextLogColorSets.SystemDefault);
        if (!isDownload)
        {
            DoFiltering();
            return;
        }


    }

    /// <summary>
    /// 実際にダウンロードを実行
    /// </summary>
    private void DoDownload()
    {

    }
}
