using System.Collections;
using System.Collections.Generic;
using UI_Builder;
using UnityEngine;
using UnityEngine.UI;

public class AudioDescriptions_Page : MonoBehaviour,UIB_IPage {
    GameObject AudioPlayerScreen;

    public void Init()
    {
        GetComponent<UIB_Page>().OnActivated += PageActivatedHandler;
        GetComponent<UIB_Page>().OnDeActivated += PageDeActivatedHandler;

        AudioPlayerScreen = GameObject.Find("AudioPlayer_Page");

    }

    internal void SetupAudioPlayer()
    {
        AudioPlayerScreen.GetComponent<AspectRatioFitter>().enabled = true;
        AudioPlayerScreen.GetComponent<UIB_AudioPlayer>().SetTitle("Audio Descriptions");
        AudioPlayerScreen.GetComponent<UIB_AudioPlayer>().SetImage("BackGroundPhotos/AudioDescriptions");
        AudioPlayerScreen.GetComponent<UIB_AudioPlayer>().SetAudio("AudioDescriptions/Displayed_AudioDescriptions");
        AudioPlayerScreen.GetComponent<UIB_AudioPlayer>().SetAudioCaptions(null);
    }

    public void PageActivatedHandler()
    {
        SetupAudioPlayer();
        AudioPlayerScreen.transform.SetParent(transform);
        AudioPlayerScreen.transform.SetSiblingIndex(1);
        AudioPlayerScreen.GetComponent<Canvas>().enabled = true;
    }

    public void PageDeActivatedHandler()
    {
        var thing = AudioPlayerScreen.GetComponent<UIB_AudioPlayer>().tools;
        //   Resources.UnloadUnusedAssets();
        if (thing != null)
            thing.PlayMethod(2);
    }

}
