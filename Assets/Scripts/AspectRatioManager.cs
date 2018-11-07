using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//This script should be added to the main canvas of the app
//

[ExecuteInEditMode]
public class AspectRatioManager : MonoBehaviour {
    public static float ScreenWidth;
    public static float ScreenHeight;

    public static bool AspectRatioSet;
    public static bool Stopped;

    private void Awake()
    {
    }

    public IEnumerator GetScreenResolution()
    {
        yield return new WaitForEndOfFrame();

        ScreenWidth = gameObject.GetComponent<RectTransform>().rect.width;
        ScreenHeight = gameObject.GetComponent<RectTransform>().rect.height;

        Debug.Log(AspectRatioManager.ScreenWidth + ", " + AspectRatioManager.ScreenHeight);

        if(!Stopped)
            AspectRatioSet = true;
        else
            AspectRatioSet = false;

        yield break;
    }

}
