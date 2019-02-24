using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HLD;
using UI_Builder;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InitializationManager : MonoBehaviour
{
    GameObject aspectManager;

    GameObject AccessibilityInstructions;
    GameObject blankPage;

    Database_Accessor db_Manager;
    public float InitializeTime;
    float t1;
    float t2;

    int numberOfBundles = 8;

    Color tmpColor;

    public static int DownloadCount = 0;
    public static int checkingForUpdates = 0;

    private bool hasAllFiles;

    void Start()
    {
        Debug.Log(Application.persistentDataPath);
#if UNITY_EDITOR
        UIB_AspectRatioManager_Editor.Instance().IsInEditor = false;
#endif
        StartCoroutine("Init");
    }

    private void StopSpeech()
    {
        if (UAP_AccessibilityManager.IsSpeaking())
        {
            UAP_AccessibilityManager.StopSpeaking();
        }
        Debug.Log("TWOFINGERSINGLE");
        UAP_AccessibilityManager.Say("", false, true, UAP_AudioQueue.EInterrupt.All);

    }

    private void Update()
    {
    }

    IEnumerator Init()
    {
        UIB_PlatformManager.Init();
        hasAllFiles = false;

        aspectManager = GameObject.FindGameObjectWithTag("MainCanvas");
        blankPage = GameObject.Find("BlankPage");

        UAP_AccessibilityManager.RegisterOnTwoFingerSingleTapCallback(StopSpeech);
        
        yield return new WaitForSeconds(1.0f);
        AccessibilityInstructions = GameObject.Find("AccessibleInstructions_Button");
        //enable accessible instructins if plugin is on
        if (UAP_AccessibilityManager.IsActive())
        {
            AccessibilityInstructions.SetActive(true);
        }
        else
        {
            if (AccessibilityInstructions != null)
                AccessibilityInstructions.SetActive(false);
            else
                Debug.LogWarning("No accessibility instructions assigned");
        }

        ObjPoolManager.Init();


        if (aspectManager == null)
        {
            Debug.LogWarning("Unable to find Aspect Manager");
            yield break;
        }
        t1 = Time.time;

        foreach (AspectRatioFitter arf in GetComponentsInChildren<AspectRatioFitter>())
        {
            arf.aspectRatio = (UIB_AspectRatioManager.ScreenWidth) / (UIB_AspectRatioManager.ScreenHeight);
            arf.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            arf.enabled = true;
        }


        UIB_PageContainer MainContainer = null;
        foreach (UIB_PageContainer PageContainer in GetComponentsInChildren<UIB_PageContainer>())
        {
            MainContainer = PageContainer;
            MainContainer.Init();
        }
        if (GameObject.Find("DB_Manager") != null)
        {
            db_Manager = GameObject.Find("DB_Manager").GetComponent<HLD.Database_Accessor>();
        }
        if (db_Manager == null)
        {
            Debug.LogError("No Database Manager");
        }

        db_Manager.Init();

        yield return ManageAssetBundleFiles();

        StartCoroutine("CheckWifiAndDownloads");

        foreach (UI_Builder.UIB_Button ab in GetComponentsInChildren<UI_Builder.UIB_Button>())
        {
            ab.Init();
        }

        foreach (UIB_IPage p in GetComponentsInChildren<UIB_IPage>())
        {
            p.Init();
        }

        foreach (CompanyDancers_Page p in GetComponentsInChildren<CompanyDancers_Page>())
        {
            p.Init();
        }

        foreach (UIB_Page p in GetComponentsInChildren<UIB_Page>())
        {
            //TODO:Fix this bad bad shit
            if (p.gameObject.name == "Landing_Page")
                yield return p.MoveScreenOut(true);
            else
            {
                p.StartCoroutine("MoveScreenOut", true);
            }
        }

        var firstScreen = GameObject.Find("Landing_Page");

        yield return firstScreen.GetComponent<UIB_Page>().StartCoroutine("MoveScreenIn", true);

        //if we finish initializing faster than expected, take a moment to finish the video
        t2 = Time.time;
        var elapsed = t2 - t1;
        if (InitializeTime > elapsed)
            yield return new WaitForSeconds(InitializeTime - elapsed);
        else if (Mathf.Approximately(InitializeTime, float.Epsilon))
            Debug.Log("took " + elapsed + "s to initialize");
        else
            Debug.LogWarning("Took longer to initialize than expected");

        UAP_AccessibilityManager.PauseAccessibility(false);
        var first = GameObject.Find("DISPLAYED-Code_Button");

        UAP_AccessibilityManager.SelectElement(first, true); ;

        MainContainer.DisableCover();
        yield break;
    }

    private IEnumerator ManageAssetBundleFiles()
    {
        //First check if we have local versions of the files
        yield return CheckLocalFiles();
        if (hasAllFiles)
        {
            blankPage.transform.SetAsLastSibling();
            GameObject.Find("MainCanvas").GetComponent<UIB_AssetBundleHelper>().StartCoroutine("LoadAssetBundlesInBackground");
        }
        else
        {
            Debug.Log("we don't have all the files");
        }
    }

    private IEnumerator CheckLocalFiles()
    {
        UIB_PlatformManager.persistantDataPath = Application.persistentDataPath + "/heidi-latsky-dance/";

        //check for relevant asset bundle files
        //First check that platform specific assetbundle exists
        var filename = UIB_PlatformManager.platform+"/";

        filename = "hld/" + filename;
        //TODO: DeAuth if Default_Code.json is older than 24 hours and doesn't match current code.
        //Next up: Check for "general" asset bundle
        filename =  "general";
        filename = "hld/" + filename;
        TryDownloadFile(UIB_PlatformManager.persistantDataPath, UIB_PlatformManager.platform, filename);

        filename = "bios/json";
        filename = "hld/" + filename;
        TryDownloadFile(UIB_PlatformManager.persistantDataPath, UIB_PlatformManager.platform, filename);

        filename = "bios/photos";
        filename = "hld/" + filename;
        TryDownloadFile(UIB_PlatformManager.persistantDataPath, UIB_PlatformManager.platform, filename);

        filename = "displayed/audio";
        filename = "hld/" + filename;
        TryDownloadFile(UIB_PlatformManager.persistantDataPath, UIB_PlatformManager.platform, filename);

        filename = "displayed/narratives/audio";
        filename = "hld/" + filename;
        TryDownloadFile(UIB_PlatformManager.persistantDataPath, UIB_PlatformManager.platform, filename);

        filename = "displayed/narratives/captions";
        filename = "hld/" + filename;
        TryDownloadFile(UIB_PlatformManager.persistantDataPath, UIB_PlatformManager.platform, filename);


        filename = "displayed/narratives/photos";
        filename = "hld/" + filename;
        TryDownloadFile(UIB_PlatformManager.persistantDataPath, UIB_PlatformManager.platform, filename);

        filename = "meondisplay/captions";
        filename = "hld/" + filename;
        TryDownloadFile(UIB_PlatformManager.persistantDataPath, UIB_PlatformManager.platform, filename);

        //TODO:figure out video loading
        /*
        filename = "meondisplay/videos";
        if (!(FileManager.FileExists(persistantDataPath + platform + filename)))
        {
            if (CheckInternet())
            {
                //Download the file
                DownloadFileFromDatabase(persistantDataPath + platform, platform + filename);
            }
            else
                yield break;
        }
        */

        //if we get here we have all the files
        hasAllFiles = true;
        yield break;
    }

    private void TryDownloadFile(string persistantDataPath, string platform, string filename)
    {
        if (!(UIB_FileManager.FileExists(persistantDataPath + platform + filename)))
        {
            if (CheckInternet())
            {
                //Download the file
                Debug.Log("file" + persistantDataPath + platform + filename + " does not exist download commencing download");
                DownloadFileFromDatabase(persistantDataPath + platform, platform + filename);
            }
            else
            {
                //no internet, load bundle from streaming assets
                Debug.Log("loading bundle from streaming assets " + platform + filename);
                AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + platform + filename);
            }
        }
        else
        {
            //we have the file check for update
            if (CheckInternet())
            {
                db_Manager.CheckIfObjectHasUpdate(persistantDataPath + platform + filename, platform + filename, "heidi-latsky-dance");
            }
        }
    }

    private void ActivateLimitedFunctionality()
    {
        //TODO: refactor this
        /*
        //Bring up no internet logo. 
        UIB_PageManager.InternetActive = false;
        NoWifi.GetComponentInParent<Canvas>().enabled = true;
        NoWifi.GetComponentInChildren<Image>().color = new Color(tmpColor.r, tmpColor.g, tmpColor.b, 130);
        NoWifi.GetComponent<Button>().interactable = true;

        tmpLandingPage = GameObject.Find("NoInternetCriticalLanding");
        */
    }

    private void DownloadFileFromDatabase(string path, string fName)
    {
        //TODO: Alert the user we are about to begin a large download
        //How often can we call this download function before it costs too much $$$
        //db_Manager.GetObjectFromBucketByName(name, "heidi-latsky-dance");
        db_Manager.GetObject(fName, "heidi-latsky-dance");
    }

    private bool CheckInternet()
    {
        switch (Application.internetReachability)
        {
            case NetworkReachability.NotReachable:
                UIB_PageManager.InternetActive = false;
                return false;
            case NetworkReachability.ReachableViaLocalAreaNetwork:
                UIB_PageManager.InternetActive = true;
                return true;
            case NetworkReachability.ReachableViaCarrierDataNetwork:
                UIB_PageManager.InternetActive = true;
                return false;
        }
        return false;
    }

    private string[] GetListOfDancers()
    {
        AssetBundle tmp = null;
        string[] list;

        foreach (AssetBundle b in AssetBundle.GetAllLoadedAssetBundles())
        {
            if (b.name == "hld/general")
                tmp = b;
        }
        if (tmp != null)
            list = tmp.LoadAsset<TextAsset>("listofdancers").ToString().Split(',');

        else
        {
            Debug.LogWarning("No list of dancers");
            return null;
        }

        var str_list = list.OrderBy(x => x).ToArray();
        for (int i = 0; i < list.Length; i++)
        {
            //clean up newlines;
            list[i] = list[i].Replace("\n", "");
            list[i] = list[i].Replace("\r", "");
            list[i] = list[i].TrimEnd(System.Environment.NewLine.ToCharArray());
            list[i] = list[i].TrimStart(System.Environment.NewLine.ToCharArray());
        }
        return list;
    }

    IEnumerator CheckWifiAndDownloads()
    {
        GameObject WifiInUseIcon = null;
        GameObject NoWifiIcon = null;
        string persistantDataPath = Application.persistentDataPath + "/heidi-latsky-dance/";

        WifiInUseIcon = GameObject.Find("Wifi_Icon");
        NoWifiIcon = GameObject.Find("NoWifi_Icon");

        while (true)
        {
            if (WifiInUseIcon == null)
                Debug.LogError("bad");
            if (NoWifiIcon == null)
                Debug.LogError("worse");

            if (CheckInternet())
            {
                NoWifiIcon.SetActive(false);

                if (DownloadCount > 0 && checkingForUpdates <= 0)
                {
//                    Debug.Log("we have downloads going");
                    WifiInUseIcon.SetActive(true);

                }
                else
                {
                    WifiInUseIcon.SetActive(false);
                    /*
                      if (isAssetBundleLoaded(platform + "general"))
                         yield return tryLoadAssetBundle(persistantDataPath + platform + "general");
                     if (isAssetBundleLoaded(platform + "bios/json"))
                         yield return tryLoadAssetBundle(persistantDataPath + platform + "bios/json");
                     if (isAssetBundleLoaded(platform + "bios/photos"))
                         yield return tryLoadAssetBundle(persistantDataPath + platform + "bios/photos");
                     if (isAssetBundleLoaded(platform + "displayed/audio"))
                         yield return tryLoadAssetBundle(persistantDataPath + platform + "displayed/audio");
                     if (isAssetBundleLoaded(platform + "displayed/narratives/captions"))
                         yield return tryLoadAssetBundle(persistantDataPath + platform + "displayed/narratives/captions");
                     if (isAssetBundleLoaded(platform + "displayed/narratives/photos"))
                         yield return tryLoadAssetBundle(persistantDataPath + platform + "displayed/narratives/photos");
                     if (isAssetBundleLoaded(platform + "displayed/narratives/audio"))
                         yield return tryLoadAssetBundle(persistantDataPath + platform + "displayed/narratives/audio");
                     //TODO:Figure out videos
                     //if (isAssetBundleLoaded(platform + "general"))
                     // yield return tryLoadAssetBundle(persistantDataPath + platform + "meondisplay/videos");
                     if (isAssetBundleLoaded(platform + "meondisplay/captions"))
                         yield return tryLoadAssetBundle(persistantDataPath + platform + "meondisplay/captions");
                                     */
                    yield return null;
                }
            }
            else
            {
                NoWifiIcon.SetActive(true);
            }
            yield return null;
        }
    }


}

