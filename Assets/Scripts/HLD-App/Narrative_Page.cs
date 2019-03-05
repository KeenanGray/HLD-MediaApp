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
    string TopPageName = "DisplayedNarrativesBT_Page";

    private void Start()
    {
    }

    public void Init()
    {
        AudioPlayerScreen = GameObject.Find("AudioPlayer_Page");

        title = "";
        GetComponent<UIB_Page>().OnActivated += PageActivatedHandler;
        GetComponent<UIB_Page>().OnDeActivated += PageDeActivatedHandler;

        backButton = transform.GetChild(0).Find("DisplayedNarrativesBT_Button").gameObject;
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

    //When the narrative page is activated, we want to set the back button to either the 
    //list of dancer's, or the camera face recognizer
    public void PageActivatedHandler()
    {
        var tmp = GameObject.Find("DisplayedNarrativesList_Page");
        tmp = GameObject.Find(TopPageName);

        UIB_AudioPlayer pageAudioPlayer = AudioPlayerScreen.GetComponent<UIB_AudioPlayer> ();

        pageAudioPlayer.SetTitle(title);
        pageAudioPlayer.SetImageAssetBundle(photoPath, "hld/displayed/narratives/photos");
        var captionFile = title.Replace(" ", "_").ToLower();
        pageAudioPlayer.SetAudioCaptions(captionFile, "hld/displayed/narratives/captions");
        pageAudioPlayer.enabled = true;
        pageAudioPlayer.fileType = ".wav";
        //TODO::this should use filemanger to check that file exists

#if UNITY_ANDROID
        pageAudioPlayer.SetAudio(title.Replace(" ", "_"), "hld/displayed/narratives/audio");
#else
        pageAudioPlayer.SetAudio(title.Replace(" ","_"), "hld/displayed/narratives/audio");
#endif

        pageAudioPlayer.Tools.PlayMethod(1);
        pageAudioPlayer.Init();

        AudioPlayerScreen.transform.SetParent(transform);
        //    AudioPlayerScreen.transform.SetSiblingIndex(transform.childCount-1);
        AudioPlayerScreen.transform.SetAsFirstSibling();

        AudioPlayerScreen.GetComponent<AspectRatioFitter>().enabled = false;
        AudioPlayerScreen.GetComponent<AspectRatioFitter>().enabled = true;
        AudioPlayerScreen.GetComponent<Canvas>().enabled = true;

        backButton.name = UIB_PageManager.LastPage.name.Split('_')[0] + "_Button";

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

        GameObject.Find("DisplayedNarrativesBT_Page").GetComponent<DisplayedNarrativesBluetooth_Page>().StopBTAudio();

    }


}
