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

    bool CaptionsShowing = true;

    public void Init()
    {
        AudioCaptions = new TextAsset("");

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
                    CaptionsShowing = !CaptionsShowing;

                    CaptionsCanvas.GetComponentInChildren<TextMeshProUGUI>().enabled = CaptionsShowing;

                    if (CaptionsCanvas.GetComponentInChildren<TextMeshProUGUI>().enabled)
                    {
                        b.GetComponentInChildren<TextMeshProUGUI>().color = new Color32(200, 197, 43, 255);
                    }
                    else
                    {
                        b.GetComponentInChildren<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255);
                    }
                });

                //  b.onClick.Invoke();
            }
        }

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
    /*
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
    */

    public void SetImageAssetBundle(string PathToImage, string bundleString)
    {
        GetComponent<AspectRatioFitter>().enabled = true;
        GetComponent<AspectRatioFitter>().enabled = false;
        Sprite ImageToUse = null;
        AssetBundle tmp = null;
        foreach (AssetBundle b in AssetBundle.GetAllLoadedAssetBundles())
        {
            //            Debug.Log(b.name);
            if (b.name == bundleString)
                tmp = b;
        }

        try
        {
            ImageToUse = tmp.LoadAsset<Sprite>(PathToImage.Replace(" ", "_"));
        }
        catch (Exception e)
        {
            if (e.GetBaseException().GetType() == typeof(NullReferenceException))
            {
                Debug.LogWarning("Null Reference Exception");
            }
            Debug.Log("asset not loaded: " + PathToImage + " b: " + bundleString + "::" + e);
        }
        if (BgPhoto != null)
        {
            BgPhoto.sprite = ImageToUse;

            //set recttransform aspect based on image and aspect ratio of screen
            var ar = UIB_AspectRatioManager.ScreenWidth / UIB_AspectRatioManager.ScreenHeight;
            var imgAR = 9f / 16f;

            if (!ar.Equals(imgAR))
            {
                try
                {
                    if (ImageToUse != null)
                        BgPhoto.rectTransform.sizeDelta = new Vector2(ImageToUse.rect.width, ImageToUse.rect.height * ar);
                }
                catch (Exception e)
                {
                    if (e.GetBaseException().GetType() == typeof(NullReferenceException))
                    {
                        Debug.LogError("Null Reference Exception");
                    }

                    Debug.Log("no image to use. " + PathToImage + "- - -" + e);
                }
            }
        }
        else
        { Debug.Log("BgPhoto object is null"); }
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

        if (newText == null || newText == "")
        {
            Debug.LogWarning("Null Text Given for Captions");
            //CaptionsCanvas.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
            hasCaptions = false;

            //no captions so no screen readable button
            // Debug.Log("turning captions reader off");

            StartCoroutine("TurnOffCaptionsReader");

            return;
        }
        //        Debug.Log("new " + newText);
        hasCaptions = true;
        AudioCaptions = new TextAsset(newText);

        //        Debug.Log("turning captions reader on");
        CaptionsToggle.GetComponent<UAP_BaseElement>().enabled = true;
        CaptionsToggle.GetComponent<Button>().enabled = true;
    }

    IEnumerator TurnOffCaptionsReader()
    {
        yield return new WaitForSeconds(0.5f);
        CaptionsToggle.GetComponent<UAP_BaseElement>().enabled = false;
        CaptionsToggle.GetComponent<Button>().enabled = false;

        UAP_AccessibilityManager.RecalculateUIElementsOrder();
        //We have to reinitialize sort order
        yield break;
    }

    int newStart;
    int iterator;
    bool wait;
    private bool hasCaptions;
    IEnumerator PlayCaptionsWithAudio()
    {
        TextMeshProUGUI tmp = CaptionsCanvas.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp == null)
        {
            Debug.LogWarning("couldn't find text");
            yield break;
        }
        if (AudioCaptions == null)
            yield break;

        var words = GetNumberOfLines();
        int WordsPerLine = 10;

        while (true)
        {
            var cur = src.time;
            var total = src.clip.length;
            var percent = cur / total;

            int current_word = (int)Mathf.Lerp(0, words.Length, percent);

            //convert the current word to find out which word starts that line.
            int word_to_line = (int)current_word / WordsPerLine;
            //Debug.Log(word_to_line + "  " + words[current_word]);

            var line = "";
            for (iterator = word_to_line * WordsPerLine; iterator <= (word_to_line * WordsPerLine) + WordsPerLine; iterator++)
            {
                if (iterator < words.Length)
                    line += words[iterator] + " ";
                else
                    break;
                tmp.text = line;
            }
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

    public void PageActivatedHandler()
    {
    }

    public void PageDeActivatedHandler()
    {
    }
}
