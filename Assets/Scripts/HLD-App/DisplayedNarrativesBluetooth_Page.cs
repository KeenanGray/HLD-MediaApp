using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UI_Builder;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DisplayedNarrativesBluetooth_Page : MonoBehaviour, UIB_IPage
{
    public List<string> DancerMajorsList;

    GameObject GoToListBtn;

    Dictionary<string, GameObject> AudioPlayers;
    GameObject AudioPlayerPrefab;

    string PageName = "";
    string ListName = "";
    public bool PlayMultiple;

    string ToggleStartingText = "";

    public void Init()
    {
        dancersDetected = new Dictionary<int, int>();

        ShowName = gameObject.name.Split('-')[0];

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
        var tmp = GameObject.Find("CameraViewTexture");

        foreach (UIB_Button button in GetComponentsInChildren<UIB_Button>())
        {

        }

        AudioPlayers = new Dictionary<string, GameObject>();
        AudioPlayerPrefab = Resources.Load("BluetoothAudioSource") as GameObject;

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
#if !UNITY_EDITOR
        Debug.Log(state);
#endif
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

    }

    void WhenApplicationUnpauses()
    {

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
            var dancers = tmp.LoadAsset<TextAsset>(ShowName + "ListOfDancers") as TextAsset;

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
        }

        GetComponent<Canvas>().enabled = true;

        if (UIB_PageManager.LastPage.name != ListName)
        {
        }

        var page = GameObject.Find(ListName).GetComponent<UIB_Page>();
        page.GetComponent<UIB_Page>().StartCoroutine("MoveScreenIn", false);

        if (!delegateAddedToList)
        {
            page.OnActivated += myDelegate();
            page.OnDeActivated += myDeactivatedDelegate();
            delegateAddedToList = true;
        }

        if (!movedListIn)
        {
            //  page.GetComponent<UIB_Page>().Init();
            page.GetComponent<UIB_Page>().StartCoroutine("MoveScreenIn", false);
            movedListIn = true;
        }

        StartCoroutine(GetComponent<UIB_Page>().ResetUAP(true));

        //create the players for each dancer
        for (int i = 1; i < DancerMajorsList.Count; i++)
        {
            var DancerFromBeacon = DancerMajorsList[i - 1];
            InstantiateBlueToothObject(DancerFromBeacon);

        }
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
        foreach (GameObject go in AudioPlayers.Values)
        {
            Destroy(go);
        }

        AudioPlayers.Clear();
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
            movedListIn = false;
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
        GetComponent<UIB_Page>().OnActivated += PageActivatedHandler;
        GetComponent<UIB_Page>().OnDeActivated += PageDeActivatedHandler;

        delegateAddedToList = false;
        ShowName = gameObject.name.Split('-')[0];

        DancerMajorsList = new List<string>();
        GoToListBtn = GameObject.Find(ShowName + "-ListOfDancersButton");
        GoToListBtn.GetComponent<Button>().onClick.AddListener(GoToList);

        ToggleButton = GameObject.Find(ShowName + "-ToggleMultipleButton");
        ToggleButton.GetComponent<Button>().onClick.AddListener(ToggleMultiple);

        OnOff = GameObject.Find(ShowName + "-ToggleMultipleOnOff");

        OnOff.transform.Find("ToggleOff").gameObject.SetActive(false);
        OnOff.transform.Find("ToggleOn").gameObject.SetActive(true);

        ToggleStartingText = ToggleButton.GetComponentInChildren<TextMeshProUGUI>().text;
        var uap_T = GameObject.Find(ShowName + "-ToggleMultipleButton").GetComponentInParent<Special_AccessibleButton>();
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

        var t = GameObject.Find(ShowName + "-ToggleMultipleButton").GetComponentInChildren<TextMeshProUGUI>();
        var uap_T = GameObject.Find(ShowName + "-ToggleMultipleButton").GetComponentInParent<Special_AccessibleButton>();

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

        //toggle all the children on or off
        TurnOffNonSelected();

    }

    public void TurnOffNonSelected()
    {
        foreach (GameObject blas in AudioPlayers.Values)
        {
            blas.SetActive(true);

            if (!blas.GetComponent<BluetoothAudioSource>().Selected)
            {
                blas.SetActive(PlayMultiple);
            }

        }
    }

    void GoToList()
    {
        GetComponent<Canvas>().enabled = false;
        UIB_PageManager.CurrentPage = GameObject.Find(ListName);
        UIB_PageManager.LastPage = GameObject.Find(ListName);

        StartCoroutine(GetComponent<UIB_Page>().ResetUAP(false));
        StartCoroutine(UIB_PageManager.CurrentPage.GetComponent<UIB_Page>().ResetUAP(true));

    }

    IEnumerator InitializeRecognizer()
    {
        yield break;
    }
    /*
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
    */

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
            tmp.transform.SetParent(GameObject.Find(ShowName + "-AudioSourceList").transform);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to get grid for these components " + e);
        }

        tmp.transform.localScale = new Vector3(1, 1, 1);

        AudioPlayers.Add(dancerFromBeacon, tmp);

        var blas = tmp.GetComponent<BluetoothAudioSource>();

        blas.Init();
        blas.SetAudio(dancerFromBeacon, "hld/" + ShowName.ToLower() + "/narratives/audio");
        blas.SetPhoto(dancerFromBeacon, "hld/" + ShowName.ToLower() + "/narratives/photos");
        blas.SetAudioCaptions(dancerFromBeacon, "hld/" + ShowName.ToLower() + "/narratives/captions");
        StartCoroutine(blas.PlayCaptionsWithAudio());
        blas.AudioStart();
        blas.Stop();

        blas.SetOpacity(150);
        //add the on click listener to the button
        blas.GetComponentInChildren<Button>().onClick.AddListener(blas.OnAudioSourceClicked);


    }

    Dictionary<int, int> dancersDetected;
    int detectionsRecquired = 2;
    int cnt = 0;
    private GameObject OnOff;
    private bool delegateAddedToList;
    private bool movedListIn;
    private List<BluetoothAudioSource> audioSourceList;

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
            dancersDetected.Clear();
        }

        if (cnt > 60)
        {
            cnt = 0;
            dancersDetected.Clear();
        }

    }

}