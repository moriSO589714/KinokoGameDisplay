using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class SortPanelManager : UIPanel
{
    [SerializeField] private CategorySortManager _categorySortManager;
    [SerializeField] private AscDescManager _ascAndDescManager;
    [SerializeField] private UIActBase _onSortingButton;
    [SerializeField] private GameBoxsManager _gameBoxManager;
    [SerializeField] private Text _errorDisplay;
    [SerializeField] private UIActBase _closeThisPanelButton;

    private GameDatasSingleton _gameDatasSingleton;
    private FieldInfo[] _gameDataFieldInfos;

    protected override void Awake()
    {
        base.Awake();
        _gameDataFieldInfos = typeof(GameData).GetFields();
        _gameDatasSingleton = GameDatasSingleton.Instance;
        _onSortingButton.ClickAct += DoSort;
        _closeThisPanelButton.ClickAct += OnCloseProc;
    }

    protected override void OnCloseProc()
    {
        //各項目の初期化
        _categorySortManager.InitSortElement();
        _ascAndDescManager.InitSortElement();
        _errorDisplay.text = "";
        base.OnCloseProc();
    }

    private void DoSort()
    {
        SortLibrary sortLib = new SortLibrary();
        //並べ替えるカテゴリの取得
        string sortingCategory = GetSelectedElement(_categorySortManager);
        if(sortingCategory == "")
        {
            _errorDisplay.text = "絞り込む項目を選択してください";
            return;
        }
        string sortingCategoryFieldName = sortLib.CategoryDic.FirstOrDefault(x => x.Value == sortingCategory).Key;
        //昇順か降順かを取得
        string ascOrDesc = GetSelectedElement(_ascAndDescManager);
        if(ascOrDesc == "")
        {
            _errorDisplay.text = "昇順か降順かを選択してください";
            return;
        }
        bool ascOrDescFlag = sortLib.AscOrDescDic.FirstOrDefault(x => x.Value == ascOrDesc).Key;

        //GameDatasシングルトンから現在表示しているゲームデータ群を取得
        List<GameData> currentDisplayGameDatas = new List<GameData>(_gameDatasSingleton.CurrentDisplayGames);
        //絞り込み条件を取得しておく
        FilterCondition filterCondition = _gameDatasSingleton.CurrentFilterCondition;
        //ソートを実行
        currentDisplayGameDatas = SortingForGameDatas(ascOrDescFlag,sortingCategoryFieldName,currentDisplayGameDatas);
        //新しくシングルトンに登録
        _gameDatasSingleton.SetCurrentDisplayGames(currentDisplayGameDatas, filterCondition);
        //メイン画面のゲームボックスを新しく生成する
        _gameBoxManager.GenerateBoxs(_gameDatasSingleton.CurrentDisplayGames);

        OnCloseProc();
    }

    private string GetSelectedElement(SortElementManager sortManager)
    {
        return sortManager._currentSelectElement;
    }

    private List<GameData> SortingForGameDatas(bool ascOrDesc, string elementName, List<GameData> targetGameDatasList)
    {
        FieldInfo targetField = null;
        foreach (FieldInfo f in _gameDataFieldInfos)
        {
            if(f.Name == elementName)
            {
                targetField = f;
            }
        }
        if(targetField == null)
        {
            throw new System.Exception("選択された項目と一致する変数名がGameDataフィールドに存在しません");
        }

        //昇順ソート
        if (ascOrDesc)
        {
            targetGameDatasList = targetGameDatasList.OrderBy(x => targetField.GetValue(x)).ToList();
        }
        else//降順ソート
        {
            targetGameDatasList = targetGameDatasList.OrderByDescending(x => targetField.GetValue(x)).ToList();
        }

        return targetGameDatasList;
    }
}