using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// シーンのロードを行うクラス
/// </summary>
public class SceneLoader
{
    /// <summary>
    /// シーンのロードを行う
    /// </summary>
    /// <param name="sceneName">シーン名</param>
    public void OnLoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
