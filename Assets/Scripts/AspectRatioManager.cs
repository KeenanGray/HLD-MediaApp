using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//This script should be added to the main canvas of the app
//

[ExecuteInEditMode]
public class AspectRatioManager : MonoBehaviour {
    public static float ScreenWidth;
    public static float ScreenHeight;
    
    private void Awake()
    {
    }

    private void Update()
    {
            if(ScreenWidth.Equals(0))
              ScreenWidth = gameObject.GetComponent<RectTransform>().rect.width;
            if (ScreenHeight.Equals(0))
                ScreenHeight = gameObject.GetComponent<RectTransform>().rect.height;
      }

}
