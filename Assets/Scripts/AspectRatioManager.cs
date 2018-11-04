using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//This script should be added to the main canvas of the app
//

[ExecuteInEditMode]
public class AspectRatioManager : MonoBehaviour {
    GameObject MainCanvas;
    
    public static float ScreenWidth;
    public static float ScreenHeight;

    public float framecount;
    private void Awake()
    {
        framecount = 0;
    }

    private void Update()
    {
        if (framecount < 10)
        {
            framecount++;
            MainCanvas = gameObject;

            if (MainCanvas == null)
            {
                Debug.LogWarning("This object is not attached to the main canvas of the app");
            }

            ScreenWidth = gameObject.GetComponent<RectTransform>().rect.width;
            ScreenHeight = gameObject.GetComponent<RectTransform>().rect.height;
            
            foreach (WidgetContainer wc in GetComponentsInChildren<WidgetContainer>())
            {
                wc.Init();
            }

            foreach (Page p in GetComponentsInChildren<Page>())
            {
                p.Init();
            }

            foreach (SubMenu sm in GetComponentsInChildren<SubMenu>())
            {
                sm.Init();
            }

            foreach (AspectRatioFitter arf in GetComponentsInChildren<AspectRatioFitter>())
            {
                arf.aspectRatio = (ScreenWidth) / (ScreenHeight);
            }
        }
      }

}
