
using System.Collections.Generic;

public interface OnNetAppEndGameInfo
{
    /// <summary>
    /// 1ゲームぶんの情報を一括で新規の行に追加する
    /// </summary>
    /// <param name="appEndGameInfos">スプレッドシートの要素順に合わせて並び替えたゲーム情報のリスト</param>
    void AppEndGameInfo(List<string> appEndGameInfos);
}
