using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[ExecuteInEditMode]
public class AspectRatioManager : MonoBehaviour {
    GameObject MainCanvas;

    float width;
    float height;

    private void Awake()
    {

    }

    // Use this for initialization
    void Update () {
        Debug.Log("Initializing Aspect Ratio");

        MainCanvas = GameObject.FindWithTag("MainCanvas");

        if (MainCanvas == null)
        {
            Debug.LogWarning("Canvas tagged with \"Main Canvas\" Not Found");
        }

        width = MainCanvas.GetComponent<RectTransform>().rect.width;
        height = MainCanvas.GetComponent<RectTransform>().rect.height;

        foreach (AspectRatioFitter arf in GetComponents<AspectRatioFitter>()){
            arf.aspectRatio = width / height;
        }
        
	}
	
}
