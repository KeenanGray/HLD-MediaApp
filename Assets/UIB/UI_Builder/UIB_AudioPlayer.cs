using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIB_AudioPlayer : MonoBehaviour {

    Image BgPhoto;
    GameObject CaptionsCanvas;
    TextMeshProUGUI Title;
    AudioSource src;
    TextAsset AudioCaptions;
    public UIB_AudioPlayerTools tools;

    // Use this for initialization
    void Start () {
        CaptionsCanvas = transform.Find("CaptionsCanvas").gameObject;

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
        foreach(AudioSource srcObj in GetComponentsInChildren<AudioSource>())
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
                this.tools = tools;
            }
        }

        foreach (Button b in GetComponentsInChildren<Button>())
        {
            if (b.name == "Captions_Button")
            {
                b.onClick.AddListener(delegate {
                    CaptionsCanvas.SetActive(!CaptionsCanvas.activeSelf);
                });
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetTitle(string str)
    {
        Title.text = str;
    }

    public void SetImage(string PathToImage)
    {
        Sprite ImageToUse = null;
        if (PathToImage == null)
        {

        }
        else
        {
            ImageToUse = Resources.Load<Sprite>(PathToImage) as Sprite;
        }

        if (ImageToUse != null)
        {
            //Debug.Log(PathToImage + " : name " + ImageToUse.name);
        }
        else
        {
            if(ImageToUse!=null)
                Debug.LogWarning("Failed to load " + PathToImage);
            //turn off the audioDescriptionBGPhoto;
            if (BgPhoto != null)
            {
                BgPhoto.color = new Color(BgPhoto.color.r, BgPhoto.color.g, BgPhoto.color.b, 255);
            }
        }
        if (BgPhoto != null)
        {
            BgPhoto.sprite = ImageToUse;
            BgPhoto.rectTransform.sizeDelta = new Vector2(1000, 1000);
        }
    }

    public void SetAudio(string PathToAudio)
    {
        var AudioToUse = Resources.Load<AudioClip>(PathToAudio) as AudioClip;
        if (PathToAudio != null && AudioToUse!=null)
        {
 //           Debug.Log(PathToAudio + " : name " + AudioToUse.name);
        }
        else
        {
  //          Debug.LogWarning("Failed to load " + PathToAudio);
        }
        if (src != null)
        {
            src.clip = AudioToUse;
            tools.Init();
        }
    }

    public void SetAudioCaptions(TextAsset newText)
    {
        if (newText == null)
        {
            Debug.LogWarning("Null Text Given for Captions");
            CaptionsCanvas.SetActive(false);
            return;
        }
        AudioCaptions = newText;
    }

    IEnumerator PlayCaptionsWithAudio()
    {
        if (AudioCaptions == null)
            yield break;

        //set up video captions
        TextMeshProUGUI tmp = CaptionsCanvas.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp == null)
            Debug.LogWarning("couldnt find text");

        var words = GetNumberOfLines();

        int start = 0;
        int WordsPerLine = 6;
        string line = "";

        var TimePerLine = (src.clip.length - 2)
                /
            (words.Length / WordsPerLine);

        for (int i = start; i < words.Length; i += WordsPerLine)
        {
            line = "";
            for (int j = 0; j < WordsPerLine; j++)
            {
                if (i + j < words.Length)
                    line += words[i + j] + " ";
                else
                    break;
            }

            start += WordsPerLine;
            tmp.text = line;
            yield return new WaitForSeconds((float)TimePerLine);
        }

        yield break;
    }

    string[] GetNumberOfLines()
    {
        if (AudioCaptions == null)
            return null;

        //Count all the words
        var words = AudioCaptions.text.Split(' ');
        return words;
    }

}
