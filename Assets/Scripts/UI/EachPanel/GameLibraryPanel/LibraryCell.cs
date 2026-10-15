using System;
using UnityEngine;
using UnityEngine.UI;

public class LibraryCell : Box
{
    [SerializeField] Text GameTitleText;
    [SerializeField] Image StatusLamp;
    [SerializeField] GameObject CellSpot;

    //アクティブ化する際に実行するアクション
    public Action<LibraryCell> _activateAct;
    private bool _isActive = false;

    public GameData _myGameData { get; private set; }

    public override void SetDataMyBox<T>(T originData)
    {
        base.SetDataMyBox(originData);

        //型をキャストする
        GameData originGameData = originData as GameData;
        _myGameData = originGameData;
        SetTitle(_myGameData.GameTitle);
        SetStatus(_myGameData.Status);
    }

    /// <summary>
    /// このセルがクリックされた際にアクティブ化させる処理
    /// </summary>
    public void ActivateThisCell()
    {
        if (_isActive) return;
        _isActive = true;

        _activateAct?.Invoke(this);

        CellSpot.SetActive(true);
    }

    /// <summary>
    /// このセルがアクティブ状態(クリックされている状態)では無くなった際に呼ばれる
    /// </summary>
    public void InactiveThisCell()
    {
        if (!_isActive) return;
        _isActive = false;

        CellSpot.SetActive(false);
    }

    private void SetTitle(string title)
    {
        GameTitleText.text = title;
    }

    private void SetStatus(GameStatus currentStatus)
    {
        Color lampColor = Color.white;
        switch (currentStatus) 
        {
            case GameStatus.ByLocal:
                lampColor = Color.green;
                break;
            case GameStatus.Downloaded:
                lampColor = Color.green;
                break;
            case GameStatus.UpdateAvailable:
                lampColor = Color.blue;
                break;
            case GameStatus.NotDownloaded:
                lampColor = Color.gray;
                break;
            case GameStatus.Downloading:
                lampColor = Color.gray;
                break;
        }
        StatusLamp.color = lampColor;
    }
}
