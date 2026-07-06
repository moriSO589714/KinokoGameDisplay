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
        List<GameData> allGameDatas = _gameDatasSingleton.AllGameDatas;
        FilterCondition currentConditions = _gameDatasSingleton.CurrentFilterCondition;

        List<GameData> displayGameDatas = allGameDatas;
        //フィルタリングを通す
        if (currentConditions != null)
        {
            displayGameDatas = GameBoxFilter.FilteringGameDatas(currentConditions, allGameDatas);
        }
        _gameDatasSingleton.SetCurrentDisplayGames(displayGameDatas, currentConditions);
        
        //uiの生成
        _gameBoxsManager.GenerateBoxs(displayGameDatas);
    }

    public async UniTask SetAllGameBoxfromNet()
    {
        await UniTask.RunOnThreadPool(_gameDataManager.LoadGameDataFromSpSt);
        List<GameData> allGameDatas = _gameDatasSingleton.AllGameDatas;
        FilterCondition currentConditions = _gameDatasSingleton.CurrentFilterCondition;

        List<GameData> displayGameDatas = allGameDatas;
        //フィルタリングを通す
        if(currentConditions != null)
        {
            displayGameDatas = GameBoxFilter.FilteringGameDatas(currentConditions, allGameDatas);
        }
        _gameDatasSingleton.SetCurrentDisplayGames(displayGameDatas, currentConditions);

        //uiの生成
        _gameBoxsManager.GenerateBoxs(displayGameDatas);
    }

    public void NoLoadSetCurrentDisplayGameBox()
    {
        List<GameData> gameDatas = _gameDatasSingleton.CurrentDisplayGames;
        _gameBoxsManager.GenerateBoxs(gameDatas);
    }
}