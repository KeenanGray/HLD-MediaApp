using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class InitializationManager : MonoBehaviour
{

    GameObject aspectManager;

    public UAP_AccessibilityManager AccessibilityManager;
    public GameObject bCanvas;

    void Start()
    {
        bCanvas.SetActive(true);
        aspectManager = GameObject.Find("AppCanvas");

        StartCoroutine("Init");
    }

    private void Update()
    {
        if (AspectRatioManager.ScreenHeight > 0 && AspectRatioManager.ScreenWidth > 0)
        {
            foreach (WidgetContainer wc in GetComponentsInChildren<WidgetContainer>())
            {
                wc.SetAspectRatio();
            }
            foreach (AspectRatioFitter arf in GetComponentsInChildren<AspectRatioFitter>())
            {
                arf.aspectRatio = (AspectRatioManager.ScreenWidth) / (AspectRatioManager.ScreenHeight);
            }
            foreach (SetProperResolution spr in GetComponentsInChildren<SetProperResolution>())
            {
                spr.SetAspectRatio();
            }

            AspectRatioManager.Stopped = true;
        }
    }

    IEnumerator Init()
    {
        bCanvas.SetActive(true);
        AccessibilityManager.enabled = false;

        if (aspectManager == null)
        {
            Debug.LogWarning("Unable to find Aspect Manager");
            yield break;
        }
        yield return aspectManager.GetComponent<AspectRatioManager>().GetScreenResolution();

        ObjPoolManager.Init();
        yield return GameObject.Find("DB_Manager").GetComponent<MongoLib>().UpdateFromDatabase();

        var str = MongoLib.ReadJson("Bios.json");
        Bio_Factory.CreateBioPages(str);

        foreach (WidgetContainer wc in GetComponentsInChildren<WidgetContainer>())
        {
            yield return wc.Init();
        }

        foreach (App_Button ab in GetComponentsInChildren<App_Button>())
        {
            ab.Init();
        }
        foreach (Page p in GetComponentsInChildren<Page>())
        {
            p.Init();
            yield return p.MoveScreenOut();
            p.ToggleRenderer(false);
        }
    
        foreach (SubMenu sm in GetComponentsInChildren<SubMenu>())
        {
            sm.Init();
            yield return sm.MoveScreenOut();
            sm.ToggleRenderer(true);
        }

        foreach (AspectRatioFitter arf in GetComponentsInChildren<AspectRatioFitter>())
        {
            arf.aspectRatio = (AspectRatioManager.ScreenWidth) / (AspectRatioManager.ScreenHeight);
        }
        AspectRatioManager.Stopped = true;

        AccessibilityManager.enabled = true;
        bCanvas.SetActive(false);

        yield break;
    }
}

