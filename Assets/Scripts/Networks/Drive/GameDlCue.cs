using Cysharp.Threading.Tasks;
using Google.Apis.Drive.v3;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UnityEngine;

public class GameDlCue : MonoBehaviour
{
    public List<GameDlTask> GameDlTasksList { get; private set; } = new List<GameDlTask>();
    //進捗監視用のオブジェクト
    public GameDlProgress CurrentGameDlProgress { get; private set; } = null;

    //タスク実行中のフラグ
    bool doTaskFlag = false;
    //新しくタスクを実行させない(リスト操作中は新しくタスクを実行させない)
    bool lockCueFlag = false;
    GameDlTask onProgressTask = null;
    CancellationTokenSource cts;

    /// <summary>
    /// ダウンロードタスクの追加
    /// </summary>
    public void AddGameDlTask(GameDlTask task)
    {
        //同じタスク名(ダウンロードするゲームのID)のタスクの追加は許容しない
        if(GameDlTasksList.Any(x => x.TaskName == task.TaskName))
        {
            UnityEngine.Debug.LogError("現在実行待ちのタスクに同名のタスクがあります");
            return;
        }
        GameDlTasksList.Add(task);
        //タスク実行のループを発火
        SequentiallyDoTasks();
    }

    /// <summary>
    /// 実行中のダウンロードタスクを破棄する
    /// </summary>
    public void BreakProgressTask()
    {
        if(onProgressTask != null)
        {
            onProgressTask.TaskInstance.StopDlGame();
            RemoveOnProgressTask(onProgressTask.TaskName);
        }
    }

    /// <summary>
    /// タスク名を指定して実行待ちのリストからタスクを消去する
    /// </summary>
    public void RemoveSpecifiedTask(string taskName)
    {
        int index = GameDlTasksList.FindIndex(x => x.TaskName == taskName);
        if(index > 0)
        {
            GameDlTasksList.RemoveAt(index);
        }
        else if (index == 0)
        {
            if (doTaskFlag)
            {
                BreakProgressTask();
            }
            else
            {
                GameDlTasksList.RemoveAt(index);
            }
        }
    }

    /// <summary>
    /// オブジェクト破棄時にダウンロード中・ダウンロード待ちのタスクを全て破棄する
    /// </summary>
    private void OnDestroy()
    {
        //実行中以外の全てのタスクを破棄
        List<GameDlTask> progressTask = new List<GameDlTask>();
        progressTask.Add(onProgressTask);
        GameDlTasksList = progressTask;

        //実行中のタスクを破棄
        BreakProgressTask();
    }

    /// <summary>
    /// GameDlTasksのタスクをなくなるまで順番に実行する
    /// </summary>
    private async UniTask SequentiallyDoTasks()
    {
        if (doTaskFlag) return;
        while (GameDlTasksList.Count > 0)
        {
            doTaskFlag = true;
            cts = new CancellationTokenSource();
            //今回実行するタスク
            onProgressTask = GameDlTasksList[0];
            //進捗監視用のクラスをインスタンス
            GameDlProgress progress = new GameDlProgress(onProgressTask.TaskName, onProgressTask.TaskInstance.GameData.GameDirName);
            CurrentGameDlProgress = progress;

            //タスクの実行
            await onProgressTask.TaskInstance.DLGameInUniTask(cts.Token, CurrentGameDlProgress);

            //キューがロックされている間は解除されるまで除去するのを待機する


            //終了したタスクを終了済みとしてリストから除去する
            RemoveOnProgressTask(onProgressTask.TaskName);
        }
        doTaskFlag = false;
    }

    private void RemoveOnProgressTask(string taskName)
    {
        if (GameDlTasksList[0].TaskName == taskName)
        {
            GameDlTasksList.RemoveAt(0);
            onProgressTask = null;
            CurrentGameDlProgress = null;
            cts.Cancel();
        }
    }
}
