
public class GameDlError
{
    public GameDlTask Task { get; private set; }
    public GameDlCustomException DlException { get; private set; }

    public GameDlError(GameDlTask task, GameDlCustomException exception)
    {
        Task = task;
        DlException = exception;
    }
}
