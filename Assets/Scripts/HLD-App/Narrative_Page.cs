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

    bool ReturnToCameraView;

    string title;
    string photoPath;
    public void Init()
    {
        AudioPlayerScreen = GameObject.Find("AudioPlayer_Page");

        title = "";
        GetComponent<UIB_Page>().OnActivated += PageActivatedHandler;
        GetComponent<UIB_Page>().OnDeActivated += PageDeActivatedHandler;

        backButton = transform.Find("Interface").Find("DisplayedNarrativesFR_Button").gameObject;
        if (backButton == null)
            Debug.LogWarning("Bad error checking");

        ReturnToCameraView = false;
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
        tmp.GetComponent<UIB_Page>().DeActivateUAP();
        tmp = GameObject.Find("DisplayedNarrativesFR_Page");
        tmp.GetComponent<UIB_Page>().DeActivateUAP();

        var camefrom = UIB_PageManager.LastPage.name;

        if (camefrom == "DisplayedNarrativesFR_Page")
        {
            ReturnToCameraView = true;
        }
        else
        {
            ReturnToCameraView = false;
        }

        foreach(UIB_Button uibb in GameObject.Find("DisplayedNarrativesFR_Page").GetComponentsInChildren<UIB_Button>())
        {
            uibb.enabled = false;
            uibb.GetComponent<Button>().enabled = false;
        }
        foreach (UIB_Button uibb in GameObject.Find("DisplayedNarrativesList_Page").GetComponentsInChildren<UIB_Button>())
        {
            uibb.enabled = false;
            uibb.GetComponent<Button>().enabled = false;
        }

        if (UIB_PageManager.LastPage.name == "DisplayedNarrativesList_Page")
        {
            GameObject.Find("DisplayedNarrativesFR_Page").GetComponent<Canvas>().enabled = false;
        }

        UIB_AudioPlayer pageAudioPlayer = AudioPlayerScreen.GetComponent<UIB_AudioPlayer> ();

        pageAudioPlayer.SetTitle(title);
        pageAudioPlayer.SetImageFromFile(Application.persistentDataPath+ photoPath);
        var captionFile = "/hld-displayed/Captions/" + title.Replace(" ", "_")+".txt";
        pageAudioPlayer.SetAudioCaptions(captionFile);
        pageAudioPlayer.enabled = true;
        pageAudioPlayer.fileType = ".wav";
        //TODO::this should use filemanger to check that file exists
        pageAudioPlayer.SetAudio("file://" + Application.persistentDataPath + "/hld-displayed/Audio/"+ title.Replace(" ","_") + ".wav");
        pageAudioPlayer.Tools.PlayMethod(1);
        pageAudioPlayer.Init();

        AudioPlayerScreen.transform.SetParent(transform);
        AudioPlayerScreen.transform.SetSiblingIndex(transform.childCount-1);

        AudioPlayerScreen.GetComponent<AspectRatioFitter>().enabled = false;
        AudioPlayerScreen.GetComponent<AspectRatioFitter>().enabled = true;
        AudioPlayerScreen.GetComponent<Canvas>().enabled = true;

        backButton.name = UIB_PageManager.LastPage.name.Split('_')[0] + "_Button";

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

        if (ReturnToCameraView)
        {
            GameObject.Find("DisplayedNarrativesFR_Page").GetComponent<Canvas>().enabled = true;
        }
        else
        {
            GameObject.Find("DisplayedNarrativesFR_Page").GetComponent<Canvas>().enabled = false;
            UIB_PageManager.LastPage = GameObject.Find("DisplayedNarrativesList_Page");
        }
    }


}
