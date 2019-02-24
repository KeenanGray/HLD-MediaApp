using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BluetoothAudioSource : MonoBehaviour
{

    AudioSource src;

    public void Init()
    {
        src = GetComponent<AudioSource>();
    }

    public void SetAudio(string PathToAudio, string bundleString)
    {
        src = GetComponent<AudioSource>();

        AssetBundle tmp = null;
        foreach (AssetBundle b in AssetBundle.GetAllLoadedAssetBundles())
        {
            if (b.name == bundleString)
                tmp = b;
        }

        if (tmp != null && src != null)
        {
            src.clip = tmp.LoadAsset<AudioClip>(PathToAudio) as AudioClip;
            //src.time = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    internal void AudioStart()
    {
        if (src == null)
            return;

        if (src.clip == null)
        {
            Debug.Log("Could not find audio clip");
            return;
        }
        Debug.Log("readied clip " + src.clip.name);
        src.Play();
        src.Pause();
    }

    internal void Play()
    {
        if (src == null)
            return;

        if (src.clip == null)
        {
            Debug.Log("Could not find audio clip");
            return;
        }


        if (!src.isPlaying)
        {
            Debug.Log("playing clip " + src.clip.name);
            src.Play();
        }
        else
        {
            Debug.Log("unpausing clip " + src.clip.name);
            src.UnPause();
        }
    }

    internal void Stop()
    {
        src.Pause();
    }

    internal void setVolume(float strength)
    {
        src.volume = strength;
    }

}
