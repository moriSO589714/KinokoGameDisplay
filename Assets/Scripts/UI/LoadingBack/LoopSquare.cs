using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class LoopSquare : MonoBehaviour
{
    //移動幅
    [SerializeField] float MoveWidth = 0;
    [SerializeField] float WaitingTime = 1;
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

    private async UniTask MoveTrigger(CancellationToken cancellationToken)
    {
        await UniTask.WaitForSeconds(WaitingTime, cancellationToken: cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        Move();
        flag = false;
    }

    private void Move()
    {
        Vector2 movePosition = GetComponent<RectTransform>().anchoredPosition;
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

        GetComponent<RectTransform>().anchoredPosition = movePosition;
    }

    private void Init()
    {
        if(cancellationTokenSource != null) cancellationTokenSource.Cancel();
        cancellationTokenSource = new CancellationTokenSource();
        if(firstPos == Vector2.zero)
        {
            firstPos = transform.position;
        }
        else
        {
            transform.position = firstPos;
        }
        counter = 0;
        flag = false;
    }
}
