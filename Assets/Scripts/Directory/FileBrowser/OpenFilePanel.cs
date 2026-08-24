using SFB;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OpenFilePanel
{
    public string OpenFilePanelAndReturnPath(ExtensionFilter[] extensionFilters)
    {
        string panelTitle = "ファイルを選択";
        string[] paths = null;
        if(extensionFilters == null)
        {
            paths = StandaloneFileBrowser.OpenFilePanel(panelTitle, "", "", false);
        }
        else
        {
            paths = StandaloneFileBrowser.OpenFilePanel(panelTitle, "", extensionFilters, false);
        }        

        if(paths.Count() == 1)
        {
            return paths[0];
        }
        else if(paths.Count() == 0)
        {
            return null;
        }
        else
        {
            throw new System.Exception("1つ以上のファイルが選択されました。");
        }
    }

    public string OpenFolderPanelAndReturnPath()
    {
        string[] paths = StandaloneFileBrowser.OpenFolderPanel("フォルダを選択", "", false);

        if(paths.Count() == 1)
        {
            return paths[0];
        }
        else if(paths.Count() == 0)
        {
            return null;
        }
        else
        {
            throw new System.Exception("1つ以上のフォルダが選択されました。");
        }
    }
}
