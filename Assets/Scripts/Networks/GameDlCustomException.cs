using System;

/// <summary>
/// ゲームダウンロード時に発生させるエラーハンドリング用の独自例外クラス
/// </summary>
public class GameDlCustomException : Exception
{
    public GameDlErrorType GameDlErrorType { get; private set; }

    /// <summary>
    /// デフォルトコンストラクタ
    /// </summary>
    public GameDlCustomException()
    {

    }

    /// <summary>
    /// エラーメッセージを受け取るコンストラクタ
    /// </summary>
    public GameDlCustomException(string message, GameDlErrorType gameDlErrorType) : base(message) 
    {
        GameDlErrorType = gameDlErrorType;
    }

    /// <summary>
    /// メッセージと内部例外を受け取るコンストラクタ
    /// </summary>
    public GameDlCustomException(string message, Exception innerException, GameDlErrorType gameDlErrorType) : base(message, innerException)
    {
        GameDlErrorType = gameDlErrorType;
    }
}

public enum GameDlErrorType 
{
    Unknown,
    NeedCleanDirectory,
    NeedRetryAccessDrive,
    ImpossibleRecoveryErrorOnDrive,
    ImpossibleRecoveryErrorOnRuntime,
    Others
}