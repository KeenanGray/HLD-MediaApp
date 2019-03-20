using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
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

        RectTransform rt = null;
        try
        {
            rt = GetComponentInChildren<HorizontalLayoutGroup>().GetComponent<RectTransform>();
        }
        catch (Exception e)
        {
            if (rt == null)
                Debug.LogWarning(e);

            return;
        }

        var cellX = rt.rect.width;
        var cellY = rt.rect.height;

        var cellsize = new Vector2(cellX, cellY);
        grg.cellSize = cellsize;
    }
}
