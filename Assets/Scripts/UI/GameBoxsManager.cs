using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameBoxsManager : MonoBehaviour
{
    [SerializeField] GameObject GameBox;
    [SerializeField] float BoxInterval;
    [SerializeField] float SclollWidth;

    [SerializeField] MonitorPlayerInput monitorPlayerInput;

    float _startTimeYPos;
    List<GameObject> _instantiatedGameBoxs= new List<GameObject>();

    private void Awake()
    {
        _startTimeYPos = this.GetComponent<RectTransform>().anchoredPosition.y;
        monitorPlayerInput.onMouseScroll += OnScroll;
    }

    /// <summary>
    /// GameDataクラスのリストからUIであるGameBoxをインスタンスして配置する
    /// </summary>
    /// <param name="gameDatas"></param>
    public void SetGameBoxsByGameDataList(List<GameData> gameDatas)
    {
        int count = 0;
        foreach(GameData gameData in gameDatas)
        {
            Vector2 createPos = new Vector2(0,-(count * BoxInterval));
            Debug.Log(createPos);
            //UIのInstantiateはrecttransformでやってくれる
            GameObject gameBox = Instantiate(GameBox, createPos, Quaternion.identity);
            //拡大率が変わってしまうためSetParent()の第二引数はfalse、そのためInstantiate()の座標は相対座標で指定する
            gameBox.transform.SetParent(transform, false);
            gameBox.GetComponent<GameBox>()?.SetDataByGameData(gameData);
            _instantiatedGameBoxs.Add(gameBox);
            count++;
        }
    }

    public void OnScroll(float scrollDirection)
    {
        if(scrollDirection < 0)
        {
            float targetYPos = this.GetComponent<RectTransform>().anchoredPosition.y + SclollWidth;
            float limitYPos = (_instantiatedGameBoxs.Count - 1) * BoxInterval;
            if (targetYPos >= limitYPos)
            {
                targetYPos = limitYPos;
            }

            this.GetComponent<RectTransform>().anchoredPosition = new Vector2(this.GetComponent<RectTransform>().anchoredPosition.x, targetYPos);
        }
        else if(scrollDirection > 0)
        {
            float targetYPos = this.GetComponent<RectTransform>().anchoredPosition.y - SclollWidth;
            if (targetYPos <= _startTimeYPos)
            {
                targetYPos = _startTimeYPos;
            }

            this.GetComponent<RectTransform>().anchoredPosition = new Vector2(this.GetComponent<RectTransform>().anchoredPosition.x, targetYPos);
        }
    }

    /// <summary>
    /// 全てのゲームボックスを削除する
    /// </summary>
    public void DeleteAllGameBoxs()
    {
        //子オブジェクトから、GameBoxクラスの付いているオブジェクトを全て削除する
        foreach (Transform child in gameObject.transform)
        {
            if (child.gameObject.GetComponent<GameBox>())
            {
                Destroy(child.gameObject);
            }
        }
    }
}
