using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitializationManager : MonoBehaviour {

    GameObject aspectManager;

    // Update is called once per frame
    void Start () {
        aspectManager = GameObject.Find("AppCanvas");
        StartCoroutine("Init");
    }

    private void Update()
    {
    
    }

    IEnumerator Init()
    {
        if(aspectManager==null){
            Debug.Log("warn");
            yield break;
        }
        yield return aspectManager.GetComponent<AspectRatioManager>().GetScreenResolution();
        Debug.Log("here");
        
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
            }

        foreach (SubMenu sm in GetComponentsInChildren<SubMenu>())
            {
                sm.Init();
            }

        foreach (AspectRatioFitter arf in GetComponentsInChildren<AspectRatioFitter>())
            {
                arf.aspectRatio = (AspectRatioManager.ScreenWidth) / (AspectRatioManager.ScreenHeight);
            }

        AspectRatioManager.Stopped = true;
        yield break;

    }
}

