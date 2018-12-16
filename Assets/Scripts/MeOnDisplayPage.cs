﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI_Builder;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

public class MeOnDisplayPage : MonoBehaviour {


    List<string> Dancers;
    ScrollRect scroll;
    Special_AccessibleButton AccessibleButton = null;

    // Use this for initialization
    void Start () {
        Dancers = new List<string>(){"Desmond Cadogan", "Chris Braz", "Peter Trojic", "Victoria Dombroski", "Tianshi Suo", "Tiffany Geigel", "Donald Lee",
                                      "Louisa Mann", "Leslie Taub", "Jerron Herman", "Kelly Ramis", "Nico Gonzales", "Meredith Fages", "Amy Meisner", "Jillian Hollis",
                                      "Jaclyn Rea", "Carmen Schoenster"};

        scroll = GetComponentInChildren<ScrollRect>();

       GetComponent<UIB_Page>().OnActivated += PageActivated;
        GetComponent<UIB_Page>().OnDeActivated += PageDeActivated;
    }

    private void PageActivated()
    {
        GameObject.FindWithTag("App_VideoPlayer").GetComponent<App_VideoPlayer>().OriginScreen = gameObject;

        //sort alphabetically
        var OrderedByName = Dancers.OrderBy(x => x);

        int i = 0;
        foreach (string dancer in OrderedByName){
            var b = ObjPoolManager.RetrieveFromPool(ObjPoolManager.Pool.Button);

            if (b != null)
            {
                b.name = dancer + " video";

                b.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { PlayVideo(dancer); });
                b.transform.SetParent(scroll.content.transform);

                var ab = b.GetComponent<UI_Builder.UIB_Button>();
                ab.SetButtonText(dancer);
                ab.Button_Opens = UI_Builder.UIB_Button.UIB_Button_Activates.Video;

                ab.Init();

                var sab = b.GetComponent<Special_AccessibleButton>();
                if(i==0){
                    AccessibleButton = sab;
                }

                sab.m_ManualPositionOrder = i;
                sab.m_ManualPositionParent = transform.parent.gameObject;

                i++;
            }
            else
                Debug.LogError("Not enough objects in pool");
        }

        var theScroll = transform.Find("Interface").GetComponentInChildren<ScrollRect>().gameObject;
        theScroll.SetActive(false);
        theScroll.SetActive(true);

        GameObject.FindWithTag("App_VideoPlayer").GetComponent<UIB_Page>().DeActivate();
    }
    private void PageDeActivated()
    {
        //Return the objecst to the pool
        ObjPoolManager.Init();
    }

    void PlayVideo(string src)
    {
        var filename = "MeOnDisplay/" + src.Replace(" ", "_");
        VideoClip videoSource = Resources.Load<VideoClip>(filename) as VideoClip;

        var captionFile = "VideoCaptions/" + src.Replace(" ", "_");
        TextAsset captions = Resources.Load<TextAsset>(captionFile) as TextAsset;

        var vp = GameObject.FindWithTag("App_VideoPlayer").GetComponent<VideoPlayer>();
        vp.GetComponent<App_VideoPlayer>().SetVideoCaptions(captions);
        vp.clip = videoSource;
        StartCoroutine("PlayVideoCoroutine");
    }

    IEnumerator PlayVideoCoroutine(){
        //Move the current screen out
        GetComponent<UIB_Page>().StartCoroutine("MoveScreenOut");

        //Get the video player screen and associated script
        var avp = GameObject.FindWithTag("App_VideoPlayer").GetComponent<App_VideoPlayer>();
        avp.StopAllCoroutines();
        yield return avp.StartCoroutine("PlayVideo");
        
        yield break;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
