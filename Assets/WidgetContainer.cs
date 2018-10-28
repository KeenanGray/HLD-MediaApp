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
    private void Awake()
    {
    }

    void Update()
    {
        Debug.Log("Initializing Widgets");
        MainCanvas = GameObject.FindWithTag("MainCanvas");

        if (MainCanvas == null)
        {
            Debug.LogWarning("Canvas tagged with \"Main Canvas\" Not Found");
        }

        width = MainCanvas.GetComponent<RectTransform>().rect.width;
        height = MainCanvas.GetComponent<RectTransform>().rect.height;


        rt = GetComponent<RectTransform>();
        if (rt == null)
        {
            Debug.LogWarning("difficulty finding rect transform attached to this gameobject");
        }

        rt.anchoredPosition = new Vector3(0, 0, 0);
        rt.sizeDelta = new Vector2(width, height);
    }
}