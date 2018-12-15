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

        foreach (AspectRatioFitter arf in GetComponentsInChildren<AspectRatioFitter>())
        {
            var tmp =arf.GetComponent<RectTransform>().position;

            arf.enabled = true;
            arf.aspectRatio = (ScreenWidth) / (ScreenHeight);
            arf.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            arf.enabled = false;

            arf.GetComponent<RectTransform>().position = new Vector3((int) tmp.x, (int) tmp.y, (int)tmp.z);
        }
    }
#endif
}
