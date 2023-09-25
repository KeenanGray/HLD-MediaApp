using System.Collections;
using System.Collections.Generic;
using UI_Builder;
using UnityEngine;

public class AssignUrlFromAssetBundle : MonoBehaviour
{
    public string filename;

    public void UpdateURL()
    {
        UIB_AssetBundleHelper.tryLoadAssetBundle("hld/general");
        
        var u_button = GetComponent<UIB_Button>();
        u_button.s_link =
            UIB_FileManager
                .ReadTextAssetBundle(filename, "hld/general");
    }
}
