using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIB_AudioPlayer : MonoBehaviour {

    Image BgPhoto;
    TextMeshProUGUI Title;
    AudioSource src;
    public UIB_AudioPlayerTools myTools;

    // Use this for initialization
    void Start () {
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
                myTools = tools;
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
            myTools.Init();
        }
    }
}
