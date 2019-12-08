using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[ExecuteInEditMode]
public class UpdateNameOfTextItem : MonoBehaviour
{
#if UNITY_EDITOR

    TextMeshProUGUI text;
    public static bool ShouldRun = true;

    // Start is called before the first frame update
    void Update()
    {
        if (!ShouldRun)
            return;

        if (text == null)
            text = GetComponentInChildren<TextMeshProUGUI>();

        if (text != null)
        {
            var length = Mathf.Max(0, Mathf.Min(text.text.Length, 15));

            if (name != text.text.Substring(0, length))
            {
                name = text.text.Substring(0, length);
            }
        }
    }
#endif

}

