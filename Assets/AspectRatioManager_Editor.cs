using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//This script should be added to the main canvas of the app
[ExecuteInEditMode]
public class AspectRatioManager_Editor : MonoBehaviour
{
#if UNITY_EDITOR
    public float ScreenWidth;
    public float ScreenHeight;

    public bool AspectRatioSet;
    public bool IsInEditor;

    static AspectRatioManager_Editor aspectRatioManager;

    public static AspectRatioManager_Editor Instance()
    {
        if (aspectRatioManager == null)
        {
            aspectRatioManager = GameObject.FindWithTag("MainCanvas").GetComponent<AspectRatioManager_Editor>();
            return aspectRatioManager;
        }
        else
            return aspectRatioManager;
    }

    private void Update()
    {
        if (IsInEditor)
        {
            Debug.Log("Run");
            GetScreenResolution();
        }
    }

    public void GetScreenResolution()
    {
        ScreenWidth = Screen.width;
        ScreenHeight = Screen.height;

        float right = ScreenWidth * 2;
        float up = ScreenHeight / 2;

        foreach (AspectRatioFitter arf in GetComponentsInChildren<AspectRatioFitter>())
        {
            // var tmp = arf.GetComponent<RectTransform>().position;
            //if get component has a "page", move it into a nice position;
            if ((arf.GetComponent<Page>() != null || arf.GetComponent<SubMenu>()!=null) && arf.tag!="App_Biography" && arf.tag!="Pool")
            {
                var pos = new Vector3(right, up, 0);
                right += ScreenWidth+ 100;
                arf.GetComponent<RectTransform>().position = new Vector3((int) pos.x, (int) pos.y, (int)pos.z);
            }

            if (arf.tag == "App_Biography")
            {
                var pos = new Vector3(right, up, 0);
                arf.GetComponent<RectTransform>().position = new Vector3((int)pos.x, (int)pos.y, (int)pos.z);
            }

            arf.enabled = true;
            arf.aspectRatio = (ScreenWidth) / (ScreenHeight);
            arf.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            arf.enabled = false;

        }
    }
#endif
}
