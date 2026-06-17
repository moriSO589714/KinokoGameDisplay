using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// シーン遷移を行う
/// </summary>
public class CmdScene : MonoBehaviour
{
    private CommonStateManager stateManager;
    private void Awake()
    {
        stateManager = CommonStateManager.Instance;
    }

    public void TransitionMainScene()
    {
        TransitionScene(SceneStates.Main);
    }

    private void TransitionScene(SceneStates toScene)
    {
        stateManager.SetCurrentState(toScene);
    }
}
