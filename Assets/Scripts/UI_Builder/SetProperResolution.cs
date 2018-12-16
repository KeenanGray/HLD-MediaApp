using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetProperResolution : MonoBehaviour {

    //This script exists to 'Tag' objects as parts of app_pages

    float width;
    float height;
    RectTransform rt;

    // Use this for initialization
    public void SetAspectRatio() {
        width = AspectRatioManager.ScreenWidth;
        height = AspectRatioManager.ScreenHeight;

        rt = gameObject.GetComponent<RectTransform>();
        if (rt == null)
            Debug.LogWarning("No rect transform on this component");

        rt.sizeDelta = new Vector2(width, height);
    }
}
