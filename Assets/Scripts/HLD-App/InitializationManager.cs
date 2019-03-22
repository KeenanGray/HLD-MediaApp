using System;
using System.Collections;
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

    Color tmpColor;

    public static float DownloadCount = 0;
    public static float TotalDownloads { get; private set; }

    public static int checkingForUpdates = 0;
    public static float PercentDownloaded = 0;

    public TextMeshProUGUI percentText;

    private bool hasAllFiles;


    void Start()
    {
        Debug.Log(Application.persistentDataPath);
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
        UIB_PlatformManager.Init();

        try
        {
            percentText = GameObject.Find("DownloadPercent").GetComponent<TextMeshProUGUI>();
        }
        catch (Exception e)
        {
            Debug.Log("Failed to find GameObject: " + e);
        }

#if UNITY_ANDROID
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

        try
        {
            UAP_AccessibilityManager.PauseAccessibility(true);
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
            //StartCoroutine("Init");
            //yield break;
        }

        hasAllFiles = false;

        aspectManager = GameObject.FindGameObjectWithTag("MainCanvas");
        blankPage = GameObject.Find("BlankPage");

        yield return new WaitForSeconds(1.0f);
        AccessibilityInstructions = GameObject.Find("AccessibleInstructions_Button");
        //enable accessible instructins if plugin is on

        ObjPoolManager.Init();

        //set scroll rects to top
        foreach (Scrollbar sb in GetComponentsInChildren<Scrollbar>())
        {
            sb.value = 1;
        }

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

        //initialize each button
        foreach (UI_Builder.UIB_Button ab in GetComponentsInChildren<UI_Builder.UIB_Button>())
        {
            //before initializing buttons, we may change some names based on player_prefs

            if (ab.name == "DISPLAYED-Code_Button")
                CheckAndUpdateLinks("DISPLAYED-Info_Page");

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
        if (UAP_AccessibilityManager.IsActive())
        {
            AccessibilityInstructions.SetActive(true);
        }
        else
        {
            if (AccessibilityInstructions != null)
            {
                //turn off label and move viewport down.
                var adjust = -1 * AccessibilityInstructions.GetComponent<RectTransform>().rect.height / 2f;
                //                Debug.Log("adjust " + adjust);
                var parent1 = AccessibilityInstructions.transform.parent.parent.parent;
                //                Debug.Log("parent1 " + parent1.name);
                parent1.Translate(new Vector3(0, adjust, 0));
                AccessibilityInstructions.SetActive(false);


            }
            else
                Debug.LogWarning("No accessibility instructions assigned");
        }

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
        var filename = UIB_PlatformManager.platform + "/";

        StartCoroutine("UpdateDownloadPercent");

        TotalDownloads = 8;
        filename = "hld/" + filename;
        //TODO: DeAuth if Default_Code.json is older than 24 hours and doesn't match current code.
        //Next up: Check for "general" asset bundle
        filename = "general";
        filename = "hld/" + filename;
        TryDownloadFile(UIB_PlatformManager.persistantDataPath, UIB_PlatformManager.platform, filename);

        filename = "bios/json";
        filename = "hld/" + filename;
        TryDownloadFile(UIB_PlatformManager.persistantDataPath, UIB_PlatformManager.platform, filename);

        filename = "displayed/narratives/captions";
        filename = "hld/" + filename;
        TryDownloadFile(UIB_PlatformManager.persistantDataPath, UIB_PlatformManager.platform, filename);

        filename = "meondisplay/captions";
        filename = "hld/" + filename;
        TryDownloadFile(UIB_PlatformManager.persistantDataPath, UIB_PlatformManager.platform, filename);

        filename = "bios/photos";
        filename = "hld/" + filename;
        TryDownloadFile(UIB_PlatformManager.persistantDataPath, UIB_PlatformManager.platform, filename);

        filename = "displayed/narratives/photos";
        filename = "hld/" + filename;
        TryDownloadFile(UIB_PlatformManager.persistantDataPath, UIB_PlatformManager.platform, filename);

        filename = "displayed/narratives/audio";
        filename = "hld/" + filename;
        TryDownloadFile(UIB_PlatformManager.persistantDataPath, UIB_PlatformManager.platform, filename);

        /*
         * These files will live in asset bundle on app as well
         * if they can't be downloaded after 60 seconds - fallback to asset bundle        
         * */
        filename = "displayed/audio";
        filename = "hld/" + filename;
        TryDownloadFile(UIB_PlatformManager.persistantDataPath, UIB_PlatformManager.platform, filename, true);



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

    private void TryDownloadFile(string persistantDataPath, string platform, string filename, bool fallbackUsingBundle = false)
    {
        if (!(UIB_FileManager.FileExists(persistantDataPath + platform + filename)))
        {
            if (CheckInternet())
            {
                //Download the file
                Debug.Log("file" + persistantDataPath + platform + filename + " does not exist download commencing download");
                DownloadFileFromDatabase(persistantDataPath + platform, platform + filename, fallbackUsingBundle);
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

        tmpLandingPage = GameObject.Find("NoInternetCriticalLanding");
        */
    }

    private void DownloadFileFromDatabase(string path, string fName, bool fallbackUsingBundle = false)
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
        string persistantDataPath = Application.persistentDataPath + "/heidi-latsky-dance/";

        WifiInUseIcon = GameObject.Find("DownloadIcon");

        while (true)
        {
            if (WifiInUseIcon == null)
            {
                Debug.Log("Bad");
            }

            if (CheckInternet())
            {
                // Debug.Log("We have internet");
                if (DownloadCount > 0 && checkingForUpdates <= 0)
                {
                    //                    Debug.Log("we have downloads going");
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
            }
            if (PercentDownloaded.Equals(100))
            {
                Debug.Log("exiting wifi check after " + Time.time);
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
                percentText.text = PercentDownloaded + "%";
            }
            yield return null;
        }
        yield break;
    }



    private void CheckAndUpdateLinks(string key)
    {
        // if we have entered passcode previously.
        //If date of passcode entry doesn't check out. we don't change the name
        if (PlayerPrefs.HasKey(key))
        {
            var codeEntered = DateTime.Parse(PlayerPrefs.GetString(key));
            
            if (codeEntered.AddHours(48).CompareTo(DateTime.UtcNow) < 0)
            {
              //exceeded time limit. Reactivte code-entry page
                try
                {
                    var gb = GameObject.Find(key.Replace("-Info_Page", "-Info_Button"));
                    gb.name = key.Replace("Info_Page", "Code_Button");
                    gb.GetComponent<UIB_Button>().Init();
                    //PlayerPrefs.DeleteKey(key);
                }
                catch (Exception e)
                {
                    Debug.Log(" we haven't updated the code-button, no info-button found" + e );
                }
            }
            else
            {
                Debug.Log("we have access " + PlayerPrefs.GetString(key));
                //We have access.
                //Change the code page to the info page
                try
                {
                    var gb = GameObject.Find(key.Replace("-Info_Page", "-Code_Button"));
                    gb.name = key.Replace("Info_Page", "Info_Button");
                    gb.GetComponent<UIB_Button>().Init();

                }
                catch (Exception e)
                {
                    Debug.Log("We have probably already updated the code-button to the info-button" + e);
                }

            }

            //Swap info button for code button
        }
    }

    private void OnApplicationFocus(bool focus)
    {
       CheckAndUpdateLinks("DISPLAYED-Info_Page");
    }

}