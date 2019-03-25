﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI_Builder;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UIB_AudioPlayer : MonoBehaviour, UIB_IPage
{
    GameObject cover;
    Image BgPhoto;
    GameObject CaptionsCanvas;
    TextMeshProUGUI Title;
    AudioSource src;
    TextAsset AudioCaptions;
    public UIB_AudioPlayerTools Tools;
    GameObject CaptionsToggle;
    public string fileType;

    // Use this for initialization
    void Start()
    {
        AudioCaptions = new TextAsset("");

        GetComponent<UIB_Page>().OnActivated += PageActivatedHandler;
        GetComponent<UIB_Page>().OnDeActivated += PageDeActivatedHandler;

        cover = transform.Find("Cover").gameObject;

        foreach (Canvas c in GetComponentsInChildren<Canvas>())
        {
            if (c.gameObject.name == "CaptionsCanvas")
            {
                CaptionsCanvas = c.gameObject;
            }
        }
        if (CaptionsCanvas == null)
            Debug.LogError("no captions canvas");

        foreach (TextMeshProUGUI tm in GetComponentsInChildren<TextMeshProUGUI>())
        {
            if (tm.name == "Title noedit")
                Title = tm;
        }
        foreach (Image i in GetComponentsInChildren<Image>())
        {
            if (i.name == "Background_Image")
            {
                BgPhoto = i;
            }
        }
        foreach (AudioSource srcObj in GetComponentsInChildren<AudioSource>())
        {
            if (srcObj.name == "AudioSourceAndTools")
            {
                src = srcObj;
            }
        }
        foreach (UIB_AudioPlayerTools tools in GetComponentsInChildren<UIB_AudioPlayerTools>())
        {
            if (tools.name == "AudioSourceAndTools")
            {
                this.Tools = tools;
            }
        }

        foreach (Button b in GetComponentsInChildren<Button>())
        {
            if (b.name == "Captions_Button")
            {
                CaptionsToggle = b.gameObject;
                b.onClick.RemoveAllListeners();
                b.GetComponentInChildren<TextMeshProUGUI>().faceColor = new Color32(255, 255, 255, 255);

                b.onClick.AddListener(delegate
                {
//                    Debug.Log("HERE " + CaptionsCanvas);
                    CaptionsCanvas.GetComponentInChildren<TextMeshProUGUI>().enabled = (!CaptionsCanvas.GetComponentInChildren<TextMeshProUGUI>().enabled);

                    if (CaptionsCanvas.GetComponentInChildren<TextMeshProUGUI>().enabled)
                    {
                        b.GetComponentInChildren<TextMeshProUGUI>().color = new Color32(200, 197, 43, 255);
                    }
                    else
                    {
                        b.GetComponentInChildren<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255);
                    }
                });

                b.onClick.Invoke();
            }
        }
    }



    public void Init()
    {
        CaptionsToggle.GetComponentInChildren<TextMeshProUGUI>().enabled = hasCaptions;
        CaptionsToggle.GetComponent<Button>().enabled = hasCaptions;

        GetComponent<AspectRatioFitter>().enabled = false;
        GetComponent<AspectRatioFitter>().enabled = true;
        StartCoroutine(GetComponent<UIB_Page>().ResetUAP(true));
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetTitle(string str)
    {
        // cover.SetActive(true);
        if (Title != null)
        {
            Title.text = str;
        }
        else
            Debug.LogError("Check the names of your gameobjects");


    }

    public void SetImageFromResource(string PathToImage, float size)
    {
        Sprite ImageToUse = null;

        if (PathToImage == null)
        { }
        else
        {
            ImageToUse = Resources.Load<Sprite>(PathToImage) as Sprite;
        }

        if (BgPhoto != null)
        {
            BgPhoto.sprite = ImageToUse;
            BgPhoto.rectTransform.sizeDelta = new Vector2(size, size);
        }
    }

    public void SetImageAssetBundle(string PathToImage, string bundleString)
    {
        GetComponent<AspectRatioFitter>().enabled = true;
        GetComponent<AspectRatioFitter>().enabled = false;
        Sprite ImageToUse = null;
        AssetBundle tmp = null;
        foreach (AssetBundle b in AssetBundle.GetAllLoadedAssetBundles())
        {
            if (b.name == bundleString)
                tmp = b;
        }

        try
        {
            ImageToUse = tmp.LoadAsset<Sprite>(PathToImage);
        }
        catch (Exception e)
        {
            if (e.GetBaseException().GetType() == typeof(NullReferenceException))
            {
                Debug.Log("asset not loaded");
            }
        }

        if (BgPhoto != null)
        {
            BgPhoto.sprite = ImageToUse;

            //set recttransform aspect based on image and aspect ratio of screen
            var ar = UIB_AspectRatioManager.ScreenWidth / UIB_AspectRatioManager.ScreenHeight;
            var imgAR = 9f / 16f;

            if (!ar.Equals(imgAR))
            {
                BgPhoto.rectTransform.sizeDelta = new Vector2(ImageToUse.rect.width, ImageToUse.rect.height * ar);
            }
        }
    }

    public void SetImageFromFile(string PathToImage)
    {
        byte[] fileData = null;

        if (UIB_FileManager.FileExists(PathToImage, UIB_FileTypes.Images))
        {
            fileData = UIB_FileManager.ReadFromBytes(PathToImage, UIB_FileTypes.Images);
            if (fileData == null)
            {
                Debug.Log("HERE");
                return;
            }
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);
            var newSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0), 100.0f);
            if (BgPhoto != null)
            {
                BgPhoto.sprite = newSprite;
                BgPhoto.rectTransform.sizeDelta = new Vector2(1000, 1000);
            }

        }
    }

    public void SetAudio(string PathToAudio, string bundleString)
    {
        AssetBundle tmp = null;
        foreach (AssetBundle b in AssetBundle.GetAllLoadedAssetBundles())
        {
            if (b.name == bundleString)
                tmp = b;
        }
        if (tmp != null && src != null)
        {
            src.clip = tmp.LoadAsset<AudioClip>(PathToAudio) as AudioClip;
            src.time = 0;
            Tools.Init();
        }
    }

    public void SetAudioCaptions(string name, string filePath)
    {
        var newText = UIB_FileManager.ReadTextAssetBundle(name, filePath);

        if (newText == null)
        {
            Debug.LogWarning("Null Text Given for Captions");
            CaptionsCanvas.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
            hasCaptions = false;

            return;
        }
        hasCaptions = true;
        AudioCaptions = new TextAsset(newText);
    }

    int newStart;
    int iterator;
    bool wait;
    private bool hasCaptions;
    IEnumerator PlayCaptionsWithAudio()
    {
        wait = true;
        newStart = 0;
        iterator = 0;

        var words = GetNumberOfLines();
        int WordsPerLine = 9;

        if (AudioCaptions == null)
            yield break;

        //set up video captions
        TextMeshProUGUI tmp = CaptionsCanvas.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp == null)
            Debug.LogWarning("couldnt find text");

        while (true)
        {
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

            // line = "<mark=#020202CC>";
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
            //line += "</mark>";
            iterator++;

            //break if skipped
            if (wait)
            {
                yield return new WaitForSeconds(TimePerLine);
            }
            else
            {
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

    public void PageActivatedHandler()
    {
        // Debug.Log("Activated");
    }

    public void PageDeActivatedHandler()
    {
        // Debug.Log("DeActivated");
    }
}