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

    HLD.Database_Accessor db_Manager;
    public float InitializeTime;
    float t1;
    float t2;

    private GameObject NoWifi;
    Color tmpColor;
    private GameObject tmpLandingPage;

    void Start()
    {
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

        //on android we must check for OBB file

        if (GooglePlayDownloader.RunningOnAndroid())
        {
            Debug.Log("Running on Android");
        }

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

        switch (ManageDatabaseFiles())
        {
            case (DatabaseResult.SUCCESS):
                Debug.Log("SUCCESS");
                break;
            case (DatabaseResult.FAILURE):
                Debug.LogError("FAILURE");
                break;
            case (DatabaseResult.NOCONNECTION):
                Debug.LogWarning("NO CONNECTION");
                break;
            case (DatabaseResult.RETRY):
                Debug.Log("RETRY");
                break; ;
            default:
                break;
        }

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

    private DatabaseResult ManageDatabaseFiles()
    {
        //First check if we have local versions of the files
        if (CheckForLocalFiles())
        {
            //if we have local files, see if we have an internet connection
            if (CheckInternet())
            {
                //If we have internet, compare versions of each files
                UpdateFilesIfNecessary();
                return DatabaseResult.SUCCESS;
            }
            else
            {
                //No internet, we will continue with app based on most recent version.
                //This function will add global UI button (no internet indicator) - click button to attempt an update
                ActivateNoInternetMode();
                return DatabaseResult.NOCONNECTION;
            }
        }
        else
        {
            //The app is missing one or all local files. 
            //Check for internet
            if (CheckInternet())
            {
                //We have internet, 
                //TODO: alert the user that we will be downloading data
                DownloadFilesFromDatabase();
                return DatabaseResult.RETRY;
            }
            else
            {
                //We do not have internet
                //Alert the user that the app will have very limited functionality until connected to the internet
                ActivateLimitedFunctionality();
                return DatabaseResult.NOCONNECTION;
            }
        }
        return DatabaseResult.SUCCESS;
    }

    private void ActivateLimitedFunctionality()
    {
        //Bring up no internet logo. 
        UIB_PageManager.InternetActive = false;
        NoWifi.GetComponentInChildren<Image>().color = new Color(tmpColor.r, tmpColor.g, tmpColor.b, 130);
        NoWifi.GetComponent<Button>().interactable = true;

        tmpLandingPage = GameObject.Find("NoInternetCriticalLanding");
    }

    private void DownloadFilesFromDatabase()
    {
        //TODO: Alert the user we are about to begin a large download
        Debug.LogWarning("Fetching Downloads from Database: If your are testing, it's possible a file is missing");
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
        SetupArrayOfInterest();
        db_Manager.GetUpdatedObjects("hld-general");
        db_Manager.GetUpdatedObjects("hld-displayed");
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

    private bool CheckForLocalFiles()
    {
        string persisantDataPath = Application.persistentDataPath + "/";

        //check for relevant json files
        //Here we are checking for Bios.json (containing dancer biographies) and the AccessCode for DISPLAYED section
        if (!(FileManager.FileExists("hld-general/Bios.json") && FileManager.FileExists("hld-displayed/AccessCode.json")))
            return false;

        //TODO: DeAuth if Default_Code.json is older than 24 hours and doesn't match current code.

        string[] ListOfDancers = null;
        //Extract the list of dancers
        //This is the list we are concerned about in the app
        //This list can then be updated in real time to remove and add dancers to the app

        //We will then ensure we have a picture for each dancer
        ListOfDancers = GetListOfDancers();

        if (ListOfDancers == null)
            return false;

        ///Get the picture file for each bio
        var path = "/hld-general/Bio_Photos/";
        foreach (string dancer in ListOfDancers)
        {
            var filePath = path + dancer.Replace(" ", "_");
            if (FileManager.FileExists(filePath + ".png") || FileManager.FileExists(filePath + ".jpg") || FileManager.FileExists(filePath + ".jpeg"))
            {
                //                    Debug.Log("File exists: " + filePath );
            }
            else
            {
                Debug.Log("Bio picture file does not exist: " + filePath + ".");
                return false;
            }
        }

        //Next up: Check for the Audio Description
        path = "/hld-general/AudioDescriptions/Displayed_AudioDescriptions.mp3";
        if (FileManager.FileExists(path))
        { }
        else
        {
            Debug.Log("/hld-general/AudioDescriptions/Displayed_AudioDescriptions.mp3 not found");
            return false;
        }

        #region MeOnDisplay
        //Next check #MeOnDisplay Videos
        path = "/hld-general/MeOnDisplay/";
        foreach (string dancer in ListOfDancers)
        {
            var filePath = path + dancer.Replace(" ", "_");
            if (FileManager.FileExists(filePath + ".mov") || FileManager.FileExists(filePath + ".mp4"))
            {
            }
            else
            {
                Debug.Log("Video file does not exist: " + filePath + ".");
                return false;
            }
        }
        // and Video Captions
        path = "/hld-general/MeOnDisplay/VideoCaptions/";
        foreach (string dancer in ListOfDancers)
        {
            var filePath = path + dancer.Replace(" ", "_");
            if (FileManager.FileExists(filePath + ".txt"))
            {
            }
            else
            {
                Debug.Log("Video Captions file does not exist: " + filePath + ".");
                return false;
            }
        }
        #endregion

        #region Displayed Files
        //Next check DISPLAYED audio
        path = "/hld-displayed/Audio/";
        foreach (string dancer in ListOfDancers)
        {
            var filePath = path + dancer.Replace(" ", "_");
            if (FileManager.FileExists(filePath + ".wav") || FileManager.FileExists(filePath + ".mp3"))
            {
            }
            else
            {
                Debug.Log("Displayed Audio file does not exist: " + filePath + ".");
                return false;
            }
        }
        //Next check #MeOnDisplay Videos
        path = "/hld-displayed/Captions/";
        foreach (string dancer in ListOfDancers)
        {
            var filePath = path + dancer.Replace(" ", "_");
            if (FileManager.FileExists(filePath + ".txt"))
            {
            }
            else
            {
                Debug.Log("Captions file does not exist: " + filePath + ".");
                return false;
            }
        }
        //Next check #MeOnDisplay Videos
        path = "/hld-displayed/Photos/";
        foreach (string dancer in ListOfDancers)
        {
            var filePath = path + dancer.Replace(" ", "_");
            if (FileManager.FileExists(filePath + ".png") || FileManager.FileExists(filePath + ".jpg") || FileManager.FileExists(filePath + ".jpeg"))
            {
            }
            else
            {
                Debug.Log("#MeOnDisplay photo file does not exist: " + filePath + ".");
                return false;
            }
        }
        #endregion

        //If we get here, we ahve all the files we want
        return true;
    }

    private string[] GetListOfDancers()
    {
        string[] list;
        if (FileManager.FileExists("hld-general/ListOfDancers.txt"))
        {
            list = FileManager.ReadTextFile("hld-general/ListOfDancers.txt").Split(',');
        }
        else
        {
            Debug.LogWarning("No list of dancers");
            return null;
        }

        list = list.OrderBy(x => x).ToArray();
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

    /*  void InitializePlaceHolderJsonFiles()
       {
           Debug.Log(Application.persistentDataPath);

           TextAsset code = Resources.Load<TextAsset>("Json/AccessCode_default");
           TextAsset bios = Resources.Load<TextAsset>("Json/Bios_default");

           MongoLib.WriteJsonUnModified(bios.text, "Bios.json");
           MongoLib.WriteJsonUnModified(code.text, "AccessCode.json");

       }
       */

}

