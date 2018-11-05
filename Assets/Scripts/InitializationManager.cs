﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class InitializationManager : MonoBehaviour {

    public bool Initialize;

    // Update is called once per frame
    void Awake () {
    }

    private void Update()
    {
        if (!AspectRatioManager.ScreenWidth.Equals(0) && !AspectRatioManager.ScreenHeight.Equals(0))
        {
            if (AspectRatioManager.AspectRatioSet)
            {
                foreach (WidgetContainer wc in GetComponentsInChildren<WidgetContainer>())
                {
                    wc.Init();
                }

                foreach (App_Button ab in GetComponentsInChildren<App_Button>())
                {
                    ab.Init();
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
                    arf.aspectRatio = (AspectRatioManager.ScreenWidth) / (AspectRatioManager.ScreenHeight);
                }

                ObjPoolManager.Init();

               GameObject.Find("DB_Manager").GetComponent<MongoLib>().UpdateFromDatabase();

                AspectRatioManager.Stopped = true;
            }
        }
        else
            Debug.LogWarning("Canvas size is invalid");
    }
}
