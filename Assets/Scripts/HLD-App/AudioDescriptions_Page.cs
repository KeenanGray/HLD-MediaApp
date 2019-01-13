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
        AudioPlayerScreen.GetComponent<UIB_AudioPlayer>().SetImage(null);
        AudioPlayerScreen.GetComponent<UIB_AudioPlayer>().SetAudio("Audio/AudioDescriptions/Displayed_AudioDescriptions");
    }

    public void PageActivatedHandler()
    {
        SetupAudioPlayer();
        AudioPlayerScreen.transform.SetParent(transform);
        AudioPlayerScreen.GetComponent<Canvas>().enabled = true;
    }

    public void PageDeActivatedHandler()
    {
        var thing = AudioPlayerScreen.GetComponent<UIB_AudioPlayer>().myTools.source;
        //   Resources.UnloadUnusedAssets();
        if (thing != null)
            thing.Stop();
    }

}
