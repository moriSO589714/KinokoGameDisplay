using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// ディレクトリ関係のメソッド
/// </summary>
public class DirectorySystems : MonoBehaviour
{
    //ディレクトリが存在するかの確認と生成されていないなら生成
    public async UniTask CheckDirectory()
    {
        DirectoryPaths _directoryPaths = new DirectoryPaths();
        string _baseDirectory = _directoryPaths.BaseDirectory;
        string _gameFilePath = _baseDirectory +"/"+ _directoryPaths.GameFilePath;
        string _jsonFolderPath = _baseDirectory +"/"+ _directoryPaths.JsonFolderPath;
        string _imageFolderPath = _baseDirectory +"/"+ _directoryPaths.ImageFolderPath;
    }

    
}
