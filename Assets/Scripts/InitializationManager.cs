using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class InitializationManager : MonoBehaviour {

    GameObject aspectManager;
    public GameObject mCamera;

    void Start () {
        mCamera.SetActive(false);
        aspectManager = GameObject.Find("AppCanvas");
        StartCoroutine("Init");
        
        // StartRequest is build like a normal 
        // coroutine and yield returns on WWW
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

            GameObject.Find("DB_Manager").GetComponent<MongoLib>().updateThis = true;

            AspectRatioManager.Stopped = true;
        }
    }

    IEnumerator Init()
    {
        if (aspectManager==null){
            Debug.LogWarning("Unable to find Aspect Manager");
            yield break;
        }
        yield return aspectManager.GetComponent<AspectRatioManager>().GetScreenResolution();

        ObjPoolManager.Init();
        yield return GameObject.Find("DB_Manager").GetComponent<MongoLib>().UpdateFromDatabase();
         
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
            p.StartCoroutine("MoveScreenOut");
        }

        foreach (SubMenu sm in GetComponentsInChildren<SubMenu>())
            {
                sm.Init();
            sm.StartCoroutine("MoveScreenOut");
            }

        foreach (AspectRatioFitter arf in GetComponentsInChildren<AspectRatioFitter>())
            {
                arf.aspectRatio = (AspectRatioManager.ScreenWidth) / (AspectRatioManager.ScreenHeight);
            }
        
        AspectRatioManager.Stopped = true;

        mCamera.SetActive(true);
        yield break;

    }
}

