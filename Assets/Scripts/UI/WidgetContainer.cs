using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class WidgetContainer : MonoBehaviour
{
    GameObject MainCanvas;
    float width;
    float height;

    RectTransform rt;

    public void Init()
    {
        MainCanvas = GameObject.FindWithTag("MainCanvas");

        if (MainCanvas == null)
        {
            Debug.LogWarning("Canvas tagged with \"Main Canvas\" Not Found");
        }

        rt = gameObject.GetComponent<RectTransform>();
        if(rt==null)
            Debug.LogWarning("No rect transform on this component");

        width = AspectRatioManager.ScreenWidth;
        height = AspectRatioManager.ScreenHeight;

     //   Debug.Log("width " + width + " ,  " + height);

        rt.anchoredPosition = new Vector3(0, 0, 0);
        rt.sizeDelta = new Vector2(width, height);
    }

    void Update()
    {
     
    }
}