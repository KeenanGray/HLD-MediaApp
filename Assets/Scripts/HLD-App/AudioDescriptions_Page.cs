﻿using System;
using System.Collections;
using System.Collections.Generic;
using UI_Builder;
using UnityEngine;
using UnityEngine.UI;

public class AudioDescriptions_Page : MonoBehaviour, UIB_IPage
{
    GameObject AudioPlayerScreen;

    public void Init()
    {
        GetComponent<UIB_Page>().OnActivated += PageActivatedHandler;
        GetComponent<UIB_Page>().OnDeActivated += PageDeActivatedHandler;

//        UIB_AssetBundleHelper.InsertAssetBundle("hld/displayed/audio");
        AudioPlayerScreen = GameObject.Find("AudioPlayer_Page");
    }

    internal void SetupAudioPlayer()
    {
        var audioPlayer = AudioPlayerScreen.GetComponent<UIB_AudioPlayer>();
        AudioPlayerScreen.GetComponent<AspectRatioFitter>().enabled = true;
        audioPlayer.SetTitle("Audio Descriptions");
        audioPlayer.SetImageFromResource("BackGroundPhotos/AudioDescriptions",1000);
        audioPlayer.fileType = ".mp3";
        audioPlayer.SetAudio("Audio_Captions", "hld/displayed/audio");
        //TODO: null captions for now
        audioPlayer.SetAudioCaptions("captions", null);
        //audioPlayer.SetAudioCaptions("captions", "hld/displayed/audio");
        audioPlayer.Init();
    }

    public void PageActivatedHandler()
    {
        SetupAudioPlayer();
        AudioPlayerScreen.transform.SetParent(transform);
        AudioPlayerScreen.transform.SetSiblingIndex(transform.childCount-2);
        AudioPlayerScreen.GetComponent<Canvas>().enabled = true;

        try
        {
            GetComponentInChildren<UIB_AudioPlayerTools>().LoadtimeCodeToPrefs();
        }
        catch (Exception e)
        {
            if (e.GetType() == typeof(NullReferenceException))
            {

            }
        }
    }

    public void PageDeActivatedHandler()
    {
        try
        {
            GetComponentInChildren<UIB_AudioPlayerTools>().SavetimeCodeToPrefs();
        }
        catch (Exception e)
        {
            if(e.GetType() == typeof(NullReferenceException))
            {

            }
        }

        var thing = AudioPlayerScreen.GetComponent<UIB_AudioPlayer>().Tools;
        if (thing != null)
        {
            thing.PlayMethod(2);
        }
        else
        {
        }
    }

}
