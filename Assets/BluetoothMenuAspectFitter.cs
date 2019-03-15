using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI_Builder;
using TMPro;

public class BluetoothMenuAspectFitter : MonoBehaviour
{
    GameObject img;
    GameObject text;
    // Start is called before the first frame update
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!gameObject.activeSelf)
            return;

        img = transform.GetChild(0).Find("Image").gameObject;
        text = transform.GetChild(0).Find("Text").gameObject;

        var childCount = 0;
        try
        {
            childCount = transform.parent.childCount;
        }
        catch
        {
            return;
        }

        var boxSizeAdjust = 1;
        var textScaleAdjust = 1;

        if (childCount == 0)
        {
            return;
        }
        else if (childCount == 1)
        {
            boxSizeAdjust = 3;
            textScaleAdjust = 3;
        }
        else if (childCount > 1 && childCount <= 3)
        {
            boxSizeAdjust = 3;
            textScaleAdjust = 2;
        }
        else if (childCount > 3 && childCount <= 4)
        {
            boxSizeAdjust = 4;
            textScaleAdjust = 2;
        }
        else
        {
            boxSizeAdjust = 5;
            textScaleAdjust = 2;
        }
        var width = transform.parent.parent.GetComponent<RectTransform>().rect.width / boxSizeAdjust;
        img.GetComponent<RectTransform>().sizeDelta = new Vector2(width, width);

        //        Debug.Log(UIB_AspectRatioManager_Editor.ScreenWidth + ":" + width);
        text.GetComponent<RectTransform>().sizeDelta = new Vector2 (UIB_AspectRatioManager.ScreenWidth, width);
        text.GetComponent<TextMeshProUGUI>().fontSize = 50 * textScaleAdjust;
    }
}
