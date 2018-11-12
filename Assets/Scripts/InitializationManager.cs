using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InitializationManager : MonoBehaviour
{
    GameObject aspectManager;

    public UAP_AccessibilityManager AccessibilityManager;
    GameObject VideoCanvas;

    public float InitializeTime;
    float t1;
    float t2;

    void Start()
    {
        aspectManager = GameObject.FindGameObjectWithTag("MainCanvas");
        VideoCanvas = GameObject.Find("Front");
        StartCoroutine("Init");
    }

    private void Update()
    {
        /*
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
        */
    }

    IEnumerator Init()
    {
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
        Canvas.ForceUpdateCanvases();
        
        foreach (AspectRatioFitter arf in GetComponentsInChildren<AspectRatioFitter>())
        {
            arf.aspectRatio = (AspectRatioManager.ScreenWidth) / (AspectRatioManager.ScreenHeight);
            arf.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            arf.enabled = true;
        }

      //  VideoCanvas.GetComponent<AspectRatioFitter>().enabled = true;

        foreach (App_Button ab in GetComponentsInChildren<App_Button>())
        {
            ab.Init();
        }
        foreach (Page p in GetComponentsInChildren<Page>())
        {
            p.Init();
            yield return p.MoveScreenOut();
        }
    
        foreach (SubMenu sm in GetComponentsInChildren<SubMenu>())
        {
            sm.Init();
            yield return sm.MoveScreenOut();
            sm.SetOnScreen(false);
        }

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Hidden")) {
            go.SetActive(false);
        }

        var firstScreen = GameObject.Find("LandingScreen");
        firstScreen.GetComponent<Page>().StartCoroutine("MoveScreenIn");

        AspectRatioManager.Stopped = true;

        t2 = Time.time;
        Debug.Log("boot time " + t2);
        if (t2 < InitializeTime) ;
        yield return new WaitForSeconds(InitializeTime - t2);

        VideoCanvas.SetActive(false);

        AccessibilityManager.enabled = true;

        t1 = Time.time;
        Debug.Log("Total time " + t1);
        yield break;
    }
}

