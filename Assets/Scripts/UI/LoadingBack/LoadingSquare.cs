using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class LoadingSquare : MonoBehaviour
{
    //移動幅
    [SerializeField] float MoveWidth = 0;
    [SerializeField] float WaitingTime = 1;
    [SerializeField] GameObject _movingSquareObject;

    CancellationTokenSource cancellationTokenSource;
    private Vector2 firstPos = new Vector2();
    private int counter;
    private bool flag = false;

    void Start()
    {
        Init();
    }

    void Update()
    {
        if (!flag)
        {
            flag = true;
            try
            {
                MoveTrigger(cancellationTokenSource.Token);
            }
            catch
            {

            }
        }
    }

    private void OnDisable()
    {
        Init();
    }

    private void OnDestroy()
    {
        Init();
    }

    /// <summary>
    /// 自身の親オブジェクトを変更する。
    /// その際、このオブジェクトは新しいオブジェクトの中央に適切な縮尺で表示される
    /// ※Canvasオブジェクト下にあること！
    /// </summary>
    public void ChangeParent(GameObject newParent)
    {
        this.gameObject.transform.SetParent(newParent.transform, false);
        Init();
    }

    private async UniTask MoveTrigger(CancellationToken cancellationToken)
    {
        await UniTask.WaitForSeconds(WaitingTime, cancellationToken: cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        Move();
        flag = false;
    }

    private void Move()
    {
        Vector2 movePosition = _movingSquareObject.GetComponent<RectTransform>().anchoredPosition;
        if (counter < 2)
        {
            movePosition = new Vector2(movePosition.x, movePosition.y + MoveWidth);
        }
        else if (2 <= counter && counter < 4)
        {
            movePosition = new Vector2(movePosition.x + MoveWidth, movePosition.y);
        }
        else if (4 <= counter && counter < 6)
        {
            movePosition = new Vector2(movePosition.x, movePosition.y - MoveWidth);
        }
        else if (6 <= counter && counter < 8)
        {
            movePosition = new Vector2(movePosition.x - MoveWidth, movePosition.y);
        }
        counter++;
        if(counter == 8)
        {
            counter = 0;
        }

        _movingSquareObject.GetComponent<RectTransform>().anchoredPosition = movePosition;
    }

    private void Init()
    {
        if(cancellationTokenSource != null) cancellationTokenSource.Cancel();
        cancellationTokenSource = new CancellationTokenSource();
        if(firstPos == Vector2.zero)
        {
            firstPos = _movingSquareObject.transform.position;
        }
        else
        {
            _movingSquareObject.transform.position = firstPos;
        }
        counter = 0;
        flag = false;
    }
}
