using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WidgetContainer : MonoBehaviour
{
    float width;
    float height;

    RectTransform rt;

    public void SetAspectRatio(){
        width = AspectRatioManager.ScreenWidth;
        height = AspectRatioManager.ScreenHeight;

        rt = gameObject.GetComponent<RectTransform>();
        if (rt == null)
            Debug.LogWarning("No rect transform on this component");

        rt.sizeDelta = new Vector2(width, height);
    }


    public IEnumerator Init()
    {
        width = AspectRatioManager.ScreenWidth;
        height = AspectRatioManager.ScreenHeight;

        rt = gameObject.GetComponent<RectTransform>();
        if (rt == null)
            Debug.LogWarning("No rect transform on this component");

        rt.anchoredPosition = new Vector2(0, 0);
        rt.sizeDelta = new Vector2(width, height);
        yield break;
    }

    void Update()
    {
     
    }
}