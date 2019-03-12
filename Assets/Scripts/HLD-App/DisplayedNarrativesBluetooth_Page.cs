using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI_Builder;
using UnityEngine;
using UnityEngine.UI;

public class DisplayedNarrativesBluetooth_Page : MonoBehaviour, UIB_IPage
{
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

        StartCoroutine("StartupBluetoothService");
#endif

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
        GetComponent<Canvas>().enabled = true;

        if (UIB_PageManager.LastPage.name != ListName)
        {
            Debug.Log(UIB_PageManager.LastPage + " JDKFLD J");
        }
       
        var page = GameObject.Find(ListName).GetComponent<UIB_Page>();
        page.GetComponent<UIB_Page>().StartCoroutine("MoveScreenIn", false);

        page.OnActivated += myDelegate();
        page.OnDeActivated += myDeactivatedDelegate();

        page.GetComponent<UIB_Page>().Init();
        page.GetComponent<UIB_Page>().StartCoroutine("MoveScreenIn", false);
        page.OnActivated -= myDelegate();

        iBeaconReceiver.Scan();

        StartCoroutine(GetComponent<UIB_Page>().ResetUAP(true));
    }

    private UIB_Page.DeActivated myDeactivatedDelegate()
    {
        return delegate {
            Debug.Log("HERE2");
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

        CheckBeaconsForDistance();
    }


    //simulate in editor with update
#if UNITY_EDITOR

#endif

    private void Update()
    {
        CheckBeaconsForDistance();
       
        if (Input.GetKeyDown(KeyCode.T) && UIB_PageManager.CurrentPage==gameObject)
        {
            for (int i = 0; i < 5; i++)
            {
                var DancerFromBeacon = Enum.GetName(typeof(DancerMajors), i).ToString();

                if (i == 2 || i == 2 || i == 3)
                {
                    if (AudioPlayers.ContainsKey(DancerFromBeacon))
                    {
                        //we already have a player set up for that dancer, let's bring up the volume.
                        AudioPlayers[DancerFromBeacon].GetComponent<BluetoothAudioSource>().Play();
                    }
                    else
                    {
                        //We haven't made a gameobject for that dancer, make it and add it to the list
                        GameObject tmp = Instantiate(AudioPlayerPrefab);
                        tmp.name = DancerFromBeacon + "_Audio";
                        AudioPlayers.Add(DancerFromBeacon, tmp);
                        tmp.GetComponent<BluetoothAudioSource>().Init();
                        tmp.GetComponent<BluetoothAudioSource>().SetAudio(DancerFromBeacon, "hld/displayed/narratives/audio");
                        tmp.GetComponent<BluetoothAudioSource>().AudioStart();

                    }
                }
               
            }
        }

    }

    void CheckBeaconsForDistance()
    {

        var minNearVol = 0.1f;
        var minImmediateVol = 0.3f;
        var maxNearVol = .6f;
        var maxImmediatevol = 1;

        if (mybeacons == null)
            return;

        foreach (Beacon b in mybeacons)
        {
            var DancerFromBeacon = Enum.GetName(typeof(DancerMajors), b.major - 1).ToString();

            if (b.range == BeaconRange.IMMEDIATE)
            {
                if (AudioPlayers.ContainsKey(DancerFromBeacon))
                {
                    //we already have a player set up for that dancer, let's bring up the volume.
                    AudioPlayers[DancerFromBeacon].GetComponent<BluetoothAudioSource>().setVolume(HLD.Utilities.Map(b.rssi, -60, -30, minImmediateVol, maxImmediatevol));
                    AudioPlayers[DancerFromBeacon].GetComponent<BluetoothAudioSource>().Play();
                }
                else
                {
                    //We haven't made a gameobject for that dancer, make it and add it to the list
                    GameObject tmp = Instantiate(AudioPlayerPrefab);
                    tmp.name = DancerFromBeacon + "_Audio";
                    AudioPlayers.Add(DancerFromBeacon, tmp);
                    tmp.GetComponent<BluetoothAudioSource>().Init();
                    tmp.GetComponent<BluetoothAudioSource>().SetAudio(DancerFromBeacon, "hld/displayed/narratives/audio");
                    tmp.GetComponent<BluetoothAudioSource>().AudioStart();

                }
            }
            if (b.range == BeaconRange.NEAR)
            {
                if (AudioPlayers.ContainsKey(DancerFromBeacon))
                {
                    //continue to adjust volume at near range
                    AudioPlayers[DancerFromBeacon].GetComponent<BluetoothAudioSource>().setVolume(HLD.Utilities.Map(b.rssi, -60, -30, minNearVol, maxNearVol));
                }
                else
                {
                    //We haven't made a gameobject for that dancer, make it and add it to the list
                    //But don't play it yet
                    GameObject tmp = Instantiate(AudioPlayerPrefab);
                    tmp.name = DancerFromBeacon + "_Audio";
                    tmp.GetComponent<BluetoothAudioSource>().Init();
                    tmp.GetComponent<BluetoothAudioSource>().SetAudio(DancerFromBeacon, "hld/displayed/narratives/audio");
                    AudioPlayers.Add(DancerFromBeacon, tmp);
                }
            }
            if (b.range == BeaconRange.FAR)
            {
                if (AudioPlayers.ContainsKey(DancerFromBeacon))
                {
                    //we already have a player set up for that dancer, stop playing audio
                    AudioPlayers[DancerFromBeacon].GetComponent<BluetoothAudioSource>().Stop();
                }
                else
                {
                    //We haven't made a gameobject for that dancer, make it and add it to the list
                    //But don't play it yet
                    GameObject tmp = Instantiate(AudioPlayerPrefab);
                    tmp.name = DancerFromBeacon + "_Audio";
                    tmp.GetComponent<BluetoothAudioSource>().Init();
                    tmp.GetComponent<BluetoothAudioSource>().SetAudio(DancerFromBeacon, "hld/displayed/narratives/audio");
                    AudioPlayers.Add(DancerFromBeacon, tmp);
                }
            }
        }

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
        var dancer = Enum.GetNames(typeof(DancerMajors))[label];
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
            Debug.Log("detected " + Enum.GetNames(typeof(DancerMajors))[label - 1] + " count: " + dancersDetected[label]);
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
}