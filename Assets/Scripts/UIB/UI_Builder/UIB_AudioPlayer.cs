using System;
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

        foreach (TextMeshProUGUI tm in GetComponentsInChildren<TextMeshProUGUI>())
        {
            if (tm.name == "Title")
                Title = tm;
        }
        foreach (Image i in GetComponentsInChildren<Image>())
        {
            if (i.name == "Image")
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
                    if (CaptionsCanvas.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().enabled)
                    {
                        b.GetComponentInChildren<TextMeshProUGUI>().faceColor = new Color32(255, 255, 255, 255);
                    }
                    else
                    {
                        b.GetComponentInChildren<TextMeshProUGUI>().faceColor = new Color32(200, 197, 43, 255);
                    }
                    CaptionsCanvas.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().enabled = (!CaptionsCanvas.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().enabled);
                });
            }
        }
    }

    public void Init()
    {
        CaptionsToggle.GetComponentInChildren<TextMeshProUGUI>().enabled = hasCaptions;
        CaptionsToggle.GetComponent<Button>().enabled = hasCaptions;

        GetComponent<AspectRatioFitter>().enabled = false;
        GetComponent<AspectRatioFitter>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetTitle(string str)
    {
       // cover.SetActive(true);
        Title.text = str;
    }

    public void SetImageFromResource(string PathToImage)
    {
        Sprite ImageToUse = null;

        if(PathToImage==null)
        { }
        else
        {
            ImageToUse = Resources.Load<Sprite>(PathToImage) as Sprite;
        }

        if (ImageToUse != null)
        {

        }
        else
        {
            if (BgPhoto != null)
            {
                BgPhoto.preserveAspect = false;
                BgPhoto.color = new Color(BgPhoto.color.r, BgPhoto.color.g, BgPhoto.color.b, 255);
            }
        }
        if (BgPhoto != null)
        {
            BgPhoto.sprite = ImageToUse;
            BgPhoto.rectTransform.sizeDelta = new Vector2(1000, 1000);
        }
    }

    public void SetImageAssetBundle(string PathToImage, string bundleString)
    {
        Sprite ImageToUse = null;
        AssetBundle tmp = null;

        foreach(AssetBundle b in AssetBundle.GetAllLoadedAssetBundles())
        {
            if (b.name == bundleString)
                tmp = b;
        }

        ImageToUse = tmp.LoadAsset<Sprite>(PathToImage);

        if (ImageToUse != null)
        {

        }
        else
        {
            if (BgPhoto != null)
            {
                BgPhoto.preserveAspect = false;
                BgPhoto.color = new Color(BgPhoto.color.r, BgPhoto.color.g, BgPhoto.color.b, 255);
            }
        }
        if (BgPhoto != null)
        {
            BgPhoto.sprite = ImageToUse;
            BgPhoto.rectTransform.sizeDelta = new Vector2(1000, 1000);
        }
    }

    public void SetImageFromFile(string PathToImage)
    {
        byte[] fileData = null;

        if (FileManager.FileExists(PathToImage, UIB_FileTypes.Images))
        {
            fileData = FileManager.ReadFromBytes(PathToImage, UIB_FileTypes.Images);
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


    string url;
    public void SetAudio(string PathToAudio, string bundleString)
    {
        AssetBundle tmp = null;
        foreach(AssetBundle b in AssetBundle.GetAllLoadedAssetBundles()) {
            if (b.name == bundleString)
                tmp = b;
        }
        if (tmp != null && src!=null)
        {
            src.clip = tmp.LoadAsset<AudioClip>(PathToAudio) as AudioClip;
            src.time = 0;
            Tools.Init();
        }
    }


    IEnumerator GetAudioClip()
    {
        AudioClip AudioToUse = null;

        UnityWebRequest www= null;
        if (fileType == ".wav")
        {
            www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV);
        }
        else if (fileType == ".mp3")
        {
            www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG);
        }
        else
        {
            Debug.LogWarning("file type not specified");
        }

        if (www == null)
            yield break;

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            while (!www.downloadHandler.isDone)
            {
                Debug.Log(www.downloadHandler.isDone);
                yield return null;
            }

            AudioToUse = ((DownloadHandlerAudioClip)www.downloadHandler).audioClip;
        }

        if (AudioToUse == null)
            Debug.Log("AUDIO IS NULL");

        if (src != null)
        {
            src.clip = AudioToUse;
            src.time = 0;
            Tools.Init();
        }

       // cover.SetActive(false);

        var AudioPlayerScreen = GameObject.Find("AudioPlayer_Page");
        AudioPlayerScreen.GetComponent<UIB_AudioPlayer>().Tools.PlayMethod(1);
        yield break;
    }

    public void SetAudioCaptions(string name, string filePath)
    {
        var newText = FileManager.ReadTextAssetBundle(name, filePath);

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
