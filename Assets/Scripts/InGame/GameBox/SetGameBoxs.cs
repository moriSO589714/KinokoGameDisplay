using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class SetGameBoxs
{
    GameBoxsManager _gameBoxsManager;
    GameDataManager _gameDataManager;
    GameDatasSingleton _gameDatasSingleton;

    public SetGameBoxs(GameBoxsManager gameBoxsManager)
    {
        _gameBoxsManager = gameBoxsManager;
        _gameDataManager = new GameDataManager();
        _gameDatasSingleton = GameDatasSingleton.Instance;
    }

    public async UniTask SetAllGameBoxfromLocal()
    {
        await UniTask.RunOnThreadPool(_gameDataManager.LoadGameDataFromJsons);
        List<GameData> gameDatas = _gameDatasSingleton.AllGameDatas;
        _gameBoxsManager.GenerateBoxs(gameDatas);
    }

    public async UniTask SetAllGameBoxfromNet()
    {
        await UniTask.RunOnThreadPool(_gameDataManager.LoadGameDataFromSpSt);
        List<GameData> gameDatas = _gameDatasSingleton.AllGameDatas;
        _gameBoxsManager.GenerateBoxs(gameDatas);
    }

    /// <summary>
    /// ゲームの読み込みを行わず、現在シングルトンに登録されているものからゲームボックスを生成
    /// </summary>
    public void NoLoadSetAllGameBox()
    {
        List<GameData> gameDatas = _gameDatasSingleton.AllGameDatas;
        _gameBoxsManager.GenerateBoxs(gameDatas);
    }
}