using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MeOnDisplayPage : MonoBehaviour {


    List<string> Dancers;
    ScrollRect scroll;

	// Use this for initialization
	void Start () {
        Dancers = new List<string>(){"Desmond Cadogan", "Chris Braz", "Peter Trojic", "Victoria Dombroski", "Tianshi Suo", "Tiffany Geigel", "Donald Lee",
                                      "Louisa Mann", "Leslie Taub", "Jerron Herman", "Kelly Ramis", "Nico Gonzales", "Meredith Fages", "Amy Meisner", "Jillian Hollis",
                                      "Jaclyn Rea", "Carmen Schoenster"};

        scroll = GetComponentInChildren<ScrollRect>();

       GetComponent<Page>().OnActivated += PageActivated;
        GetComponent<Page>().OnDeActivated += PageDeActivated;

    }

    private void PageActivated()
    {
        Special_AccessibleButton AccessibleButton = null;
        //sort alphabetically
        var OrderedByName = Dancers.OrderBy(x => x);

        foreach (string dancer in OrderedByName){
            var b = ObjPoolManager.RetrieveFromPool(ObjPoolManager.Pool.Button);

            int i = 0;
            if (b != null)
            {
                b.name = dancer + " video";

                b.GetComponent<Button>().onClick.AddListener(delegate { PlayVideo(dancer); });
                b.transform.SetParent(scroll.content.transform);

                var ab = b.GetComponent<App_Button>();
                ab.SetButtonText(dancer);
                ab.Button_Opens = App_Button.Button_Activates.Video;

                ab.Init();

                if(i==0){
                    AccessibleButton = b.GetComponent<Special_AccessibleButton>();
                    i++;
                }

            }
            else
                Debug.LogError("Not enough objects in pool");
        }
        Debug.LogError("PAUSING");

        if(AccessibleButton!=null)
            AccessibleButton.SelectItem();
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
        GetComponent<Page>().StartCoroutine("MoveScreenOut");

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
