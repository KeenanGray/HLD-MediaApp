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

    //When the narrative page is activated, we want to set the back button to either the 
    //list of dancer's, or the camera face recognizer
    public void PageActivatedHandler()
    {
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
        AudioPlayerScreen.GetComponent<AspectRatioFitter>().enabled = true;
        AudioPlayerScreen.GetComponent<UIB_AudioPlayer>().SetAudio("Audio/Displayed_Narratives/" + title.Split(' ')[0]);//we only use first names for audio titles

        backButton.name = UIB_PageManager.LastPage.name.Split('_')[0] + "_Button";
        AudioPlayerScreen.transform.SetParent(transform);

        AudioPlayerScreen.GetComponent<UIB_AudioPlayer>().myTools.PlayButtonPressed();
        AudioPlayerScreen.GetComponent<Canvas>().enabled = true;
    }

    public void PageDeActivatedHandler()
    {
        if (AudioPlayerScreen != null)
        {
            var thing = AudioPlayerScreen.GetComponent<UIB_AudioPlayer>().myTools.source;
            //   Resources.UnloadUnusedAssets();
            if (thing != null)
            {
                thing.Stop();
            }
            AudioPlayerScreen.GetComponent<Canvas>().enabled = false;
        }

        if (ReturnToCameraView)
        {
            GameObject.Find("DisplayedNarrativesFR_Page").GetComponent<Canvas>().enabled = true;
       //    UIB_PageManager.CurrentPage = GameObject.Find("DisplayedNarrativesFR_Page");

        }
        else
        {
            GameObject.Find("DisplayedNarrativesFR_Page").GetComponent<Canvas>().enabled = false;
      //      UIB_PageManager.CurrentPage = GameObject.Find("DisplayedNarrativesList_Page");
        }
        // AudioPlayerScreen.GetComponent<UIB_Page>().StartCoroutine("MoveScreenOut", false);
    }


}
