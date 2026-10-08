using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface OnNetUpdateGameInfo
{
    void UpdateGameInfo(List<string> newGameInfo, int targetRow);
}
