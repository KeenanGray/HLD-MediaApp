using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//This script should be added to the main canvas of the app

public class AspectRatioManager : MonoBehaviour {
    public static float ScreenWidth;
    public static float ScreenHeight;

    public static bool AspectRatioSet;
    public static bool Stopped;

    private void Update()
    {
        if (!Stopped)
        {
            ScreenWidth = gameObject.GetComponent<RectTransform>().rect.width;
            ScreenHeight = gameObject.GetComponent<RectTransform>().rect.height;
        }
    }

    public IEnumerator GetScreenResolution()
    {
        yield return new WaitForEndOfFrame();

        ScreenWidth = gameObject.GetComponent<RectTransform>().rect.width;
        ScreenHeight = gameObject.GetComponent<RectTransform>().rect.height;

        if(!Stopped)
            AspectRatioSet = true;
        else
            AspectRatioSet = false;

        yield break;
    }

}
