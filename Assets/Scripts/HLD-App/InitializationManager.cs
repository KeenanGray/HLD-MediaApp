using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HLD;
using UI_Builder;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static HLD.JSON_Structs;

public class InitializationManager : MonoBehaviour
{
    GameObject aspectManager;

    GameObject AccessibilityInstructions;

    Database_Accessor db_Manager;
    public float InitializeTime;
    float t1;
    float t2;

    int numberOfBundles = 8;
    bool haveAllAssetBundles;

    private GameObject NoWifi;
    Color tmpColor;
    private GameObject tmpLandingPage;

    void Start()
    {
        Debug.Log(Application.persistentDataPath);
#if UNITY_EDITOR
        UIB_AspectRatioManager_Editor.Instance().IsInEditor = false;
#endif
        aspectManager = GameObject.FindGameObjectWithTag("MainCanvas");

        UAP_AccessibilityManager.RegisterOnTwoFingerSingleTapCallback(StopSpeech);
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


        UAP_AccessibilityManager.PauseAccessibility(true);

        if (aspectManager == null)
        {
            Debug.LogWarning("Unable to find Aspect Manager");
            yield break;
        }

        NoWifi = GameObject.Find("NoWifiIcon");
        if (NoWifi == null)
        {
            Debug.LogWarning("Unable to find NoWifi Logo");
            yield break;
        }

        tmpColor = NoWifi.GetComponentInChildren<Image>().color;
        NoWifi.GetComponentInChildren<Image>().color = new Color(tmpColor.r, tmpColor.g, tmpColor.b, 0);

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

        haveAllAssetBundles = false;
        yield return ManageAssetBundleFiles();


        foreach (UI_Builder.UIB_Button ab in GetComponentsInChildren<UI_Builder.UIB_Button>())
        {
            ab.Init();
        }

        foreach (UIB_IPage p in GetComponentsInChildren<UIB_IPage>())
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

        if (tmpLandingPage != null)//in some cases, we will move in a seperate warning screen
        {
            tmpLandingPage.GetComponent<UIB_Page>().StopAllCoroutines();
            firstScreen = tmpLandingPage;
        }

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

        //        Debug.Log("Init Time " + Time.time);

        MainContainer.DisableCover();
        yield break;
    }

    private IEnumerator ManageAssetBundleFiles()
    {
        //First check if we have local versions of the files
        yield return LoadAssetBundles();

        //if we have local files, see if we have an internet connection
        if (CheckInternet())
        {
            //If we have internet, compare versions of each files
            UpdateFilesIfNecessary();
            yield break;
        }
        else
        {
            //No internet, we will continue with app based on most recent version.
            //This function will add global UI button (no internet indicator) - click button to attempt an update
            ActivateNoInternetMode();
            yield break;
        }
        //TODO: COme back and fix the internet checks with AWS if necessary: the asset bundle may be small enough to live inside the App
        //The app is missing one or all local files. 
        //Check for internet
        if (CheckInternet())
        {
            //We have internet, 
            //TODO: alert the user that we will be downloading data
            DownloadFilesFromDatabase();
            yield break;
        }
        else
        {
            //We do not have internet
            //Alert the user that the app will have very limited functionality until connected to the internet
            ActivateLimitedFunctionality();
            yield break;
        }
        yield break;
    }

    private void ActivateLimitedFunctionality()
    {
        //Bring up no internet logo. 
        UIB_PageManager.InternetActive = false;
        NoWifi.GetComponentInParent<Canvas>().enabled = true;
        NoWifi.GetComponentInChildren<Image>().color = new Color(tmpColor.r, tmpColor.g, tmpColor.b, 130);
        NoWifi.GetComponent<Button>().interactable = true;

        tmpLandingPage = GameObject.Find("NoInternetCriticalLanding");
    }

    private void DownloadFilesFromDatabase()
    {
        //TODO: Alert the user we are about to begin a large download
        Debug.LogWarning("Fetching Downloads from Database: If your are testing, it's possible a file is missing");

        //How often can we call this download function before it costs too much $$$
        db_Manager.GetObjects("hld-general");
        db_Manager.GetObjects("hld-displayed");
    }

    private void ActivateNoInternetMode()
    {
        //Bring up no internet logo. 
        UIB_PageManager.InternetActive = false;
        NoWifi.GetComponentInChildren<Image>().color = new Color(tmpColor.r, tmpColor.g, tmpColor.b, 130);
        NoWifi.GetComponent<Button>().interactable = true;

        tmpLandingPage = GameObject.Find("NoInternetModeLanding");
    }

    public void UpdateFilesIfNecessary()
    {
        Debug.LogWarning("return here and check for updates to asset bundles");
    }

    void SetupArrayOfInterest()
    {
        List<string> MatchingObjects = new List<string>();

        var tmp = GetListOfDancers();
        for (int i = 0; i < tmp.Length; i++)
        {
            MatchingObjects.Add(tmp[i]);
        }
        MatchingObjects.Add("Bios");
        MatchingObjects.Add("AccessCode");
        MatchingObjects.Add("Displayed_AudioDescriptions");
        MatchingObjects.Add("ListOfDancers");

        db_Manager.SetMatchingObjects(MatchingObjects);
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
        throw new NotImplementedException();
    }

    private IEnumerator LoadAssetBundles()
    {
        string persistantDataPath = Application.streamingAssetsPath;

        //use the relevant asset bundle path for each platform
#if UNITY_IOS && !UNITY_EDITOR
        persistantDataPath+="/ios/";
#elif UNITY_ANDROID && !UNITY_EDITOR
        persistantDataPath+="/android/";
#endif
#if UNITY_EDITOR
        //if we are in the editor just use ios files...i guess...
        persistantDataPath += "/ios/";
#endif

        yield return tryLoadAssetBundle(persistantDataPath + "hld/general");
        yield return tryLoadAssetBundle(persistantDataPath + "hld/bios/json");
        yield return tryLoadAssetBundle(persistantDataPath + "hld/bios/photos");
        yield return tryLoadAssetBundle(persistantDataPath + "hld/displayed/audio");
        yield return tryLoadAssetBundle(persistantDataPath + "hld/displayed/narratives/captions");
        yield return tryLoadAssetBundle(persistantDataPath + "hld/displayed/narratives/photos");
        yield return tryLoadAssetBundle(persistantDataPath + "hld/displayed/narratives/audio");
        yield return tryLoadAssetBundle(persistantDataPath + "hld/meondisplay/videos");
        yield return tryLoadAssetBundle(persistantDataPath + "hld/meondisplay/captions");


        if (AssetBundle.GetAllLoadedAssetBundles().Count() == numberOfBundles)
        {
            haveAllAssetBundles = true;
        }
        else
        {
            haveAllAssetBundles = false;
        }

        //If we get here, we have all the files we want
        yield break;
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

    private void OnDestroy()
    {
        /*  foreach (AssetBundle bundle in AssetBundle.GetAllLoadedAssetBundles())
           {
               bundle.Unload(false);
           }
           */
    }

    IEnumerator tryLoadAssetBundle(string path)
    {
        var bundleLoadRequest = AssetBundle.LoadFromFileAsync(path);
        yield return bundleLoadRequest;

        var myLoadedAssetBundle = bundleLoadRequest.assetBundle;

        if (myLoadedAssetBundle == null)
        {
            Debug.LogError("Failed to load AssetBundle " + myLoadedAssetBundle.name);
            yield break;
        }

        yield break;
    }
}

