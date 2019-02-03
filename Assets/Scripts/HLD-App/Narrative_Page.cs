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

        AudioPlayerScreen.GetComponent<UIB_AudioPlayer>().SetTitle(title);
        AudioPlayerScreen.GetComponent<UIB_AudioPlayer>().SetImage(photoPath);

        var captionFile = "/hld-displayed/Captions/" + title.Replace(" ", "_")+".txt";
        AudioPlayerScreen.GetComponent<UIB_AudioPlayer>().SetAudioCaptions(captionFile);

        AudioPlayerScreen.GetComponent<AspectRatioFitter>().enabled = true;

        AudioPlayerScreen.GetComponent<UIB_AudioPlayer>().fileType = ".wav";
        //TODO::this should use filemanger to check that file exists
        AudioPlayerScreen.GetComponent<UIB_AudioPlayer>().SetAudio("file://" + Application.persistentDataPath + "/hld-displayed/Audio/"+ title.Split(' ')[0] + ".wav");//we only use first names for audio titles

        backButton.name = UIB_PageManager.LastPage.name.Split('_')[0] + "_Button";
        AudioPlayerScreen.transform.SetParent(transform);
        AudioPlayerScreen.transform.SetSiblingIndex(transform.childCount-1);

        AudioPlayerScreen.GetComponent<AspectRatioFitter>().enabled = false;
        AudioPlayerScreen.GetComponent<AspectRatioFitter>().enabled = true;

        AudioPlayerScreen.GetComponent<UIB_AudioPlayer>().Tools.PlayMethod(1);
        AudioPlayerScreen.GetComponent<Canvas>().enabled = true;

        AudioPlayerScreen.GetComponent<UIB_AudioPlayer>().Init();
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
            GameObject.Find("CameraViewTexture").GetComponent<HLD.FaceDetection>().BeginRecognizer();
        }
        else
        {
            GameObject.Find("DisplayedNarrativesFR_Page").GetComponent<Canvas>().enabled = false;
            UIB_PageManager.LastPage = GameObject.Find("DisplayedNarrativesList_Page");
            GameObject.Find("CameraViewTexture").GetComponent<HLD.FaceDetection>().EndRecognizer();
        }
    }


}
