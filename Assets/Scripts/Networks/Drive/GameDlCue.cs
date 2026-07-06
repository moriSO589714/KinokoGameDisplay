using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class GameDlCue : MonoBehaviour
{
    //実行待ちのタスクリスト
    public List<GameDlTask> GameDlTasksList { get; private set; } = new List<GameDlTask>();
    //エラーが発生したタスクのリスト
    public List<GameDlError> ErrorTasksList { get; private set; } = new List<GameDlError>();

    //進捗監視用のオブジェクト
    public GameDlProgress CurrentGameDlProgress { get; private set; } = null;

    //タスク実行中のフラグ
    bool doTaskFlag = false;
    GameDlTask onProgressTask = null;
    CancellationTokenSource ctsForDl;
    CancellationTokenSource ctsForRecovery;

    /// <summary>
    /// ダウンロードタスクの追加
    /// </summary>
    public void AddGameDlTask(GameDlTask task)
    {
        HandleTaskList((-1, null), (-1, null), task);
    }

    /// <summary>
    /// エラーが発生したタスクについて、復旧動作を行ってから再度ダウンロードキューに入れる
    /// </summary>
    public async UniTask RecoveryAndDlInErrorTasksList(int errorTasksListIndex)
    {
        GameDlError targetError = ErrorTasksList[errorTasksListIndex];

        ctsForRecovery?.Cancel();
        ctsForRecovery = new CancellationTokenSource();
        //リカバリの実行
        await new GameDlErrorRecovery().RecoveryError(targetError, ctsForRecovery.Token);
        
        //エラーリストから除去した後、ダウンロードキューに追加
        DeleteErrorTask(errorTasksListIndex);
        AddGameDlTask(targetError.Task);
    }

    public void DeleteTaskFromIndex(int index, string taskName)
    {
        HandleTaskList((index, taskName), (-1, null));
    }

    /// <summary>
    /// エラータスクリストからタスクを削除する
    /// </summary>
    public void DeleteErrorTask(int index)
    {
        ErrorTasksList[index].Task.TaskInstance.ForceEndThisProc();
        ErrorTasksList.RemoveAt(index);
    }

    /// <summary>
    /// 任意のインデックスのタスクと現在実行中のタスクを入れ替える
    /// </summary>
    public void ReplaceProgressTaskWithNewTask(int newTaskIndex, string newTaskName)
    {
        if (newTaskIndex == 0) return;
        string progressTaskName = GameDlTasksList[0].TaskName;
        HandleTaskList((newTaskIndex, newTaskName), (0, progressTaskName));
    }

    /// <summary>
    /// タスクの入れ替え(対象に現在実行中のタスクが含まれない)
    /// </summary>
    public void ReplaceTaskIndex((int index, string taskName) current, (int index, string taskName) target)
    {
        if (current.index == 0 || target.index == 0) return;
        HandleTaskList(current, target);
    }

    /// <summary>
    /// オブジェクト破棄時にダウンロード中・ダウンロード待ちのタスクを全て破棄する
    /// </summary>
    private void OnDestroy()
    {
        ctsForRecovery?.Cancel();

        //実行中以外の全てのタスクを破棄
        List<GameDlTask> progressTask = new List<GameDlTask>(1) { onProgressTask };
        GameDlTasksList = progressTask;

        //実行中のタスクを破棄
        DestroyProgressTask();
    }

    private async UniTaskVoid StartLoopSequence()
    {
        await UniTask.WaitUntil(() => !doTaskFlag);

        SequentiallyDoTasks().Forget();
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
            ctsForDl = new CancellationTokenSource();
            //今回実行するタスク
            onProgressTask = GameDlTasksList[0];
            //進捗監視用のクラスをインスタンス
            GameDlProgress progress = new GameDlProgress(onProgressTask.TaskName, onProgressTask.TaskInstance.GameData.GameDirName);
            CurrentGameDlProgress = progress;
            try
            {
                //タスクの実行
                await onProgressTask.TaskInstance.DLGameInUniTask(ctsForDl.Token, CurrentGameDlProgress);
            }
            catch(GameDlCustomException e)
            {
                UnityEngine.Debug.LogError(e.Message);
                ErrorTasksList.Add(new GameDlError(onProgressTask, e));
            }
            catch(System.Exception e)
            {
                //タスクキャンセルによる終了
                if (ctsForDl.IsCancellationRequested)
                {
                    doTaskFlag = false;
                    return;
                }

                UnityEngine.Debug.LogError(e);
                //エラー種別の解析を行う
                GameDlCustomException gameDlCustomException = new GameDlErrorrSpecify().SpecifyError(e);
                ErrorTasksList.Add(new GameDlError(onProgressTask, gameDlCustomException));
            }

            if(!ctsForDl.IsCancellationRequested)
            {
                //終了したタスクを終了済みとしてリストから除去する
                DestroyProgressTask();
            }
            GameDatasSingleton gs = GameDatasSingleton.Instance;
        }

        doTaskFlag = false;
    }

    /// <summary>
    /// 進行中のタスクを破棄する
    /// </summary>
    private void DestroyProgressTask()
    {
        //実行中のタスクがない場合は処理しない
        if (onProgressTask == null) return;

        HandleTaskList((0, GameDlTasksList[0].TaskName), (-1, null));
    }

    /// <summary>
    /// タスクが入っているリストを直接操作できるメソッド
    /// ※このメソッド以外からリストは操作しない
    /// </summary>
    /// <param name="currentIndex">操作対象となる要素の現在のインデックス値。タスクの追加時は-1を指定</param>
    /// <param name="targetIndex">操作実行後のインデックス値。最後尾にタスクを追加する際とタスクの破棄を行う際は-1を指定</param>
    /// <param name="addValue">タスク追加時に追加するタスク</param>
    private void HandleTaskList
        ((int index, string taskName) current, (int index, string taskName) target, GameDlTask addValue = null)
    {
        //想定と異なる操作が行われる場合(予期しないリストの変更等により)はここで処理を中断
        if(current.index != -1)
        {
            if(current.index > GameDlTasksList.Count - 1 || GameDlTasksList[current.index].TaskName != current.taskName)
            {
                return;
            }
        }
        if(target.index != -1)
        {
            if(target.index > GameDlTasksList.Count)
            {
                return;
            }
            else if (target.index < GameDlTasksList.Count - 1 && GameDlTasksList[target.index].TaskName != target.taskName)
            {
                return;
            }
        }

        //タスク追加時
        if(current.index == -1 && addValue != null)
        {
            AddTask(target.index, addValue);
        }
        else
        {
            //タスク削除
            if(target.index == -1)
            {
                DeleteTask(current.index);
            }
            else //タスクの入れ替え
            {
                ReplaceTask(current.index, target.index);
            }
        }
        //タスクを止めていた際のためにループを再実行する
        StartLoopSequence();
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
        if (targetIndex > GameDlTasksList.Count - 1) return;

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
        onProgressTask.TaskInstance.ForceEndThisProc();
        CurrentGameDlProgress = null;
        ctsForDl?.Cancel();
    }
}
