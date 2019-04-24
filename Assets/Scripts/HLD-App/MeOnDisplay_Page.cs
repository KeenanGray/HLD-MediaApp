using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI_Builder;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

public class MeOnDisplay_Page : MonoBehaviour, UIB_IPage
{
    List<string> Dancers;
    List<string> DancerLinks;

    ScrollRect scroll;
    Special_AccessibleButton AccessibleButton = null;

    // Use this for initialization
    public void Init()
    {
        //TODO:Replace this with new code for Get list of dancers
        Dancers = new List<string>(){
        "Desmond Cadogan",
        "Chris Braz",
        "Peter Trojic",
        "Victoria Dombroski",
        "Tianshi Suo",
        "Tiffany Geigel",
        "Donald Lee",
        "Louisa Mann",
        "Leslie Taub",
        "Jerron Herman",
        "Kelly Ramis",
        "Nico Gonzales",
        "Meredith Fages",
        "Amy Meisner",
        "Jillian Hollis",
        "Jaclyn Rea",
        "Carmen Schoenster"};

        DancerLinks = new List<string>(){
        "https://www.youtube.com/embed/5TPBet8AEhA",//chris
        "https://www.youtube.com/embed/VJ-7A8UxrVE",//desmond
        "https://www.youtube.com/embed/wQe9mUcN25g",//victoria,
        "https://www.youtube.com/embed/k81_muECh7A",//meredith
        "https://www.youtube.com/embed/hO86WBGKuw0",//tiffany
        "https://www.youtube.com/embed/d5YPtFWB1zE",//nico
        "https://www.youtube.com/embed/QUNjumFRdX8", //jerron
        "https://www.youtube.com/embed/roYusKHjaSQ",//jillian
        "https://www.youtube.com/embed/Y0ONlXXe30o", //donald
        "https://www.youtube.com/embed/UAq0XEFFlm8",//louisa
        "https://www.youtube.com/embed/5rBFeBDBTK4",//amy
        "https://www.youtube.com/embed/1ONsL4OhtM4",//kelly
        "https://www.youtube.com/embed/YmvygifcTK0",//jaclyn
        "https://www.youtube.com/embed/Zwhbw1sNa3Q",//carmen
        "https://www.youtube.com/embed/cR0adJj-z3w",//tianshi
        "https://www.youtube.com/embed/z1CP4C54xVo",//leslie
        "https://www.youtube.com/embed/yXGRIeVEDdY",//peter
        };

        scroll = GetComponentInChildren<ScrollRect>();

        GetComponent<UIB_Page>().AssetBundleRequired = true;
        //     UIB_AssetBundleHelper.InsertAssetBundle("hld/general");
        //     UIB_AssetBundleHelper.InsertAssetBundle("hld/meondisplay/captions");

        GetComponent<UIB_Page>().OnActivated += PageActivatedHandler;
        GetComponent<UIB_Page>().OnDeActivated += PageDeActivatedHandler;
    }

    void PlayVideoFromFile(string src)
    {
        var filename = "/hld-general/MeOnDisplay/VideoCaptions/" + src.Replace(" ", "_") + ".txt";
        string videoSource = Application.persistentDataPath + "/hld-general/MeOnDisplay/" + src.Replace(" ", "_") + ".mov";

        TextAsset captions = new TextAsset(UIB_FileManager.ReadTextAssetBundle(filename, "hld/displayed/narratives/video"));

        var vp = GameObject.FindWithTag("App_VideoPlayer").GetComponent<VideoPlayer>();
        vp.GetComponent<UIB_VideoPlayer>().SetVideoCaptions(captions);

        vp.url = videoSource;
        StartCoroutine("PlayVideoCoroutine");
    }

    void PlayVideoFromAssetBundle(string src, string bundleString)
    {
        string videoStr = bundleString + "videos";
        string captionStr = bundleString + "captions";

        AssetBundle videoBundle = null;
        AssetBundle captionsBundle = null;

        var filename = src.Replace(" ", "_");

        foreach (AssetBundle b in AssetBundle.GetAllLoadedAssetBundles())
        {
            if (b.name == videoStr)
                videoBundle = b;
            if (b.name == captionStr)
                captionsBundle = b;
        }
        var vp = GameObject.FindWithTag("App_VideoPlayer").GetComponent<VideoPlayer>();

        if (videoBundle != null)
        {
            var videoSource = videoBundle.LoadAsset<VideoClip>(filename);
            vp.clip = videoSource;
        }
        else
        {
            Debug.LogWarning("no bundle with name " + videoStr);
        }

        if (captionsBundle != null)
        {
            TextAsset captions = captionsBundle.LoadAsset<TextAsset>(filename);
            vp.GetComponent<UIB_VideoPlayer>().SetVideoCaptions(captions);
        }
        else
        {
            Debug.LogWarning("no bundle with name " + captionStr);
        }

        StartCoroutine("PlayVideoCoroutine");
    }

    IEnumerator PlayVideoCoroutine()
    {
        //Move the current screen out
        GetComponent<UIB_Page>().StartCoroutine("MoveScreenOut", false);

        //Get the video player screen and associated script
        var avp = GameObject.FindWithTag("App_VideoPlayer").GetComponent<UIB_VideoPlayer>();
        avp.StopAllCoroutines();
        yield return avp.StartCoroutine("PlayVideo");

        yield break;
    }

    // Update is called once per frame
    void Update()
    {

    }

    /*
    //THIS IS THE OLD PAGEACTIVEDHANDLER THAT USED THE VIDEO PLAYER
    public void PageActivatedHandler()
    {
        if (GetComponent<UIB_Page>().AssetBundleRequired) {
            Debug.Log("we need an asset bundle for this page");
        }

        GameObject.FindWithTag("App_VideoPlayer").GetComponent<UIB_VideoPlayer>().OriginScreen = gameObject;

        //sort alphabetically
        var OrderedByName = Dancers.OrderBy(x => x);

        int i = 0;
        ObjPoolManager.BeginRetrieval();

        foreach (string dancer in OrderedByName)
        {
            GameObject b = null;
            ObjPoolManager.RetrieveFromPool(ObjPoolManager.Pool.Button, ref b);

            if (b != null)
            {
                b.name = dancer.Replace(" ", "_") + " video";

                b.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { PlayVideoFromAssetBundle(dancer, "hld/meondisplay/"); });
                b.transform.SetParent(scroll.content.transform);

                var ab = b.GetComponent<UI_Builder.UIB_Button>();
                ab.SetButtonText(dancer);
                ab.Button_Opens = UI_Builder.UIB_Button.UIB_Button_Activates.Video;

                ab.Init();

                var sab = b.GetComponent<Special_AccessibleButton>();
                if (i == 0)
                {
                    AccessibleButton = sab;
                }

                sab.m_ManualPositionOrder = i;
                sab.m_ManualPositionParent = transform.parent.gameObject;

                i++;
            }
            else
                Debug.LogError("Not enough objects in pool");
        }
        ObjPoolManager.EndRetrieval();

        var theScroll = transform.Find("UIB_ScrollingMenu");
        theScroll.GetComponent<UIB_ScrollingMenu>().playedOnce=false;
        theScroll.GetComponent<UIB_ScrollingMenu>().Setup();

        GameObject.FindWithTag("App_VideoPlayer").GetComponent<UIB_Page>().DeActivate();

        StartCoroutine(GetComponent<UIB_Page>().ResetUAP(true));
    }
    */

    //THIS IS THE NEW PAGE ACTIVASTED HANDLER
    //THIS ONE ADDS A BUTTON THAT OPENS A PAGE LINK TO THE VIDEO.
    public void PageActivatedHandler()
    {
        scroll.content.GetComponent<RectTransform>().pivot = new Vector2(0, 1);

        if (GetComponent<UIB_Page>().AssetBundleRequired)
        {
            Debug.Log("we need an asset bundle for this page");
        }

        GameObject.FindWithTag("App_VideoPlayer").GetComponent<UIB_VideoPlayer>().OriginScreen = gameObject;

        //sort alphabetically
        var OrderedByName = Dancers.OrderBy(x => x.Split(' ')[1]);

        int i = 0;
        ObjPoolManager.BeginRetrieval();

        foreach (string dancer in OrderedByName)
        {
            GameObject b = null;
            ObjPoolManager.RetrieveFromPool(ObjPoolManager.Pool.Button, ref b);

            if (b != null)
            {
                b.name = dancer.Replace(" ", "_") + " video";


                //b.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { PlayVideoFromAssetBundle(dancer, "hld/meondisplay/"); });
                b.transform.SetParent(scroll.content.transform);


                var ab = b.GetComponent<UIB_Button>();
                ab.SetButtonText(dancer);
                ab.Button_Opens = UIB_Button.UIB_Button_Activates.InAppUrl;
                ab.s_link = DancerLinks[i];

                ab.Init();

                var sab = b.GetComponent<Special_AccessibleButton>();
                if (i == 0)
                {
                    AccessibleButton = sab;
                }

                sab.m_ManualPositionOrder = i;
                sab.m_ManualPositionParent = transform.parent.gameObject;

                i++;
            }
            else
                Debug.LogError("Not enough objects in pool");
        }
        ObjPoolManager.EndRetrieval();

        var theScroll = transform.Find("UIB_ScrollingMenu");
        theScroll.GetComponent<UIB_ScrollingMenu>().playedOnce = false;
        theScroll.GetComponent<UIB_ScrollingMenu>().Setup();

        GameObject.FindWithTag("App_VideoPlayer").GetComponent<UIB_Page>().DeActivate();

        StartCoroutine(GetComponent<UIB_Page>().ResetUAP(true));
    }


    public void PageDeActivatedHandler()
    {
        //Return the objecst to the pool
        ObjPoolManager.RefreshPool();
    }
}
