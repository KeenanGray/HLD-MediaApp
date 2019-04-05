using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI_Builder;
using UnityEngine;
using UnityEngine.UI;

public class BluetoothAudioSource : MonoBehaviour
{

    public GameObject img;
    public GameObject text;
    public GameObject nameText;

    private bool hasCaptions;
    TextAsset AudioCaptions;

    public float knownRSSI;
    int startCount;
    int endCount;
    int unknownCount;
    bool clipEnded;
    bool captionsEnded;

    AudioSource src;

    TextMeshProUGUI captionsCanvas;

    public void Init()
    {
        try
        {
            src = GetComponent<AudioSource>();
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        }

        AudioCaptions = new TextAsset("");
        captionsCanvas = text.GetComponentInChildren<TextMeshProUGUI>();

        startCount = 0;
        endCount = 0;
        unknownCount = 0;

        if (hasCaptions == false)
        {

        }

        clipEnded = false;
        viewing = true;
    }

    private void Update()
    {
        if (src.clip == null)
            return;

        if (src.time.CompareTo(src.clip.length - 0.1f) < 0)
        {
        }
        else
        {
            Debug.Log("clip finished");
            StartCoroutine("DestroyAtEndOfFrame");
        }

        var strMap = 0.5f;
        if (src.volume < 1.0f)
        {
            strMap = HLD.Utilities.Map(src.volume, 0, 1, 0, 255);
        }
        else
        {
            strMap = 255;
        }

        Color32 clr = nameText.GetComponent<TextMeshProUGUI>().color;
        var iStrMap = (byte)strMap;

        nameText.GetComponent<TextMeshProUGUI>().color = new Color32(clr.r, clr.g, clr.b, iStrMap);
        captionsCanvas.GetComponent<TextMeshProUGUI>().color = new Color32(clr.r, clr.g, clr.b, iStrMap);
        img.GetComponent<Image>().color = new Color32(clr.r, clr.g, clr.b, iStrMap);
    }

    IEnumerator DestroyAtEndOfFrame()
    {
        yield return new WaitForEndOfFrame();
        Destroy(gameObject);

        yield break;
    }

    public void SetAudio(string PathToAudio, string bundleString)
    {
        if (PathToAudio == "" || PathToAudio == null)
            return;

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
            nameText.GetComponentInChildren<TextMeshProUGUI>().text = PathToAudio.Split('/')[PathToAudio.Split('/').Length - 1];
        }
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
        src.Play();
        src.Pause();

        StartCoroutine("CheckIsFinished");
    }

    internal void Stop()
    {
        src.Pause();
    }

    internal void setVolume(float strength)
    {
        var clr = nameText.GetComponent<TextMeshProUGUI>().color;
        /*
        var strMap = 1f;

        if (strength < 1.0f)
            strMap = HLD.Utilities.Map(strength, 0, 1, 0, 200);

        nameText.GetComponent<TextMeshProUGUI>().color = new Color(clr.r, clr.g, clr.b, strMap);
        captionsCanvas.GetComponent<TextMeshProUGUI>().color = new Color(clr.r, clr.g, clr.b, strMap);
        img.GetComponent<Image>().color = new Color(clr.r, clr.g, clr.b, strength);
        */
        src.volume = strength;
    }

    public void SetPhoto(string PathToAudio, string bundleString)
    {
        if (PathToAudio == null || PathToAudio == "")
            return;

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

    internal bool viewing;

    public IEnumerator PlayCaptionsWithAudio()
    {
 
        var words = GetNumberOfWords();
        int WordsPerLine = 12;

        var TimePerLine = (src.clip.length)
        /
    (words.Length / WordsPerLine);

        var framerate = 1.0f / Time.fixedDeltaTime;

        if (AudioCaptions == null)
        {
            Debug.Log("No captions");
            yield break;
        }

        //set up video captions
        TextMeshProUGUI tmp = captionsCanvas;
        if (tmp == null)
            Debug.LogWarning("couldnt find text");

        int word = 0;
        int start = 0;

        captionsEnded = false;

        while (true)
        {
            if (src == null)
            {
                yield break;
            }

            if (!src.isPlaying)
            {
                yield return null;
            }

            string line = "";

            word = (int)HLD.Utilities.Map(word + WordsPerLine, word + WordsPerLine, words.Length, src.time - .5f, src.clip.length);

            if (word < 0)
                yield return null;

            for (int i = start; i < start + WordsPerLine; i++)
            {
                if (i < words.Length)
                    line += words[i] + " ";

                if (i >= words.Length)
                {
                    captionsEnded = true;
                }
            }

            captionsCanvas.text = line;

            if (src.isPlaying)
            {
                yield return new WaitForSeconds(TimePerLine);
                start = start + WordsPerLine;

            }
            if (clipEnded && captionsEnded)
            {
                GetComponentInParent<DisplayedNarrativesBluetooth_Page>().StopPlaying(this);
                yield break;
            }

        }
    }

    string[] GetNumberOfWords()
    {
        if (AudioCaptions == null)
            return null;

        //Count all the words
        var words = AudioCaptions.text.Split(' ');
        return words;
    }

    public void SetAudioCaptions(string name, string filePath)
    {
        if (name == null || name == "")
            return;

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

    internal void SetPlaying(bool v)
    {
        if (clipEnded)
        {
            src.Pause();
            return;
        }

        if (v)
        {
            if (!viewing)
                return;

            //We want to play
            if (!src.isPlaying)
                src.Play();
            nameText.SetActive(true);
            text.SetActive(true);
            img.SetActive(true);

        }
        else
        {
            //we don't want to play
            if (src.isPlaying)
                src.Pause();

            nameText.SetActive(false);
            text.SetActive(false);
            img.SetActive(false);
        }
    }

    IEnumerator CheckIsFinished()
    {
        while (true)
        {
            if (src.isPlaying)
            {
                //check if the clip is finished
                if ((int)src.time >= (int)src.clip.length)
                {
                    //Debug.Log("clip over");
                    clipEnded = true;
                }
            }
            yield return null;
        }
    }
}
