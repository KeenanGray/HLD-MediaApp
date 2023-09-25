using System;
using System.Collections;
using System.IO;
using System.Linq;
using HLD;
using TMPro;
using UI_Builder;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;

public class InitializationManager : MonoBehaviour
{
    GameObject aspectManager;

    GameObject AccessibilityInstructions;

    GameObject blankPage;

    Database_Accessor db_Manager;

    public static float InitializeTime;

    float t1;

    float t2;

    public static float DownloadCount = 0;

    public static float TotalDownloads { get; private set; }

    public static int checkingForUpdates = 0;

    public static float PercentDownloaded = 0;

    public static bool hasUpdatedFiles = false;
    bool wroteToPersistant = false;

    public bool DebugLocalAssetBundles;

    public TextMeshProUGUI percentText;

    public static bool doneLoadingAssetBundles = false;

    void Start()
    {
        InitializationManager.hasUpdatedFiles = false;
        InitializeTime = 0;

#if (!UNITY_EDITOR)
        DebugLocalAssetBundles = false;
#endif

#if UNITY_EDITOR
        UIB_AspectRatioManager_Editor.Instance().IsInEditor = false;
        UpdateNameOfTextItem.ShouldRun = false;
#endif

        StartCoroutine("Init");
    }

    private void Update()
    {
        if (UIB_FileManager.HasUpdatedAFile && DownloadCount <= 0)
        {
            PlayerPrefs.SetString("LastUpdated", DateTime.UtcNow.ToString());
            UIB_FileManager.HasUpdatedAFile = false;
        }
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
            if (e.GetType() == typeof(NullReferenceException)) { }
        }

        //this player pref should get set at app launch so that it resets the timecode in the audio-desc;
        PlayerPrefs.SetInt("desc_timecode", 0);

        #region GameObjectAssignment

        try
        {
            db_Manager = GameObject.Find("DB_Manager").GetComponent<HLD.Database_Accessor>();
        }
        catch (Exception e)
        {
            Debug.LogWarning("No database manager" + e);
            yield break;
        }
        //when we init the db_manager the first time we might require a reload
        try
        {
            blankPage = GameObject.Find("BlankPage");
        }
        catch (Exception e)
        {
            Debug.LogWarning("No blankpage " + e);
            yield break;
        }
        try
        {
            aspectManager = GameObject.FindGameObjectWithTag("MainCanvas");
        }
        catch (Exception e)
        {
            Debug.LogWarning("no aspect ratio manager " + e);
            yield break;
        }

        try
        {
            AccessibilityInstructions = GameObject.Find("AccessibleInstructions_Button");
        }
        catch (Exception e)
        {
            Debug.LogWarning("no instructions " + e);
            yield break;
        }
        #endregion

        // checks the local files and starts any necessary downloads
        //if we are on ios and this is the first launch, ignore this (keenan what does this comment mean? 9/23/2023 ????????)
        CheckLocalFiles();

        var loading_text = GameObject.Find("LoadingText").GetComponent<TextMeshProUGUI>();

        while (DownloadCount > 0)
        {
            loading_text.text = "Loading Files..." + (DownloadCount - 1) + " Files Remaining";
            yield return null;
        }

        //setup checks for accessibility on android - which is wierd;
#if UNITY_ANDROID && !UNITY_EDITOR
        if (UAP_AccessibilityManager.GetAndroidAccessibility())
        {
            UAP_AccessibilityManager.EnableAccessibility(true);
        }
        else
        {
            UAP_AccessibilityManager.EnableAccessibility(false);
        }
#endif


        #region initialize UI
        //Set the main page container
        //Can't remember why i did this
        UIB_PageContainer MainContainer = GameObject.Find("Pages").GetComponent<UIB_PageContainer>();
        MainContainer.Init();

        //set scroll rects to top
        foreach (Scrollbar sb in GameObject.FindObjectsOfType<Scrollbar>())
        {
            sb.value = 1;
        }

        //turn aspect ratio fitters on
        //causes all pages to share origin with canvas and be correct dimensions
        foreach (AspectRatioFitter arf in GameObject.FindObjectsOfType<AspectRatioFitter>())
        {
            arf.aspectRatio = (UIB_AspectRatioManager.ScreenWidth) / (UIB_AspectRatioManager.ScreenHeight);
            arf.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            arf.enabled = true;
        }

        //initialize each button
        foreach (UI_Builder.UIB_Button ab in GameObject.FindObjectsOfType<UI_Builder.UIB_Button>())
        {
            //before initializing buttons, we may change some names based on player_prefs
            if (ab.name == "Displayed-Code_Button")
                CheckAndUpdateLinks("Displayed-Info_Page");
            if (ab.name == "OnDisplay-Code_Button")
                CheckAndUpdateLinks("OnDisplay-Info_Page");
            if (ab.name == "Unfinished-Code_Button")
                CheckAndUpdateLinks("Unfinished-Info_Page");
            if (ab.name == "SoloFlight-Code_Button")
                CheckAndUpdateLinks("SoloFlight-Info_Page");
            ab.Init();
        }

        //initialize objects in the object pools
        //todo:tag this for eventual replacement with better pages/buttons
        ObjPoolManager.Init();
        ObjPoolManager.RefreshPool();

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

        //initialize each scrolling menu
        foreach (UIB_ScrollingMenu uibSM in GetComponentsInChildren<UIB_ScrollingMenu>())
        {
            uibSM.Init();
        }

        if (UAP_AccessibilityManager.IsEnabled())
        {
            //unpause accessibility manger to read first button
            UAP_AccessibilityManager.PauseAccessibility(false);
            if (UAP_AccessibilityManager.IsActive())
            {
                // AccessibilityInstructions.SetActive(true);
                UAP_AccessibilityManager.EnableAccessibility(false);
                UAP_AccessibilityManager.EnableAccessibility(true);
            }
            else { }

            //select the first button with UAP
            //var first = GameObject.Find("DISPLAYED-Code_Button");
            //UAP_AccessibilityManager.SelectElement(first, true);
        }
        //setup the first screen
        var firstScreen = GameObject.Find("Landing_Page");
        firstScreen.GetComponent<UIB_Page>().StartCoroutine("MoveScreenIn", true);
        #endregion

        GameObject.Find("MainCanvas").GetComponent<UIB_AssetBundleHelper>().StartCoroutine("LoadAssetBundlesInBackground");

        yield return new WaitForSeconds(.25f);

        //if we are finally done loading everything, then we can remove the cover
        while (!doneLoadingAssetBundles)
        {
            yield return null;
        }

        //remove the cover
        MainContainer.DisableCover();
        //Remove the loading Text
        GameObject.Find("LoadingText").SetActive(false);

        //if we finish initializing faster than expected, take a moment to finish the video
        t2 = Time.time;
        var elapsed = t2 - t1;
        InitializeTime = elapsed;

        yield break;
    }

    private void CheckLocalFiles()
    {
        //if we copied files from streaming assets, this means a fresh install
        //we should reload the scene so the files load from the correct place
        //and the internet files are downloaded on first run.
        if (wroteToPersistant)
        {
            Debug.Log("Loading reload scene");
            SceneManager.LoadScene(1);
        }

        //check for relevant asset bundle files
        //First check that platform specific assetbundle exists
        var filename = UIB_PlatformManager.platform + "/";

        //DEAR KEENAN FROM THE FUTURE ----> REMEMBER TO CHANGE THIS # if you add a show.
        TotalDownloads = 13;

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

        //ufinished section
        filename = "unfinished/audio";
        filename = "hld/" + filename;
        TryDownloadFile(filename);

        //soloflight section
        filename = "sora/audio";
        filename = "hld/" + filename;
        TryDownloadFile(filename);
        filename = "suspendeddisbelief/audio";
        filename = "hld/" + filename;
        TryDownloadFile(filename);
    }

    private async void TryDownloadFile(string filename, bool fallbackUsingBundle = false)
    {
        string filepath =
            UIB_PlatformManager.persistentDataPath + UIB_PlatformManager.platform + filename;
        UIB_AssetBundleHelper.InsertAssetBundle(filename);

#if UNITY_ANDROID && !UNITY_EDITOR
        filepath =UIB_PlatformManager.persistentDataPath+ "android/assets/"+ UIB_PlatformManager.platform+ filename;
#endif

        if (!UIB_FileManager.FileExists(filepath)) //if the file does not exist we copy the stored version for peristant storage
        {
            // we need a special version of this function in order to check streaming assets for Android
#if UNITY_ANDROID && !UNITY_EDITOR
            //We don't have the file, first thing is to copy it from streaming assets
            //On Android, streaming assets are zipped so we need a special accessor
            Debug.LogWarning("file does not exist");
            GameObject.Find("FileManager").GetComponent<UIB_FileManager>().StartCoroutine("CreateStreamingAssetDirectories", filename);
#else
            //we don't have the file, first thing to do is copy it from streaming assets
            UIB_FileManager.WriteFromStreamingToPersistent(filename);
#endif
            //record here that we have never updated files from the internet;
            PlayerPrefs.SetString(
                "LastUpdated",
                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToString()
            );

            if (CheckInternet() && !DebugLocalAssetBundles)
            {
                InitializationManager.DownloadCount++;

                await db_Manager.CheckIfObjectHasUpdate(
                    UIB_PlatformManager.platform + filename,
                    "heidi-latsky-dance"
                );
            }
            wroteToPersistant = true;
        }
        else
        {
            //we have the file already so we check for an update
            if (CheckInternet() && !DebugLocalAssetBundles)
            {
                InitializationManager.DownloadCount++;

                await db_Manager.CheckIfObjectHasUpdate(
                    UIB_PlatformManager.platform + filename,
                    "heidi-latsky-dance"
                );
            }
        }
        //delete the streaming asset files
        UIB_FileManager.DeleteFile(filename);
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


    private void CheckAndUpdateLinks(string key)
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("LockedPageButton"))
        {
            GameObject CodeToInfoObject = null;
            if (go.name == key.Replace("Info_Page", "Code_Button"))
            {
                CodeToInfoObject = go;
            }
            GameObject InfoToCodeObject = null;
            if (go.name == key.Replace("Info_Page", "Info_Button"))
            {
                InfoToCodeObject = go;
            }

            // if we have entered passcode previously.
            //If date of passcode entry doesn't check out. we don't change the name
            if (PlayerPrefs.HasKey(key))
            {
                var codeEntered = DateTime.Parse(PlayerPrefs.GetString(key)).ToUniversalTime();

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
                        if (e.GetType() == typeof(NullReferenceException)) { }
                    }
                }
                else
                {
                    //We have access.
                    //Change the code page to the info page
                    try
                    {
                        CodeToInfoObject.name = key.Replace("Info_Page", "Info_Button");
                        CodeToInfoObject.GetComponent<UIB_Button>().Init();
                    }
                    catch (Exception e)
                    {
                        if (e.GetType() == typeof(NullReferenceException)) { }
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
                    if (e.GetType() == typeof(NullReferenceException)) { }
                }
            }
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        CheckAndUpdateLinks("Displayed-Info_Page");
        CheckAndUpdateLinks("OnDisplay-Info_Page");
        CheckAndUpdateLinks("Unfinished-Info_Page");
        CheckAndUpdateLinks("SoloFlight-Info_Page");
    }
}
