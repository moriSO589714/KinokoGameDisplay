
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
        _gameBoxsManager.SetGameBoxsByGameDataList(gameDatas);
    }

    public async UniTask SetAllGameBoxfromNet()
    {
        await UniTask.RunOnThreadPool(_gameDataManager.LoadGameDataFromSpSt);
        List<GameData> gameDatas = _gameDatasSingleton.AllGameDatas;
        _gameBoxsManager.SetGameBoxsByGameDataList(gameDatas);
    }
}
