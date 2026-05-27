using UnityEngine.UI;

/// <summary>
/// Textオブジェクトを対象に現在表示するべきUI部分に内部処理のログを表示させる
/// </summary>
public class NotificationDisplayer : BasedSingletonInMono<NotificationDisplayer>
{
    private Text currentDisplayUI;
    public int counter { get; private set; } = 0;
    const int counterMaxVal = 100;

    public void SetNewDisplayUI(Text textUIBox)
    {
        currentDisplayUI = textUIBox;
    }

    public void DisplayNotification(string notification)
    {
        if (currentDisplayUI == null || !currentDisplayUI.IsActive()) return;
        currentDisplayUI.text = notification;
        Count();
    }

    private void Count()
    {
        if (counter > counterMaxVal) counter = 1;
        else counter++;
    }
}
