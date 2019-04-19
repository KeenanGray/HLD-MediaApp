using System;
using System.Collections;
using System.IO;
using System.Linq;
using HLD;
using TMPro;
using UI_Builder;
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

    public static float DownloadCount = 0;
    public static float TotalDownloads { get; private set; }

    public static int checkingForUpdates = 0;
    public static float PercentDownloaded = 0;

    public TextMeshProUGUI percentText;

    private bool hasCheckedFiles;


    void Start()
    {
#if UNITY_EDITOR
        UIB_AspectRatioManager_Editor.Instance().IsInEditor = false;
#endif
        StartCoroutine("Init");
    }

    private void Update()
    {

    }

    IEnumerator Init()
    {
        //set T1 for timing Init;
        t1 = Time.time;
        // Disable screen dimming
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        UIB_PlatformManager.Init();

        try
        {
            UAP_AccessibilityManager.PauseAccessibility(true);
        }
        catch (Exception e)
        {
            if (e.GetType() == typeof(NullReferenceException))
            {

            }
        }

        //this player pref should get set at app launch so that it resets the timecode in the audio-desc;
        PlayerPrefs.SetInt("desc_timecode", 0);

        try
        {
            percentText = GameObject.Find("DownloadPercent").GetComponent<TextMeshProUGUI>();
        }
        catch (Exception e)
        {
            Debug.Log("Failed to find GameObject: " + e);
            yield break;
        }

        try
        {
            db_Manager = GameObject.Find("DB_Manager").GetComponent<HLD.Database_Accessor>();
        }
        catch (Exception e)
        {
            Debug.Log("No database manager" + e);
            yield break;
        }
        db_Manager.Init();

        try
        {
            blankPage = GameObject.Find("BlankPage");
        }
        catch (Exception e)
        {
            Debug.Log("No blankpage " + e);
            yield break;
        }
        try
        {
            aspectManager = GameObject.FindGameObjectWithTag("MainCanvas");
        }
        catch (Exception e)
        {
            Debug.Log("no aspect ratio manager " + e);
            yield break;
        }

        try
        {
            AccessibilityInstructions = GameObject.Find("AccessibleInstructions_Button");
        }
        catch (Exception e)
        {
            Debug.Log("no instructions " + e);
            yield break;
        }

        //this coroutine checks the local files and starts any necessary downloads
        StartCoroutine("CheckLocalFiles");

        //this coroutine continously checks if we have wifi and downloads are happening
        //it updates the download icon accordingly
        StartCoroutine("CheckWifiAndDownloads");

        //this coroutine updates download percentage over time
        StartCoroutine("UpdateDownloadPercent");

        //this coroutine waits until we have checked for all the files
        //then it begins loading asset bundles in the background
        //it must be started after pages have initialized
        StartCoroutine("ManageAssetBundleFiles");

        //setup checks for accessibility on android - which is wierd;
#if UNITY_ANDROID && !UNITY_EDITOR
      Debug.Log("checking accessibility " + UAP_AccessibilityManager.GetAndroidAccessibility());
  
        if(UAP_AccessibilityManager.GetAndroidAccessibility()){
        Debug.Log("Accessibility ON");
        UAP_AccessibilityManager.EnableAccessibility(true);
        }
        else{
        Debug.Log("Accessibility OFF");
        UAP_AccessibilityManager.EnableAccessibility(false);
        }
#endif

        //Set the main page container
        //Can't remember why i did this
        UIB_PageContainer MainContainer = null;
        foreach (UIB_PageContainer PageContainer in GetComponentsInChildren<UIB_PageContainer>())
        {
            MainContainer = PageContainer;
            MainContainer.Init();
        }

        //set scroll rects to top
        foreach (Scrollbar sb in GetComponentsInChildren<Scrollbar>())
        {
            sb.value = 1;
        }

        //turn aspect ratio fitters on
        //causes all pages to share origin with canvas and be correct dimensions
        foreach (AspectRatioFitter arf in GetComponentsInChildren<AspectRatioFitter>())
        {
            arf.aspectRatio = (UIB_AspectRatioManager.ScreenWidth) / (UIB_AspectRatioManager.ScreenHeight);
            arf.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            arf.enabled = true;
        }

        //initialize each button
        foreach (UI_Builder.UIB_Button ab in GetComponentsInChildren<UI_Builder.UIB_Button>())
        {
            //before initializing buttons, we may change some names based on player_prefs

            if (ab.name == "Displayed-Code_Button")
                CheckAndUpdateLinks("Displayed-Info_Page");
            if (ab.name == "OnDisplay-Code_Button")
                CheckAndUpdateLinks("OnDisplay-Info_Page");
            ab.Init();
        }

        //initialize each page
        foreach (UIB_IPage p in GetComponentsInChildren<UIB_IPage>())
        {
            p.Init();
        }

        //initialize companty dancers page
        //TODO: might not need this
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

        //initialize each scrolling menu
        foreach (UIB_ScrollingMenu uibSM in GetComponentsInChildren<UIB_ScrollingMenu>())
        {
            uibSM.Init();
        }

        //initialize objects in the object pools
        //todo:tag this for eventual replacement with better pages/buttons 
        ObjPoolManager.Init();

        //this coroutine waits until we have checked for all the files
        //then it begins loading asset bundles in the background
        //it must be started after pages have initialized
        // StartCoroutine("ManageAssetBundleFiles");

        //setup the first screen
        var firstScreen = GameObject.Find("Landing_Page");
        yield return firstScreen.GetComponent<UIB_Page>().StartCoroutine("MoveScreenIn", true);

        if (UAP_AccessibilityManager.IsEnabled())
        {
            //unpause accessibility manger to read first button
            UAP_AccessibilityManager.PauseAccessibility(false);
            if (UAP_AccessibilityManager.IsActive())
            {
                AccessibilityInstructions.SetActive(true);
            }
            else
            {

            }

            //select the first button with UAP
            var first = GameObject.Find("DISPLAYED-Code_Button");
            UAP_AccessibilityManager.SelectElement(first, true); ;
        }

        //remove the cover
        MainContainer.DisableCover();

        //if we finish initializing faster than expected, take a moment to finish the video
        t2 = Time.time;
        var elapsed = t2 - t1;
        if (InitializeTime > elapsed)
            yield return new WaitForSeconds(InitializeTime - elapsed);
        else if (Mathf.Approximately(InitializeTime, float.Epsilon))
            Debug.Log("took " + elapsed + "s to initialize");
        else
            Debug.LogWarning("Took longer to initialize than expected");

        yield break;
    }

    private IEnumerator ManageAssetBundleFiles()
    {
        while (!hasCheckedFiles)
        {
            Debug.Log("we don't have all the files");
            yield return null;
        }
        blankPage.transform.SetAsLastSibling();
        GameObject.Find("MainCanvas").GetComponent<UIB_AssetBundleHelper>().StartCoroutine("LoadAssetBundlesInBackground");
    }

    private IEnumerator CheckLocalFiles()
    {

        //check for relevant asset bundle files
        //First check that platform specific assetbundle exists
        var filename = UIB_PlatformManager.platform + "/";

        TotalDownloads = 8;
        filename = "hld/" + filename;
        //TODO: DeAuth if Default_Code.json is older than 24 hours and doesn't match current code.
        //Next up: Check for "general" asset bundle
        filename = "general";
        filename = "hld/" + filename;
        TryDownloadFile(filename);

        filename = "bios/json";
        filename = "hld/" + filename;
        TryDownloadFile(filename);

        filename = "displayed/narratives/captions";
        filename = "hld/" + filename;
        TryDownloadFile(filename);


        filename = "bios/photos";
        filename = "hld/" + filename;
        TryDownloadFile(filename);

        filename = "displayed/narratives/photos";
        filename = "hld/" + filename;
        TryDownloadFile(filename);

        filename = "displayed/narratives/audio";
        filename = "hld/" + filename;
        TryDownloadFile(filename);

        filename = "displayed/audio";
        filename = "hld/" + filename;
        TryDownloadFile(filename);

        //OnDisplaySection
        filename = "ondisplay/narratives/audio";
        filename = "hld/" + filename;
        TryDownloadFile(filename);

        filename = "ondisplay/narratives/captions";
        filename = "hld/" + filename;
        TryDownloadFile(filename);

        filename = "ondisplay/narratives/photos";
        filename = "hld/" + filename;
        TryDownloadFile(filename);

        filename = "unfinished/audio";
        filename = "hld/" + filename;
        TryDownloadFile(filename);

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
        /*
  filename = "meondisplay/captions";
  filename = "hld/" + filename;
  TryDownloadFile(filename);
  */

        //if we get here we have all the files
        hasCheckedFiles = true;
        yield break;
    }

    private void TryDownloadFile(string filename, bool fallbackUsingBundle = false)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
         if (!(UIB_FileManager.FileExists(UIB_PlatformManager.persistentDataPath +"android/assets/"+ UIB_PlatformManager.platform + filename)))
        {
            //We don't have the file, first thing is to copy it from streaming assets
            //On Android, streaming assets are zipped so we need a special accessor
            GameObject.Find("FileManager").GetComponent<UIB_FileManager>().StartCoroutine("CreateStreamingAssetDirectories", filename);
        }
        else{
         //we have the file check for update
            if (CheckInternet())
            {
                db_Manager.CheckIfObjectHasUpdate(UIB_PlatformManager.persistentDataPath + UIB_PlatformManager.platform + filename, UIB_PlatformManager.platform + filename, "heidi-latsky-dance");
            }
        }
#else
        if (!(UIB_FileManager.FileExists(UIB_PlatformManager.persistentDataPath + UIB_PlatformManager.platform + filename)))
        {
            //we don't have the file, firs thing to do is copy it from streaming assets
            UIB_FileManager.WriteFromStreamingToPersistent(filename);
            // AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + UIB_PlatformManager.platform + filename);

            if (CheckInternet())
            {
                db_Manager.CheckIfObjectHasUpdate(UIB_PlatformManager.persistentDataPath + UIB_PlatformManager.platform + filename, UIB_PlatformManager.platform + filename, "heidi-latsky-dance");
            }
            else
            {
                //
            }
        }
        else
        {
            //we have the file check for update
            if (CheckInternet())
            {
                db_Manager.CheckIfObjectHasUpdate(UIB_PlatformManager.persistentDataPath + UIB_PlatformManager.platform + filename, UIB_PlatformManager.platform + filename, "heidi-latsky-dance");
            }

            //delete the streaming asset files
            UIB_FileManager.DeleteFile(filename);
        }
#endif
            UIB_AssetBundleHelper.InsertAssetBundle(filename);

    }

    private void ActivateLimitedFunctionality()
    {
        //TODO: refactor this
        /*
        //Bring up no internet logo. 
        UIB_PageManager.InternetActive = false;

        tmpLandingPage = GameObject.Find("NoInternetCriticalLanding");
        */
    }

    private void DownloadFileFromDatabase(string fName, bool fallbackUsingBundle = false)
    {
        //TODO: Alert the user we are about to begin a large download
        //How often can we call this download function before it costs too much $$$
        //db_Manager.GetObjectFromBucketByName(name, "heidi-latsky-dance");
        if (fallbackUsingBundle)
        {
            db_Manager.GetObjectWithFallback(fName, "heidi-latsky-dance");
        }
        else
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
                return true;
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
        string persistantDataPath = UIB_PlatformManager.persistentDataPath;

        WifiInUseIcon = GameObject.Find("DownloadIcon");

        while (true)
        {
            //Debug.Log("DL Count " + DownloadCount + " checking for " + checkingForUpdates);
            if (WifiInUseIcon == null)
            {
                Debug.Log("Bad");
            }

            if (CheckInternet())
            {
                //Debug.Log("We have internet");
                if (DownloadCount > 0 && checkingForUpdates <= 0)
                {
                    WifiInUseIcon.SetActive(true);
                }
                else
                {
                    WifiInUseIcon.SetActive(false);
                }
                yield return null;

            }
            else
            {
                Debug.Log("No internet ");
            }

            if (PercentDownloaded.Equals(100))
            {
                //Debug.Log("Finished File Check and Downloads " + Time.time + " Seconds");
                WifiInUseIcon.SetActive(false);

                yield break;
            }

            yield return null;
        }
    }

    IEnumerator UpdateDownloadPercent()
    {
        while (PercentDownloaded < 100)
        {
            if (TotalDownloads > 0)
            {
                PercentDownloaded = (float)((TotalDownloads - DownloadCount) / TotalDownloads) * 100;
                if (PercentDownloaded > 0)
                { }
                else
                {
                    PercentDownloaded = 0;
                }
                percentText.text = PercentDownloaded + "%";
            }
            yield return null;
        }
        yield break;
    }

    private void CheckAndUpdateLinks(string key)
    {
        var CodeToInfoObject = GameObject.Find(key.Replace("Info_Page", "Code_Button"));
        var InfoToCodeObject = GameObject.Find(key.Replace("Info_Page", "Info_Button"));

        // if we have entered passcode previously.
        //If date of passcode entry doesn't check out. we don't change the name
        if (PlayerPrefs.HasKey(key))
        {
            var codeEntered = DateTime.Parse(PlayerPrefs.GetString(key)).ToUniversalTime();

            //Debug.Log("code previously entered " + codeEntered + " now " + DateTime.UtcNow );

            if (codeEntered.AddHours(48).CompareTo(DateTime.UtcNow) < 0)
            {
                try
                {
                    //exceeded time limit. Reactivte code-entry page
                    InfoToCodeObject.name = key.Replace("Info_Page", "Code_Button");
                    InfoToCodeObject.GetComponent<UIB_Button>().Init();
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(NullReferenceException))
                    {

                    }
                }
            }
            else
            {
                //We have access.
                //Change the code page to the info page

                //Debug.Log("THINK WE HAVE ACCESS");
                try
                {
                    CodeToInfoObject.name = key.Replace("Info_Page", "Info_Button");
                    CodeToInfoObject.GetComponent<UIB_Button>().Init();
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(NullReferenceException))
                    {

                    }
                }
            }
            //Swap info button for code button
        }
        else
        {
            try
            {
                //if you do not have the player pref
                //set info page to code page
                InfoToCodeObject.name = key.Replace("Info_Page", "Code_Button");
                InfoToCodeObject.GetComponent<UIB_Button>().Init();
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(NullReferenceException))
                {

                }
            }
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        CheckAndUpdateLinks("Displayed-Info_Page");
        CheckAndUpdateLinks("OnDisplay-Info_Page");
        CheckAndUpdateLinks("Unfinished-Info_Page");

    }

}