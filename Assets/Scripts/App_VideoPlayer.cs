using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class App_VideoPlayer : MonoBehaviour {

    GameObject cover;
    GameObject CaptionsCanvas;

    public VideoClip myClip;
    VideoPlayer myPlayer;
    TextAsset VideoCaptions;

    public GameObject OriginScreen;

    // Use this for initialization
    void Start () {
        cover = transform.Find("Cover").gameObject;
        CaptionsCanvas = transform.Find("CaptionsCanvas").gameObject;
        if (CaptionsCanvas == null)
            Debug.LogError("no canvas");

        myClip = GetComponent<VideoPlayer>().clip;
        myClip = null;

        myPlayer = GetComponent<VideoPlayer>();
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    public IEnumerator PlayVideo(){
        WaitForSeconds delay = new WaitForSeconds(0.5f);

        while (myClip == null)
        {
            myClip = myPlayer.clip;
            yield return delay;
        }

        yield return delay;
        myPlayer.Prepare();
        yield return delay;

        myPlayer.enabled = false;
        myPlayer.enabled = true;
        cover.SetActive(false);

        myPlayer.Play();
        StartCoroutine("PlayCaptionsWithVideo");

        var t0 = Time.time;
        var t1 = Time.time;

        while(myClip.length - (t1-t0) >= 0){
            t1 = Time.time;
            yield return delay;
        }
        yield return new WaitForSeconds(1.0f);
        OnClipEnd();
        
        yield break;
    }

    void OnClipEnd(){
        myClip = null;
        cover.SetActive(true);
        GetComponentInParent<Page>().StartCoroutine("MoveScreenOut");
        var listPage = GameObject.Find("#MeOnDisplay_Page");
        listPage.GetComponent<Page>().StartCoroutine("MoveScreenIn");
        UAP_AccessibilityManager.SelectElement(listPage);
    }

    public void SetVideoCaptions(TextAsset newText){
        if(newText==null)
        {
            Debug.Log("Null Text Given for Captions");
            return;
        }
        VideoCaptions = newText;
    }

    IEnumerator PlayCaptionsWithVideo(){
        //set up video captions
        TextMeshProUGUI tmp = CaptionsCanvas.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp == null)
            Debug.LogWarning("couldnt find text");

        var words = GetNumberOfLines();

        int start = 0;
        int WordsPerLine = 6;
        string line = "";

        var TimePerLine = (myClip.length - 2)
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

    string[] GetNumberOfLines(){
        //Count all the words
        var words = VideoCaptions.text.Split(' ');
        return words;
    }

}
