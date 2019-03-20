using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI_Builder;
using UnityEngine;
using UnityEngine.UI;

public class BluetoothAudioSource : MonoBehaviour
{
    private bool hasCaptions;
    TextAsset AudioCaptions;

    public float knownRSSI;
    int startCount;
    int endCount;
    int unknownCount;

    AudioSource src;

    TextMeshProUGUI captionsCanvas;

    public void Init()
    {
        src = GetComponent<AudioSource>();
        AudioCaptions = new TextAsset("");
        captionsCanvas = GetComponentInChildren<TextMeshProUGUI>();

        startCount = 0;
        endCount = 0;
        unknownCount = 0;

        if (hasCaptions == false)
        {

        }
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
            //Debug.Log("playing clip " + src.clip.name);
            src.Play();
        }
        else
        {
            //Debug.Log("unpausing clip " + src.clip.name);
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

    public void SetPhoto(string PathToAudio, string bundleString)
    {
        var image = GetComponentInChildren<Image>();

        AssetBundle tmp = null;
        foreach (AssetBundle b in AssetBundle.GetAllLoadedAssetBundles())
        {
            if (b.name == bundleString)
                tmp = b;
        }

        if (tmp != null && src != null)
        {
            image.sprite = tmp.LoadAsset<Sprite>(PathToAudio) as Sprite;
            //src.time = 0;
        }
    }

    private void OnDisable()
    {
        if (src != null)
        {
            src.Pause();
            paused = true;
        }
    }

    private void OnEnable()
    {
        if (src != null)
        {
            paused = false;
            src.Play();
        }
    }

    int newStart;
    int iterator;
    bool wait;
    private bool paused;

    public IEnumerator PlayCaptionsWithAudio()
    {
        wait = true;
        newStart = 0;
        iterator = 0;

        var words = GetNumberOfLines();
        int WordsPerLine = 9;

        if (AudioCaptions == null)
        {
            Debug.Log("No captions");
            yield break;
        }

        //set up video captions
        TextMeshProUGUI tmp = captionsCanvas;
        if (tmp == null)
            Debug.LogWarning("couldnt find text");

        while (true)
        {
            if (paused)
            {
                yield return null;
            }
            string line = "";

            var TimePerLine = (src.clip.length - 2)
                    /
                (words.Length / WordsPerLine);
                    
            int word = (int)(words.Length * (src.time / src.clip.length));

            int start = 0;

            if (newStart < word)
            {
                start = word;
                wait = true;
            }
            else
            {
                start = newStart;
            }

            line = "";
            for (iterator = start; iterator < start + WordsPerLine; iterator++)
            {
                if (iterator < words.Length)
                    line += words[iterator] + " ";
                else
                    break;

                //  start += WordsPerLine;
                tmp.text = line;

                newStart = iterator;
            }
            iterator++;

            //break if skipped
            if (wait)
            {
//                Debug.Log("wait");
                yield return new WaitForSeconds(TimePerLine);
            }
            else
            {
                Debug.Log("no wait");

                yield return null;
            }
            wait = true;

            yield return null;
        }
    }

    string[] GetNumberOfLines()
    {
        if (AudioCaptions == null)
            return null;

        //Count all the words
        var words = AudioCaptions.text.Split(' ');
        return words;
    }

    public void SetCaptionsStart()
    {
        wait = false;
        newStart = (int)(GetNumberOfLines().Length * (src.time / src.clip.length));
    }

    public void SetAudioCaptions(string name, string filePath)
    {
        var newText = UIB_FileManager.ReadTextAssetBundle(name, filePath);

        if (newText == null)
        {
            Debug.LogWarning("Null Text Given for Captions");
            captionsCanvas.enabled = false;
            hasCaptions = false;

            return;
        }
        hasCaptions = true;
        AudioCaptions = new TextAsset(newText);
    }

    internal void IncrementStart()
    {
        startCount++;
    }

    internal int getStartCount(bool reset = false)
    {
        var tmp = startCount;
        if (reset)
        {
            startCount = 0;
        }
        return tmp;
    }

    internal void IncrementEnd()
    {
        endCount++;
    }

    internal int getEndCount(bool reset = false)
    {
        var tmp = endCount;
        if (reset)
        {
            endCount = 0;
        }
        return tmp;
    }

    internal void IncrementUnknown()
    {
        unknownCount++;
    }

    internal int getUnknownCount(bool reset = false)
    {
        var tmp = unknownCount;
        if (reset)
        {
            unknownCount = 0;
        }
        return tmp;
    }
}
