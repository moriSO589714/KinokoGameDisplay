using System.Collections.Generic;
using UnityEngine;

public interface OnNetGameInfo
{
    List<List<string>> GetGameInfo(Vector2 startPos, Vector2 endPos);
}