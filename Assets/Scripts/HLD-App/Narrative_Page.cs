using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI_Builder;
using UnityEngine;
using UnityEngine.UI;

public class Narrative_Page : MonoBehaviour, UIB_IPage
{
    GameObject backButton;
    GameObject AudioPlayerScreen;

    string title;
    string photoPath;

    public string ShowName { get; private set; }

    private void Start()
    {
    }

    public void Init()
    {
        AudioPlayerScreen = GameObject.Find("AudioPlayer_Page");

        title = "";
        GetComponent<UIB_Page>().OnActivated += PageActivatedHandler;
        GetComponent<UIB_Page>().OnDeActivated += PageDeActivatedHandler;

        if (ShowName == "" || ShowName == null)
        {
            //Debug.LogWarning("initializing narrative page without show");
            return;
        }

        backButton = transform.GetChild(0).Find(ShowName + "-NarrativesList_Button").gameObject;
        if (backButton == null)
            Debug.LogWarning("Bad error checking");
    }

    internal void SetupPage(string Title, string bgPhotoPath)
    {
        title = Title;
        photoPath = bgPhotoPath;
    }

    void Update()
    {
        //        Debug.Log("HEY " + ReturnToCameraView);
    }

    internal void SetShowName(string v)
    {
        ShowName = v;

        //get backbutton root
        transform.Find("BackButtonRoot").GetChild(0).name = ShowName + "-NarrativesList_Button";
        transform.Find("BackButtonRoot").GetChild(0).GetComponent<UIB_Button>().Init();


        //tell the list page not to mess with activation/deactivation for the time being.
        var listPage = GameObject.Find(ShowName + "-NarrativesList_Page");
    }

    //When the narrative page is activated, we want to set the back button to either the 
    //list of dancer's, or the camera face recognizer
    public void PageActivatedHandler()
    {
        UIB_AudioPlayer pageAudioPlayer = AudioPlayerScreen.GetComponent<UIB_AudioPlayer>();

        pageAudioPlayer.SetTitle(UIB_Utilities.SplitCamelCase(title.Replace("_", " ")));
        //        Debug.Log(photoPath + " " + "hld / "+ShowName.ToLower()+" / narratives / photos");
        pageAudioPlayer.SetImageAssetBundle(photoPath, "hld/" + ShowName.ToLower() + "/narratives/photos");
        var captionFile = title.Replace(" ", "_").ToLower();
        pageAudioPlayer.SetAudioCaptions(captionFile, "hld/" + ShowName.ToLower() + "/narratives/captions");
        pageAudioPlayer.enabled = true;
        pageAudioPlayer.fileType = ".wav";
        //TODO::this should use filemanger to check that file exists

#if UNITY_ANDROID
        pageAudioPlayer.SetAudio(title.Replace(" ", "_"), "hld/" + ShowName.ToLower() + "/narratives/audio");
#else
        pageAudioPlayer.SetAudio(title.Replace(" ", "_"), "hld/"+ShowName.ToLower()+"/narratives/audio");
#endif

        pageAudioPlayer.Tools.PlayMethod(1);
        pageAudioPlayer.Init();

        AudioPlayerScreen.transform.SetParent(transform);
        //    AudioPlayerScreen.transform.SetSiblingIndex(transform.childCount-1);
        AudioPlayerScreen.transform.SetAsFirstSibling();

        AudioPlayerScreen.GetComponent<AspectRatioFitter>().enabled = false;
        AudioPlayerScreen.GetComponent<AspectRatioFitter>().enabled = true;
        AudioPlayerScreen.GetComponent<Canvas>().enabled = true;

        // backButton.name = UIB_PageManager.LastPage.name.Split('_')[0] + "_Button";

        StartCoroutine(GetComponent<UIB_Page>().ResetUAP(true));
    }

    public void PageDeActivatedHandler()
    {
        if (AudioPlayerScreen != null)
        {
            var thing = AudioPlayerScreen.GetComponent<UIB_AudioPlayer>().Tools;
            if (thing != null)
            {
                thing.PlayMethod(2);
            }
            AudioPlayerScreen.GetComponent<Canvas>().enabled = false;
            AudioPlayerScreen.transform.SetParent(GameObject.Find("Pages").transform);
        }

        try
        {
            GameObject.Find(ShowName + "-NarrativesBT_Page").GetComponent<DisplayedNarrativesBluetooth_Page>().StopBTAudio();
        }
        catch (Exception e)
        {
            if (e.GetType() == typeof(NullReferenceException))
            {

            }
        }

    }


}
