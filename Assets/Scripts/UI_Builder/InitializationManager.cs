using System.Collections;
using System.Collections.Generic;
using UI_Builder;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InitializationManager : MonoBehaviour
{
    GameObject aspectManager;

    GameObject VideoCanvas;

    public float InitializeTime;
    float t1;
    float t2;

    void Start()
    {
#if UNITY_EDITOR
        AspectRatioManager_Editor.Instance().IsInEditor = false;
#endif
        aspectManager = GameObject.FindGameObjectWithTag("MainCanvas");
        VideoCanvas = GameObject.Find("Front");
        StartCoroutine("Init");
    }

    private void Update()
    {
    }

    IEnumerator Init()
    {

        //on android we must check for OBB file

        if (GooglePlayDownloader.RunningOnAndroid())
        {
            Debug.Log("Running on Android");
        }

        UAP_AccessibilityManager.PauseAccessibility(true);

        InitializePlaceHolderJsonFiles();


        if (aspectManager == null)
        {
            Debug.LogWarning("Unable to find Aspect Manager");
            yield break;
        }

        yield return aspectManager.GetComponent<AspectRatioManager>().GetScreenResolution();
        
        ObjPoolManager.Init();

        t1 = Time.time;

        foreach (AspectRatioFitter arf in GetComponentsInChildren<AspectRatioFitter>())
        {
            arf.aspectRatio = (AspectRatioManager.ScreenWidth) / (AspectRatioManager.ScreenHeight);
            arf.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            arf.enabled = true;
        }

        yield return GameObject.Find("DB_Manager").GetComponent<MongoLib>().UpdateFromDatabase();

        var str = MongoLib.ReadJson("Bios.json");
        Bio_Factory.CreateBioPages(str);
        Canvas.ForceUpdateCanvases();
        
        VideoCanvas.SetActive(true);

        foreach (UI_Builder.UIB_Button ab in GetComponentsInChildren<UI_Builder.UIB_Button>())
        {
            ab.Init();
        }
        foreach (UIB_Page p in GetComponentsInChildren<UIB_Page>())
        {
            p.Init();
            yield return p.MoveScreenOut();
        }
  
 //       foreach (GameObject go in GameObject.FindGameObjectsWithTag("Hidden")) {
 //           go.SetActive(false);
 //       }

        var firstScreen = GameObject.Find("Landing_Page");
        firstScreen.GetComponent<UIB_Page>().StartCoroutine("MoveScreenIn");

        AspectRatioManager.Stopped = true;

        //if we finish initializing faster than expected, take a moment to finish the video
        t2 = Time.time;
        var elapsed = t2 - t1;
        if (InitializeTime > elapsed)
            yield return new WaitForSeconds(InitializeTime - elapsed);
        else
            Debug.LogWarning("Took longer to initialize than expected");

        VideoCanvas.SetActive(false);

        UAP_AccessibilityManager.PauseAccessibility(false);
        var first = GameObject.Find("DISPLAYED-Code_Button");

        UAP_AccessibilityManager.SelectElement(first,true); ;

        //        Debug.Log("Init Time " + Time.time);

        yield break;
    }

    void InitializePlaceHolderJsonFiles()
    {
        Debug.Log(Application.persistentDataPath);

        TextAsset code = Resources.Load<TextAsset>("Json/AccessCode_default");
        TextAsset bios = Resources.Load<TextAsset>("Json/Bios_default");

        MongoLib.WriteJsonUnModified(bios.text,"Bios.json");
        MongoLib.WriteJsonUnModified(code.text, "AccessCode.json");

    }

}

