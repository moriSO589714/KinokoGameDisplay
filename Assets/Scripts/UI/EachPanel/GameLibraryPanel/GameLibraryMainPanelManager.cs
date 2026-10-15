using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLibraryMainPanelManager : MonoBehaviour
{
    [SerializeField] private Text _titleText;
    [SerializeField] private Text _devNameText;
    [SerializeField] private Text _tagsText;
    [SerializeField] private Text _descriptionText;
    [SerializeField] private Image _thumbnailImage;
    [SerializeField] private DetailInfoManager _detailInfoManager;


    [SerializeField] private GameLibrarySizeFitter _gameLibrarySizeFitter;

    private int _assignNolimit = -1;

    private GameData _currentGameData;
    private Vector2 _currentFloorPos;

    public void SetData(GameData newGameData)
    {
        _currentGameData = newGameData;

        GameDataSetterForUI.SetPlainStrData(_currentGameData.GameTitle, _assignNolimit, _titleText);
        GameDataSetterForUI.SetPlainStrData(string.Join(",", _currentGameData.GameDevelopper), _assignNolimit, _devNameText);
        GameDataSetterForUI.SetPlainStrData(_currentGameData.GameDescription, _assignNolimit, _descriptionText);
        GameDataSetterForUI.SetPlainStrData("#" + string.Join(",#", _currentGameData.GameTags), _assignNolimit, _tagsText);
        _detailInfoManager.SetData(_currentGameData);

        //UIの変更を即時実行
        Canvas.ForceUpdateCanvases();

        _currentFloorPos = _gameLibrarySizeFitter.AutoFit();
    }
}
