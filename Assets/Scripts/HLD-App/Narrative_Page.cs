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
        Debug.Log("Last Page " + UIB_PageManager.LastPage);

        Debug.Log("Deactivating UAP on list");
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

        if (UIB_PageManager.LastPage.name == "DisplayedNarrativesList_Page")
        {
            GameObject.Find("DisplayedNarrativesFR_Page").GetComponent<Canvas>().enabled = false;
        }

        AudioPlayerScreen.GetComponent<UIB_AudioPlayer>().SetTitle(title);
        AudioPlayerScreen.GetComponent<UIB_AudioPlayer>().SetImage(photoPath);

        var captionFile = "DisplayedNarratives/Captions/" + title.Replace(" ", "_");
        TextAsset captions = Resources.Load<TextAsset>(captionFile) as TextAsset;
        AudioPlayerScreen.GetComponent<UIB_AudioPlayer>().SetAudioCaptions(captions);

        AudioPlayerScreen.GetComponent<AspectRatioFitter>().enabled = true;
        AudioPlayerScreen.GetComponent<UIB_AudioPlayer>().SetAudio("DisplayedNarratives/Audio/" + title.Split(' ')[0]);//we only use first names for audio titles

        backButton.name = UIB_PageManager.LastPage.name.Split('_')[0] + "_Button";
        AudioPlayerScreen.transform.SetParent(transform);
        AudioPlayerScreen.transform.SetSiblingIndex(2);

        AudioPlayerScreen.GetComponent<AspectRatioFitter>().enabled = false;
        AudioPlayerScreen.GetComponent<AspectRatioFitter>().enabled = true;


        AudioPlayerScreen.GetComponent<UIB_AudioPlayer>().tools.PlayMethod(1);
        AudioPlayerScreen.GetComponent<Canvas>().enabled = true;
    }

    public void PageDeActivatedHandler()
    {
        ReturnToCameraView = false;
        if (AudioPlayerScreen != null)
        {
            var thing = AudioPlayerScreen.GetComponent<UIB_AudioPlayer>().tools;
            //   Resources.UnloadUnusedAssets();
            if (thing != null)
            {
                thing.PlayMethod(2);
            }
            AudioPlayerScreen.GetComponent<Canvas>().enabled = false;
        }

        if (ReturnToCameraView)
        {
            Debug.Log("HERE");
            GameObject.Find("DisplayedNarrativesFR_Page").GetComponent<Canvas>().enabled = true;
            GameObject.Find("CameraViewTexture").GetComponent<FaceDetectionHLD>().BeginRecognizer();
            //    UIB_PageManager.CurrentPage = GameObject.Find("DisplayedNarrativesFR_Page");

        }
        else
        {
            GameObject.Find("DisplayedNarrativesFR_Page").GetComponent<Canvas>().enabled = false;
            UIB_PageManager.LastPage = GameObject.Find("DisplayedNarrativesFR_Page");
            //      UIB_PageManager.CurrentPage = GameObject.Find("DisplayedNarrativesList_Page");
        }
        // AudioPlayerScreen.GetComponent<UIB_Page>().StartCoroutine("MoveScreenOut", false);
    }


}
