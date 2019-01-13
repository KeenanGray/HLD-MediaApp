using System;
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
    GameObject AccessibilityInstructions;

    public float InitializeTime;
    float t1;
    float t2;

    void Start()
    {
#if UNITY_EDITOR
        UIB_AspectRatioManager_Editor.Instance().IsInEditor = false;
#endif
        aspectManager = GameObject.FindGameObjectWithTag("MainCanvas");
        VideoCanvas = GameObject.Find("Front");

        UAP_AccessibilityManager.RegisterOnTwoFingerSingleTapCallback(StopSpeech);
        StartCoroutine("Init");
    }

    private void StopSpeech()
    {
        if (UAP_AccessibilityManager.IsSpeaking())
        {
            UAP_AccessibilityManager.StopSpeaking();
        }
        Debug.Log("TWOFINGERSINGLE");
        UAP_AccessibilityManager.Say("", false, true, UAP_AudioQueue.EInterrupt.All);
    }

    private void Update()
    {
    }

    IEnumerator Init()
    {
        yield return new WaitForSeconds(1.0f);
        AccessibilityInstructions = GameObject.Find("AccessibleInstructions_Button");
        //enable accessible instructins if plugin is on
        if (UAP_AccessibilityManager.IsActive())
        {
            AccessibilityInstructions.SetActive(true);
        }
        else
            AccessibilityInstructions.SetActive(false);


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

        ObjPoolManager.Init();

        t1 = Time.time;

        foreach (AspectRatioFitter arf in GetComponentsInChildren<AspectRatioFitter>())
        {
            arf.aspectRatio = (UIB_AspectRatioManager.ScreenWidth) / (UIB_AspectRatioManager.ScreenHeight);
            arf.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            arf.enabled = true;
        }

        foreach(UIB_PageContainer PageContainer in GetComponentsInChildren < UIB_PageContainer>())
        {
            PageContainer.Init();
        }

        yield return GameObject.Find("DB_Manager").GetComponent<MongoLib>().UpdateFromDatabase();

        VideoCanvas.SetActive(true);

        foreach (UI_Builder.UIB_Button ab in GetComponentsInChildren<UI_Builder.UIB_Button>())
        {
            ab.Init();
        }

        foreach (UIB_IPage p in GetComponentsInChildren<UIB_IPage>())
        {

            p.Init();

        }

        foreach (UIB_Page p in GetComponentsInChildren<UIB_Page>())
        {
            //TODO:Fix this bad bad shit
            if (p.gameObject.name == "Landing_Page")
                yield return p.MoveScreenOut(true);
            else
            {
                p.StartCoroutine("MoveScreenOut", true);
            }
        }

        var firstScreen = GameObject.Find("Landing_Page");
        firstScreen.GetComponent<UIB_Page>().StartCoroutine("MoveScreenIn", true);

        //if we finish initializing faster than expected, take a moment to finish the video
        t2 = Time.time;
        var elapsed = t2 - t1;
        if (InitializeTime > elapsed)
            yield return new WaitForSeconds(InitializeTime - elapsed);
        else if (Mathf.Approximately(InitializeTime, float.Epsilon))
            Debug.Log("took " + elapsed + "s to initialize");
        else
            Debug.LogWarning("Took longer to initialize than expected");

        VideoCanvas.SetActive(false);

        UAP_AccessibilityManager.PauseAccessibility(false);
        var first = GameObject.Find("DISPLAYED-Code_Button");

        UAP_AccessibilityManager.SelectElement(first, true); ;

        //        Debug.Log("Init Time " + Time.time);

        yield break;
    }

    void InitializePlaceHolderJsonFiles()
    {
        Debug.Log(Application.persistentDataPath);

        TextAsset code = Resources.Load<TextAsset>("Json/AccessCode_default");
        TextAsset bios = Resources.Load<TextAsset>("Json/Bios_default");

        MongoLib.WriteJsonUnModified(bios.text, "Bios.json");
        MongoLib.WriteJsonUnModified(code.text, "AccessCode.json");

    }

}

