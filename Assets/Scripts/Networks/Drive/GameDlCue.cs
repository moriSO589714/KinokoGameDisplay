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
    GameDlTask onProgressTask = null;
    CancellationTokenSource cts;

    /// <summary>
    /// ダウンロードタスクの追加
    /// </summary>
    public void AddGameDlTask(GameDlTask task)
    {
        HandleTaskList(-1, -1, task);
    }

    /// <summary>
    /// オブジェクト破棄時にダウンロード中・ダウンロード待ちのタスクを全て破棄する
    /// </summary>
    private void OnDestroy()
    {
        //実行中以外の全てのタスクを破棄
        List<GameDlTask> progressTask = new List<GameDlTask>(1) { onProgressTask };
        GameDlTasksList = progressTask;

        //実行中のタスクを破棄
        DestroyProgressTask();
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

            //終了したタスクを終了済みとしてリストから除去する
            DestroyProgressTask();
        }
        doTaskFlag = false;
    }

    private void DestroyProgressTask()
    {
        //実行中のタスクがない場合は処理しない
        if (onProgressTask == null) return;

        HandleTaskList(0, -1);
    }

    /// <summary>
    /// タスクが入っているリストを直接操作できるメソッド
    /// ※このメソッド以外からリストは操作しない
    /// </summary>
    /// <param name="currentIndex">操作対象となる要素の現在のインデックス値。タスクの追加時は-1を指定</param>
    /// <param name="targetIndex">操作実行後のインデックス値。最後尾にタスクを追加する際とタスクの破棄を行う際は-1を指定</param>
    /// <param name="addValue">タスク追加時に追加するタスク</param>
    private void HandleTaskList(int currentIndex, int targetIndex, GameDlTask addValue = null)
    {
        //タスク追加時
        if(currentIndex == -1 && addValue != null)
        {
            AddTask(targetIndex, addValue);
        }
        else
        {
            //タスク削除
            if(targetIndex == -1)
            {
                DeleteTask(currentIndex);
            }
            else //タスクの入れ替え
            {
                ReplaceTask(currentIndex, targetIndex);
            }
        }
        //タスクを止めていた際のためにループを再実行する
        SequentiallyDoTasks();
    }

    private void AddTask(int targetIndex, GameDlTask addValue)
    {
        //最新部にタスクを追加する場合は現在の処理を中断させる
        if(targetIndex == 0)
        {
            StopProgressTask(GameDlTasksList[0].TaskName);
        }

        //同じタスク名のタスクがリストに既に存在する場合は処理を中断する
        if (GameDlTasksList.Any(x => x.TaskName == addValue.TaskName))
        {
            UnityEngine.Debug.LogError("現在実行待ちのタスクに同名のタスクがあります");
            return;
        }

        if (targetIndex == -1)
        {
            //最後尾にタスクを追加する場合
            GameDlTasksList.Add(addValue);
        }
        else
        {
            GameDlTasksList.Insert(targetIndex, addValue);
        }
    }

    private void DeleteTask(int currentIndex)
    {
        //現在進行中のタスクを削除する場合は処理を停止させておく
        if (currentIndex == 0)
        {
            StopProgressTask(GameDlTasksList[0].TaskName);
        }

        GameDlTasksList.RemoveAt(currentIndex);
    }

    private void ReplaceTask(int currentIndex, int targetIndex)
    {
        if(targetIndex == 0)
        {
            StopProgressTask(GameDlTasksList[0].TaskName);
        }
        //入れ替えの実行
        (GameDlTasksList[currentIndex], GameDlTasksList[targetIndex]) = (GameDlTasksList[targetIndex], GameDlTasksList[currentIndex]);
    }

    /// <summary>
    /// 現在進行中のタスクの中断のみを行う
    /// </summary>
    private void StopProgressTask(string taskName)
    {
        if (GameDlTasksList[0].TaskName != taskName)
        {
            throw new System.Exception("指定されたタスク名と進行中のタスク名が異なるためタスクを中断できません");
        }
        onProgressTask.TaskInstance.StopDlGame();
        doTaskFlag = false;
        onProgressTask = null;
        CurrentGameDlProgress = null;
        cts.Cancel();
    }
}
