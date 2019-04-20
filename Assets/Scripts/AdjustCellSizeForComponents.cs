using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(VerticalLayoutGroup))]
[ExecuteInEditMode]
public class AdjustCellSizeForComponents : MonoBehaviour
{
    GridLayoutGroup grg;
    float og_FontSize = 1;
    // Start is called before the first frame update
    void Start()
    {
        grg = GetComponent<GridLayoutGroup>();
        if (transform.childCount > 0)
            og_FontSize = GetComponentInChildren<TextMeshProUGUI>().fontSize;
    }

    // Update is called once per frame
    void Update()
    {

        if (transform.childCount <= 0)
            return;

        RectTransform hrt = null;
        try
        {
            hrt = GetComponentInChildren<HorizontalLayoutGroup>().GetComponent<RectTransform>();
        }
        catch (Exception e)
        {
            if (hrt == null)
                Debug.LogWarning(e);

            return;
        }

        RectTransform vrt = null;
        try
        {
            vrt = GetComponentInChildren<VerticalLayoutGroup>().GetComponent<RectTransform>();
        }
        catch (Exception e)
        {
            if (vrt == null)
                Debug.LogWarning(e);

            return;
        }

        var cellX = hrt.rect.width;
        var cellY = hrt.rect.height + vrt.rect.height;

        var cellsize = new Vector2(cellX, cellY);
       // grg.cellSize = cellsize;
    }
}
