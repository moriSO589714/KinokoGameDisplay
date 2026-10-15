
using UnityEngine;
using UnityEngine.UI;

public class DetailInfoManager : MonoBehaviour
{
    [SerializeField] private Text _dirNameText;
    [SerializeField] private Text _exeNameText;
    [SerializeField] private Text _idText;
    [SerializeField] private Text _versionText;
    [SerializeField] private Text _toolText;
    [SerializeField] private Text _driveIdText;
    [SerializeField] private Text _imageIdText;
    [SerializeField] private Text _statusText;

    private int _assignNolimit = -1;

    private GameData _myGameData;

    public void SetData(GameData newGameData)
    {
        _myGameData = newGameData;
        GameDataSetterForUI.SetPlainStrData(_myGameData.GameDirName, _assignNolimit, _dirNameText);
        GameDataSetterForUI.SetPlainStrData(_myGameData.GameExeName, _assignNolimit, _exeNameText);        
        GameDataSetterForUI.SetPlainStrData(_myGameData.GameID, _assignNolimit, _idText);
        GameDataSetterForUI.SetPlainStrData(_myGameData.GameVersion, _assignNolimit, _versionText);
        GameDataSetterForUI.SetPlainStrData(_myGameData.GameSoftwareType, _assignNolimit, _toolText);
        GameDataSetterForUI.SetPlainStrData(_myGameData.GameDriveId, _assignNolimit, _driveIdText);
        GameDataSetterForUI.SetPlainStrData(_myGameData.GameImageId, _assignNolimit, _imageIdText);
        GameDataSetterForUI.SetPlainStrData(_myGameData.Status.ToString(), _assignNolimit, _statusText);
    }
}
