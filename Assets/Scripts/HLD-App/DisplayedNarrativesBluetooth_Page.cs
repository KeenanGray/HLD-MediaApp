using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI_Builder;
using UnityEngine;
using UnityEngine.UI;

public class DisplayedNarrativesBluetooth_Page : MonoBehaviour, UIB_IPage
{

    public static List<string> DancerMajorsList;
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
    public TextMeshProUGUI DebugText;
    iBeaconReceiver beaconr;
    private List<Beacon> mybeacons;

    Dictionary<string, GameObject> AudioPlayers;
    GameObject AudioPlayerPrefab;

    string PageName = "DisplayedNarrativesBT_Page";
    string ListName = "DisplayedNarrativesList_Page";

    public void Init()
    {
        DebugText.text = "";
        beaconr = GetComponent<iBeaconReceiver>();
        mybeacons = new List<Beacon>();
        iBeaconReceiver.BeaconRangeChangedEvent += OnBeaconRangeChanged;
        dancersDetected = new Dictionary<int, int>();

        UIB_AssetBundleHelper.InsertAssetBundle("hld/general");
        UIB_AssetBundleHelper.InsertAssetBundle("hld/displayed/narratives/photos");
        UIB_AssetBundleHelper.InsertAssetBundle("hld/displayed/narratives/captions");
        UIB_AssetBundleHelper.InsertAssetBundle("hld/displayed/narratives/audio");

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
        StartCoroutine("StartupBluetoothService");
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
        GameObject.Find(ListName).GetComponent<Canvas>().enabled = false;
        GameObject.Find(ListName).GetComponent<UIB_Page>().StartCoroutine("MoveScreenOut", false);

        StopBTAudio();

    }

    // Use this for initialization
    void Start()
    {
        DancerMajorsList = new List<string>();
        GoToListBtn = GameObject.Find("ListOfDancersButton");
        GoToListBtn.GetComponent<Button>().onClick.AddListener(GoToList);
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
            for (int i = 0; i < 5; i++)
            {
                var DancerFromBeacon = DancerMajorsList[i];

                if (i == 2 || i == 2 || i == 3)
                {
                    if (AudioPlayers.ContainsKey(DancerFromBeacon))
                    {
                        //we already have a player set up for that dancer, let's bring up the volume.
                        playBeacon(1, DancerFromBeacon);
                    }
                    else
                    {
                        //We haven't made a gameobject for that dancer, make it and add it to the list
                        InstantiateBlueToothObject(DancerFromBeacon);

                    }
                }
            }
        }
#endif

    }

    void CheckBeaconsForDistance()
    {
        var minNearVol = 0.1f;
        var maxNearVol = 0.6f;

        var minImmediateVol = 1f;
        var maxImmediatevol = 1;

        var minImmediateRSSI = -60;
        var maxImmediateRSSI = -30;

        var minNearRSSI = -70;
        var maxNearRSSI = -59;

        if (mybeacons == null)
            return;

        if (mybeacons.Count <= 0)
            return;

        foreach (Beacon b in mybeacons)
        {
            var DancerFromBeacon = DancerMajorsList[b.major - 1];
            var absRSSI = Math.Abs(b.rssi);

            if (b.range != BeaconRange.UNKNOWN)
            {
            }

            if (b.range == BeaconRange.IMMEDIATE || b.range == BeaconRange.NEAR)
            {
                if (AudioPlayers.ContainsKey(DancerFromBeacon))
                {
                    //we already have a player set up for that dancer, let's bring up the volume.
                    if (b.rssi > minImmediateRSSI && b.rssi < maxImmediateRSSI && b.accuracy < 1 && b.accuracy > 0)
                    {
                        if (CheckBeaconsToStart(AudioPlayers[DancerFromBeacon]))
                        {
                            Debug.Log("Near " + DancerFromBeacon + " rssi:" + b.rssi + " acc:" + b.accuracy + ":" + b.strength);
                            playBeacon(HLD.Utilities.Map(b.rssi, minImmediateRSSI, maxImmediateRSSI, minImmediateVol, maxImmediatevol), DancerFromBeacon);
                        }
                    }
                    else if (b.rssi < minImmediateRSSI && b.accuracy > 0)
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
                }
            }

            if (b.range == BeaconRange.FAR)
            {
                if (AudioPlayers.ContainsKey(DancerFromBeacon))
                {
                    //perhaps if the accuracy is lower than 10, we may still be close enough
                    if (b.rssi > minNearRSSI && b.rssi <= maxNearRSSI && b.accuracy < 1 && b.accuracy > 0)
                    {
                    }
                    else if (b.rssi < minNearRSSI && b.accuracy > 0 && b.accuracy > 2)
                    {
                        if (CheckBeaconToEnd(AudioPlayers[DancerFromBeacon]))
                            DisableAudioPlayer(AudioPlayers[DancerFromBeacon], DancerFromBeacon);
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

    }

    private bool CheckBeaconsToStart(GameObject go)
    {
        go.GetComponent<BluetoothAudioSource>().IncrementStart();
        Debug.Log("s: " + go.GetComponent<BluetoothAudioSource>().getStartCount());
        if (go.GetComponent<BluetoothAudioSource>().getStartCount() > 20)
        {
            //reset the counter by passing true;
            go.GetComponent<BluetoothAudioSource>().getStartCount(true);
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
//        Debug.Log("e: " + go.GetComponent<BluetoothAudioSource>().getEndCount());
        if (go.GetComponent<BluetoothAudioSource>().getEndCount() > 40)
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
        go.SetActive(false);
        //we already have a player set up for that dancer, stop playing audio
        //and remove the object after 1 seconds
        //go.GetComponent<BluetoothAudioSource>().Stop();
        //Destroy(go, 1.0f);
        //AudioPlayers.Remove(dancer);
    }

    private void playBeacon(float v, string dancerFromBeacon)
    {
        AudioPlayers[dancerFromBeacon].SetActive(true);
        AudioPlayers[dancerFromBeacon].GetComponent<BluetoothAudioSource>().setVolume(v);
        AudioPlayers[dancerFromBeacon].GetComponent<BluetoothAudioSource>().Play();
    }

    private void InstantiateBlueToothObject(string dancerFromBeacon)
    {
        GameObject tmp = Instantiate(AudioPlayerPrefab);
        tmp.name = dancerFromBeacon + "_Bluetooth_Audio";
        tmp.transform.SetParent(GameObject.Find("BluetoothGridPanel").transform);

        tmp.transform.localScale = new Vector3(1, 1, 1);

        AudioPlayers.Add(dancerFromBeacon, tmp);

        var blas = tmp.GetComponent<BluetoothAudioSource>();

        blas.Init();
        blas.SetAudio(dancerFromBeacon, "hld/displayed/narratives/audio");
        blas.SetPhoto(dancerFromBeacon, "hld/displayed/narratives/photos");
        blas.SetAudioCaptions(dancerFromBeacon, "hld/displayed/narratives/captions");
        blas.StartCoroutine("PlayCaptionsWithAudio");
        blas.AudioStart();
        tmp.SetActive(false);
    }

    IEnumerator StartupBluetoothService()
    {
        BluetoothState.Init();

        while (BluetoothState.GetBluetoothLEStatus().Equals(BluetoothLowEnergyState.UNKNOWN))
        {
            BluetoothState.EnableBluetooth();
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
        while (true)
        {
            CheckBeaconsForDistance();
            yield return null;
        }
        yield break;
    }
}