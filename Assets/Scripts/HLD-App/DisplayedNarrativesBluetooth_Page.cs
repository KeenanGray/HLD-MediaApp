using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UI_Builder;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DisplayedNarrativesBluetooth_Page : MonoBehaviour, UIB_IPage
{
    public List<string> DancerMajorsList;
    //these values need to be incremented by 1 in the bluetooth setting
    /*
      enum DancerMajors
      {
          Chris_Braz,
          Desmond_Cadogan,
          Victoria_Dombroski,
          Meredith_Fages,
          Tiffany_Geigel,
          Nico_Gonzales,
          Jerron_Herman,
          Jillian_Hollis,
          Donald_Lee,
          Louisa_Mann,
          Amy_Meisner,
          Kelly_Ramis,
          Jaclyn_Rea,
          Carmen_Schoenster,
          Tianshi_Suo,
          Leslie_Taub,
          Peter_Trojic
      }
      */
    GameObject GoToListBtn;
    iBeaconReceiver beaconr;
    private List<Beacon> mybeacons;

    Dictionary<string, GameObject> AudioPlayers;
    GameObject AudioPlayerPrefab;

    string PageName = "";
    string ListName = "";
    bool PlayMultiple;

    string ToggleStartingText = "";

    public void Init()
    {
        beaconr = GetComponent<iBeaconReceiver>();
        mybeacons = new List<Beacon>();
        iBeaconReceiver.BeaconRangeChangedEvent += OnBeaconRangeChanged;
        dancersDetected = new Dictionary<int, int>();

        UIB_AssetBundleHelper.InsertAssetBundle("hld/general");
        UIB_AssetBundleHelper.InsertAssetBundle("hld/"+ ShowName.ToLower()+"/narratives/photos");
        UIB_AssetBundleHelper.InsertAssetBundle("hld/"+ ShowName.ToLower()+"/narratives/captions");
        UIB_AssetBundleHelper.InsertAssetBundle("hld/"+ ShowName.ToLower()+"/narratives/audio");

        PageName = name.Split('-')[0] + "-NarrativesBT_Page";
        ListName = name.Split('-')[0] + "-NarrativesList_Page";


#if UNITY_EDITOR
        EditorApplication.pauseStateChanged += LogPauseState;
#endif

#if TARGET_IPHONE_SIMULATOR
        BluetoothState.BluetoothStateChangedEvent += delegate (BluetoothLowEnergyState state)
        {
            switch (state)
            {
                case BluetoothLowEnergyState.TURNING_OFF:
                case BluetoothLowEnergyState.TURNING_ON:
                    break;
                case BluetoothLowEnergyState.UNKNOWN:
                    Debug.Log("status is unknown");
                    break;
                case BluetoothLowEnergyState.RESETTING:
                    break;
                case BluetoothLowEnergyState.UNAUTHORIZED:
                    break;
                case BluetoothLowEnergyState.UNSUPPORTED:
                    break;
                case BluetoothLowEnergyState.POWERED_OFF:
                    break;
                case BluetoothLowEnergyState.POWERED_ON:
                    break;
                case BluetoothLowEnergyState.IBEACON_ONLY:
                    break;
                default:
                    break;
            }

        };

#endif
        StartCoroutine("BeaconUpdateCoroutine");
        GetComponent<UIB_Page>().OnActivated += PageActivatedHandler;
        GetComponent<UIB_Page>().OnDeActivated += PageDeActivatedHandler;

        var tmp = GameObject.Find("CameraViewTexture");

        foreach (UIB_Button button in GetComponentsInChildren<UIB_Button>())
        {
            if (button.name == "DISPLAYED-Info_Button")
            {
                button.GetComponent<Button>().onClick.AddListener(delegate
                {
                    GameObject.Find(ListName).GetComponent<Canvas>().enabled = false;
                });
            }
        }

        AudioPlayers = new Dictionary<string, GameObject>();
        AudioPlayerPrefab = Resources.Load("BluetoothAudioSource") as GameObject;

        StartCoroutine("StartupBluetoothService");
    }

    public void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            WhenApplicationPauses();
        }
        else
        {
            WhenApplicationUnpauses();
        }
    }

#if UNITY_EDITOR
    private void LogPauseState(PauseState state)
    {
        Debug.Log(state);

        if (state == PauseState.Paused)
        {
            //spin up background thread
            WhenApplicationPauses();
        }
        else if (state == PauseState.Unpaused)
        {
            //return to main thread
            WhenApplicationUnpauses();
        }
    }
#endif

    void WhenApplicationPauses()
    {
        //await new  WaitForUpdate();
        //await new WaitForBackgroundThread();
        //for (int i = 0; i < 100; i++)
        //{
        //    Debug.Log("Scanning In Background");
        //    await Task.Delay(TimeSpan.FromSeconds(1.0f));
        //}
        //await new WaitForSeconds(1.0f);
    }

    void WhenApplicationUnpauses()
    {
        //await new WaitForUpdate();
        //await new WaitForUpdate();
        //Debug.Log("Finished In Background");
    }

    public void PageActivatedHandler()
    {
        AssetBundle tmp = null;
        foreach (AssetBundle b in AssetBundle.GetAllLoadedAssetBundles())
        {
            if (b.name == "hld/general")
                tmp = b;
        }

        if (tmp != null)
        {
            var dancers = tmp.LoadAsset<TextAsset>("ListOfDancers") as TextAsset;

            if (dancers.ToString().Split(',').Length != DancerMajorsList.Count)
                foreach (String s in dancers.ToString().Split(','))
                {
                    var corrected = s.Replace("\n", "");
                    corrected = corrected.Replace("\r", "");
                    corrected = corrected.Replace(" ", "");
                    DancerMajorsList.Add(corrected);
                }
            else
            {

            }
            //src.time = 0;
        }

        GetComponent<Canvas>().enabled = true;

        if (UIB_PageManager.LastPage.name != ListName)
        {
        }

        var page = GameObject.Find(ListName).GetComponent<UIB_Page>();
        page.GetComponent<UIB_Page>().StartCoroutine("MoveScreenIn", false);

        page.OnActivated += myDelegate();
        page.OnDeActivated += myDeactivatedDelegate();

        page.GetComponent<UIB_Page>().Init();
        page.GetComponent<UIB_Page>().StartCoroutine("MoveScreenIn", false);
        page.OnActivated -= myDelegate();

        try
        {
            iBeaconReceiver.Scan();
        }
        catch
        {
            iBeaconReceiver.Scan();
        }

        StartCoroutine(GetComponent<UIB_Page>().ResetUAP(true));
    }

    internal void StopPlaying(BluetoothAudioSource bluetoothAudioSource)
    {
        var key = bluetoothAudioSource.name.Replace("_Bluetooth_Audio", "");
        //Debug.Log(key);
        AudioPlayers.Remove(key);
        Destroy(bluetoothAudioSource.gameObject);
    }

    private UIB_Page.DeActivated myDeactivatedDelegate()
    {
        return delegate
        {
            var page = GameObject.Find(ListName).GetComponent<UIB_Page>();
            page.OnDeActivated -= myDeactivatedDelegate();

            StopBTAudio();
        };

    }

    public void StopBTAudio()
    {
        iBeaconReceiver.Stop();

        foreach (GameObject go in AudioPlayers.Values)
        {
            Destroy(go);
        }

        AudioPlayers.Clear();
        mybeacons.Clear();
    }

    private UIB_Page.Activated myDelegate()
    {
        return delegate
        {
            UIB_PageManager.CurrentPage = GameObject.Find(PageName);
            var page = GameObject.Find(ListName).GetComponent<UIB_Page>();
        };
    }

    public void PageDeActivatedHandler()
    {
        try
        {
            GameObject.Find(ListName).GetComponent<Canvas>().enabled = false;
            GameObject.Find(ListName).GetComponent<UIB_Page>().StartCoroutine("MoveScreenOut", false);
        }
        catch (Exception e)
        {
            if (e.GetType() == typeof(NullReferenceException))
            {

            }
        }
        StopBTAudio();

    }

    // Use this for initialization
    void Start()
    {
        ShowName = gameObject.name.Split('-')[0] + "-";

        DancerMajorsList = new List<string>();
        GoToListBtn = GameObject.Find(ShowName + "ListOfDancersButton");
        GoToListBtn.GetComponent<Button>().onClick.AddListener(GoToList);

        ToggleButton = GameObject.Find(ShowName + "ToggleMultipleButton");
        ToggleButton.GetComponent<Button>().onClick.AddListener(ToggleMultiple);

        OnOff = GameObject.Find(ShowName + "ToggleMultipleOnOff");

        OnOff.transform.Find("ToggleOff").gameObject.SetActive(false);
        OnOff.transform.Find("ToggleOn").gameObject.SetActive(true);

        ToggleStartingText = ToggleButton.GetComponentInChildren<TextMeshProUGUI>().text;
        var uap_T = GameObject.Find(ShowName + "ToggleMultipleButton").GetComponentInParent<Special_AccessibleButton>();
        uap_T.m_Text = ToggleStartingText + " On ";

        PlayMultiple = true;
    }

    public GameObject ToggleButton { get; private set; }
    public string ShowName { get; private set; }

    private void ToggleMultiple()
    {
        PlayMultiple = !PlayMultiple;
        OnOff.transform.Find("ToggleOn").gameObject.SetActive(PlayMultiple);
        OnOff.transform.Find("ToggleOff").gameObject.SetActive(!PlayMultiple);

        var t = GameObject.Find(ShowName + "ToggleMultipleButton").GetComponentInChildren<TextMeshProUGUI>();
        var uap_T = GameObject.Find(ShowName + "ToggleMultipleButton").GetComponentInParent<Special_AccessibleButton>();

        if (PlayMultiple)
        {
            t.text = ToggleStartingText;
            uap_T.m_Text = ToggleStartingText + " On ";
        }
        else
        {
            uap_T.m_Text = ToggleStartingText + " Off ";
        }

        UAP_AccessibilityManager.SelectElement(UAP_AccessibilityManager.GetCurrentFocusObject(), true);
    }

    void GoToList()
    {
        GetComponent<Canvas>().enabled = false;
        UIB_PageManager.CurrentPage = GameObject.Find(ListName);
        UIB_PageManager.LastPage = GameObject.Find(ListName);

        StartCoroutine(GetComponent<UIB_Page>().ResetUAP(false));
        StartCoroutine(UIB_PageManager.CurrentPage.GetComponent<UIB_Page>().ResetUAP(true));

        iBeaconReceiver.Stop();
    }

    IEnumerator InitializeRecognizer()
    {
        yield break;
    }

    void OnBeaconRangeChanged(Beacon[] beacons)
    {
        foreach (Beacon b in beacons)
        {
            var index = mybeacons.IndexOf(b);
            if (index == -1)
            {
                mybeacons.Add(b);
            }
            else
            {
                mybeacons[index] = b;
            }
        }
        for (int i = mybeacons.Count - 1; i >= 0; --i)
        {
            if (mybeacons[i].lastSeen.AddSeconds(10) < DateTime.Now)
            {
                mybeacons.RemoveAt(i);
            }
        }

        //  CheckBeaconsForDistance();
    }


    //simulate in editor with update
#if UNITY_EDITOR

#endif

    private void Update()
    {

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.T) && UIB_PageManager.CurrentPage == gameObject)
        {
            for (int i = 1; i < DancerMajorsList.Count; i++)
            {
                var DancerFromBeacon = DancerMajorsList[i - 1];

                if (i == 10)
                {
                    if (AudioPlayers.ContainsKey(DancerFromBeacon))
                    {
                        //we already have a player set up for that dancer, let's bring up the volume.
                        AudioPlayers[DancerFromBeacon].GetComponent<BluetoothAudioSource>().knownRSSI = .9f;
                        playBeacon(.9f, DancerFromBeacon);
                    }
                    else
                    {
                        //We haven't made a gameobject for that dancer, make it and add it to the list
                        InstantiateBlueToothObject(DancerFromBeacon);

                    }
                }

                else if (i == 2)
                {
                    if (AudioPlayers.ContainsKey(DancerFromBeacon))
                    {
                        //we already have a player set up for that dancer, let's bring up the volume.
                        try
                        {
                            AudioPlayers[DancerFromBeacon].GetComponent<BluetoothAudioSource>().knownRSSI = .25f;
                        }
                        catch (Exception e)
                        {
                            //if the clip finshed playing we deleted it.
                            //Have to remove the obejct from this list
                            if (e.GetType() == typeof(MissingReferenceException))
                            {
                                Debug.Log("got it");
                                AudioPlayers.Remove(DancerFromBeacon);
                            }

                        }
                        playBeacon(.6f, DancerFromBeacon);
                    }
                    else
                    {
                        //We haven't made a gameobject for that dancer, make it and add it to the list
                        InstantiateBlueToothObject(DancerFromBeacon);

                    }
                }
                else if (i == 6)
                {
                    if (AudioPlayers.ContainsKey(DancerFromBeacon))
                    {
                        //we already have a player set up for that dancer, let's bring up the volume.
                        AudioPlayers[DancerFromBeacon].GetComponent<BluetoothAudioSource>().knownRSSI = .3f;
                        playBeacon(.8f, DancerFromBeacon);
                    }
                    else
                    {
                        //We haven't made a gameobject for that dancer, make it and add it to the list
                        InstantiateBlueToothObject(DancerFromBeacon);

                    }
                }
            }
        }
        if (Input.GetKey(KeyCode.K))
        {
            var DancerFromBeacon = DancerMajorsList[2];
            var DancerFromBeacon2 = DancerMajorsList[1];

            DisableAudioPlayer(AudioPlayers[DancerFromBeacon2], DancerFromBeacon2);

        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            var DancerFromBeacon1 = DancerMajorsList[0];
            var DancerFromBeacon5 = DancerMajorsList[4];
            var DancerFromBeacon13 = DancerMajorsList[12];

            DisableAudioPlayer(AudioPlayers[DancerFromBeacon1], DancerFromBeacon1);
            DisableAudioPlayer(AudioPlayers[DancerFromBeacon5], DancerFromBeacon5);
            DisableAudioPlayer(AudioPlayers[DancerFromBeacon13], DancerFromBeacon13);

        }

#endif

    }

    void CheckBeaconsForDistance()
    {
        var minImmediateVol = .4f;
        var maxImmediatevol = 1f;

        var minImmediateRSSI = -61;
        var maxImmediateRSSI = -30;

        var minFarRSSI = -50;
        var maxFarRSSI = -30;
#if !UNITY_EDITOR

        if (mybeacons == null)
            return;

        if (mybeacons.Count <= 0)
            return;
#endif

        var printDebug = "";
        foreach (Beacon b in mybeacons)
        {
            var DancerFromBeacon = DancerMajorsList[b.major - 1];
            var absRSSI = Math.Abs(b.rssi);

            printDebug += "Beacon " + DancerFromBeacon + " rssi:" + b.rssi + " acc:" + b.accuracy + ":" + b.range + "\n";

            /*
            if (b.range != BeaconRange.UNKNOWN)
            {
                if (AudioPlayers.ContainsKey(DancerFromBeacon))
                {
                    if (AudioPlayers[DancerFromBeacon].GetComponent<AudioSource>().isPlaying)
                    {
                        playBeacon(AudioPlayers[DancerFromBeacon].GetComponent<AudioSource>().volume - 0.05f, DancerFromBeacon);
                        AudioPlayers[DancerFromBeacon].GetComponent<BluetoothAudioSource>().IncrementUnknown();

                        if (AudioPlayers[DancerFromBeacon].GetComponent<BluetoothAudioSource>().getUnknownCount() > 20)
                        {
                           AudioPlayers[DancerFromBeacon].SetActive(false);
                        }
                    }
                }
            }
        */

            var accuracyLimit = 2.1f;

            if (b.range == BeaconRange.IMMEDIATE || b.range == BeaconRange.NEAR)
            {
                if (AudioPlayers.ContainsKey(DancerFromBeacon))
                {
                    //we already have a player set up for that dancer, let's bring up the volume.
                    if (b.accuracy < accuracyLimit)
                    {
                        //turn on player
                        if (CheckBeaconToStart(AudioPlayers[DancerFromBeacon]))
                        {
                            AudioPlayers[DancerFromBeacon].GetComponent<BluetoothAudioSource>().knownRSSI = (AudioPlayers[DancerFromBeacon].GetComponent<BluetoothAudioSource>().knownRSSI + b.rssi) / 2;
                            playBeacon(HLD.Utilities.Map(b.rssi, minImmediateRSSI, maxImmediateRSSI, minImmediateVol, maxImmediatevol), DancerFromBeacon);
                        }
                    }
                    else
                    {
                        if (CheckBeaconToEnd(AudioPlayers[DancerFromBeacon]))
                        {
                            DisableAudioPlayer(AudioPlayers[DancerFromBeacon], DancerFromBeacon);
                            Debug.Log("Deleting at near");
                        }
                    }
                }
                else
                {
                    //We haven't made a gameobject for that dancer, make it and add it to the list
                    InstantiateBlueToothObject(DancerFromBeacon);
                    AudioPlayers[DancerFromBeacon].GetComponent<BluetoothAudioSource>().knownRSSI = (AudioPlayers[DancerFromBeacon].GetComponent<BluetoothAudioSource>().knownRSSI + b.rssi) / 2;
                    playBeacon(HLD.Utilities.Map(b.rssi, minImmediateRSSI, maxImmediateRSSI, minImmediateVol, maxImmediatevol), DancerFromBeacon);
                }
            }

            if (b.range == BeaconRange.FAR)
            {
                if (AudioPlayers.ContainsKey(DancerFromBeacon))
                {
                    if (b.accuracy < accuracyLimit)
                    {
                        //turn on beacon
                        if (CheckBeaconToStart(AudioPlayers[DancerFromBeacon]))
                        {
                            AudioPlayers[DancerFromBeacon].GetComponent<BluetoothAudioSource>().knownRSSI = (AudioPlayers[DancerFromBeacon].GetComponent<BluetoothAudioSource>().knownRSSI + b.rssi) / 2;
                            playBeacon(HLD.Utilities.Map(b.rssi, minFarRSSI, maxFarRSSI, minImmediateVol, maxImmediatevol), DancerFromBeacon);
                        }
                        //
                    }
                    else
                    {
                        if (CheckBeaconToEnd(AudioPlayers[DancerFromBeacon]))
                        {
                            DisableAudioPlayer(AudioPlayers[DancerFromBeacon], DancerFromBeacon);
                            Debug.Log("Deleting at far");
                        }
                    }
                }
                else
                {
                }
            }
            else
            {
                //at far range let's not do anything
            }
        }
        Debug.Log("Beacon Debug " + printDebug);

        if (AudioPlayers == null)
        {
            Debug.Log("No Audio Players Found");
            return;
        }

        List<string> sList = new List<string>();

        foreach (string s in AudioPlayers.Keys)
        {
            if (AudioPlayers[s] == null)
            {
                //Debug.Log("YES ");
                sList.Add(s);
            }
            else
            {
                //Debug.Log("NO");
            }
        }

        foreach (string s in sList)
        {
            AudioPlayers.Remove(s);
        }
        sList.Clear();


        List<GameObject> sortedBeacons = new List<GameObject>();
        if (AudioPlayers.Values.Count > 0)
        {
            sortedBeacons = AudioPlayers.Values.ToList();
            var nullBeacons = new List<GameObject>();

            sortedBeacons = sortedBeacons.OrderBy(go => go.GetComponent<BluetoothAudioSource>().knownRSSI).ToList();
            sortedBeacons.Reverse();
            var count = 0;
            foreach (GameObject go in sortedBeacons)
            {
                if (!PlayMultiple && count == 0)
                {
                    //we want to solo just one track
                    go.transform.SetSiblingIndex(0);
                    go.GetComponent<BluetoothAudioSource>().SetPlaying(true);
                }
                else if (!PlayMultiple && count > 0)
                {
                    //other tracks should be set off.
                    go.GetComponent<BluetoothAudioSource>().SetPlaying(false);
                }
                else if (PlayMultiple)
                {
                    //We want to play all the tracks
                    go.GetComponent<BluetoothAudioSource>().SetPlaying(true);
                }

                if (go.GetComponent<AudioSource>().isPlaying)
                {
                    if (count == 0)
                    {
                        //Debug.Log("MAXING " + go.name);
                        go.GetComponent<AudioSource>().volume = 1.0f;
                    }
                    else
                    {
                        go.SetActive(true);
                        go.GetComponent<AudioSource>().volume = HLD.Utilities.Map(go.GetComponent<BluetoothAudioSource>().knownRSSI, minImmediateRSSI, maxImmediateRSSI, 0.15f, .35f);
                    }
                }
                count++;
            }
        }
    }

    private bool CheckBeaconToStart(GameObject go)
    {
        go.GetComponent<BluetoothAudioSource>().IncrementStart();
        if (go.GetComponent<BluetoothAudioSource>().getStartCount() > 10)
        {
            //reset the counter by passing true;
            go.GetComponent<BluetoothAudioSource>().getStartCount(true);
            go.GetComponent<BluetoothAudioSource>().getEndCount(true);

            return true;
        }
        else
        {
            return false;
        }
    }

    private bool CheckBeaconToEnd(GameObject go)
    {
        go.GetComponent<BluetoothAudioSource>().IncrementEnd();
        if (go.GetComponent<BluetoothAudioSource>().getEndCount() > 4)
        {
            //reset the counter by passing true;
            go.GetComponent<BluetoothAudioSource>().getEndCount(true);
            return true;
        }
        else
        {
            return false;
        }
    }

    private void DisableAudioPlayer(GameObject go, string dancer)
    {
        go.GetComponent<BluetoothAudioSource>().viewing = false;
        go.GetComponent<BluetoothAudioSource>().SetPlaying(false);
        go.GetComponent<LayoutElement>().ignoreLayout = true;
        // go.transform.SetSiblingIndex(transform.parent.childCount);
    }

    private void playBeacon(float v, string dancerFromBeacon)
    {
        AudioPlayers[dancerFromBeacon].SetActive(true);
        var go = AudioPlayers[dancerFromBeacon].GetComponent<BluetoothAudioSource>();
        go.setVolume(v);
        go.viewing = true;
        go.SetPlaying(true);
        go.GetComponent<LayoutElement>().ignoreLayout = false;

    }

    private void InstantiateBlueToothObject(string dancerFromBeacon)
    {
        GameObject tmp = Instantiate(AudioPlayerPrefab);
        tmp.name = dancerFromBeacon + "_Bluetooth_Audio";
        try
        {
            tmp.transform.SetParent(GameObject.Find("AudioSourceList").transform);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to get grid for these components " + e);
        }

        tmp.transform.localScale = new Vector3(1, 1, 1);

        AudioPlayers.Add(dancerFromBeacon, tmp);

        var blas = tmp.GetComponent<BluetoothAudioSource>();

        blas.Init();
        blas.SetAudio(dancerFromBeacon, "hld/"+ ShowName.ToLower()+"/narratives/audio");
        blas.SetPhoto(dancerFromBeacon, "hld/"+ ShowName.ToLower()+"/narratives/photos");
        blas.SetAudioCaptions(dancerFromBeacon, "hld/"+ ShowName.ToLower()+"/narratives/captions");
        StartCoroutine(blas.PlayCaptionsWithAudio());
        blas.AudioStart();
        tmp.SetActive(false);
    }

    IEnumerator StartupBluetoothService()
    {
        BluetoothState.Init();

        while (BluetoothState.GetBluetoothLEStatus().Equals(BluetoothLowEnergyState.UNKNOWN))
        {
            try
            {
                BluetoothState.EnableBluetooth();
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
            yield return null;
        }
        iBeaconReceiver.Restart();
        //  iBeaconReceiver.Scan();

        yield break;
    }


    private void OpenPageWithBluetooth(int label)
    {
        var dancer = DancerMajorsList[label];
        dancer = dancer.Replace("_", "");

        switch (label)
        {
            default:
                if (GameObject.Find(dancer + "_Button") != null)
                {
                    // GameObject.Find(dancer + "_Button").GetComponent<Button>().onClick.Invoke();
                }
                else
                {
                    Debug.LogWarning("Page not found.");
                }
                break;
        }


        //clear all our old beacons
        mybeacons.Clear();
        iBeaconReceiver.Stop();

    }

    Dictionary<int, int> dancersDetected;
    int detectionsRecquired = 2;
    int cnt = 0;
    private GameObject OnOff;

    void CountRecognized(int label)
    {
        cnt++;
        if (dancersDetected.ContainsKey(label))
        {
            dancersDetected[label] = dancersDetected[label] + 1;
            Debug.Log("detected " + DancerMajorsList[label - 1] + " count: " + dancersDetected[label]);
        }
        else
        {
            dancersDetected.Add(label, 0);
        }

        if (dancersDetected[label] > detectionsRecquired) //lucky number 7
        {
            OpenPageWithBluetooth(label - 1);
            dancersDetected.Clear();
        }

        if (cnt > 60)
        {
            cnt = 0;
            dancersDetected.Clear();
        }

    }

    IEnumerator BeaconUpdateCoroutine()
    {
        WaitForSeconds wait = new WaitForSeconds(.25f);
        while (true)
        {
            if (UIB_PageManager.CurrentPage == gameObject)
            {
                CheckBeaconsForDistance();
            }
            yield return wait;
        }
    }
}