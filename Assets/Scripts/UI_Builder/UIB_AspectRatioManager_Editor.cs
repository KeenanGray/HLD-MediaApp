using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI_Builder
{
    //This script should be added to the main canvas of the app
    [ExecuteInEditMode]
    public class UIB_AspectRatioManager_Editor : MonoBehaviour
    {
#if UNITY_EDITOR
        public static float ScreenHeight;
        public static float ScreenWidth;

        public bool IsInEditor;

        static UIB_AspectRatioManager_Editor aspectRatioManager;

        private void Awake()
        {
            GetScreenResolution();

        }
        public static UIB_AspectRatioManager_Editor Instance()
        {
            if (aspectRatioManager == null)
            {
                aspectRatioManager = GameObject.FindWithTag("MainCanvas").GetComponent<UIB_AspectRatioManager_Editor>();
                return aspectRatioManager;
            }
            else
                return aspectRatioManager;
        }

        private void Update()
        {
            if (IsInEditor)
            {
                //            Debug.Log("Run");
                GetScreenResolution();
            }
        }

        public void GetScreenResolution()
        {
            ScreenWidth = Screen.width;
            ScreenHeight = Screen.height;

            float right = ScreenWidth * 2;
            float up = ScreenHeight / 2;

            int buffer = 100;
            int rowcount = 0;
            int rowTotal = 5;
            foreach (AspectRatioFitter arf in GetComponentsInChildren<AspectRatioFitter>())
            {

                arf.enabled = true;
                arf.aspectRatio = (ScreenWidth) / (ScreenHeight);
                arf.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
                arf.enabled = false;

                // var tmp = arf.GetComponent<RectTransform>().position;
                //if get component has a "page", move it into a nice position;
                if ((arf.GetComponent<UIB_Page>() != null) && arf.tag != "App_Biography" && arf.tag != "Pool")
                {
                    rowcount++;
                    if (rowcount > rowTotal)
                    {
                        rowcount = 0;
                        right = ScreenWidth * 2;
                        up -= (ScreenHeight + buffer);
                    }

                    var pos = new Vector3(right, up, 0);
                    right += ScreenWidth + buffer;
                    arf.GetComponent<RectTransform>().position = new Vector3((int)pos.x, (int)pos.y, (int)pos.z);
                }


                if (arf.tag == "Pool" || arf.tag == "App_Biography")
                {
                    var pos = new Vector3(-right, -up, 0);
                    arf.GetComponent<RectTransform>().position = new Vector3((int)pos.x, (int)pos.y, (int)pos.z);
                }




            }
        }
#endif
    }
}