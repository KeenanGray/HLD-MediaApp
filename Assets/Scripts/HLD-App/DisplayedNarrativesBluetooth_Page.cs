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

        GetComponent<UIB_Page>().OnActivated += PageActivatedHandler;
        GetComponent<UIB_Page>().OnDeActivated += PageDeActivatedHandler;

        var tmp = GameObject.Find("CameraViewTexture");

        //  if (fdHLD == null)
        //      Debug.LogWarning("Warning:No Camera Manager is present for face recognition");

        foreach (UIB_Button button in GetComponentsInChildren<UIB_Button>())
        {
            if (button.name == "DISPLAYED-Info_Button")
            {
                button.GetComponent<Button>().onClick.AddListener(delegate
                {
                    //  if (fdHLD != null)
                    //      fdHLD.ShutDownCamera();
                });
            }
        }

        var page = GameObject.Find("DisplayedNarrativesList_Page").GetComponent<UIB_Page>();
        foreach (UIB_Button button in page.GetComponentsInChildren<UIB_Button>())
        {
            if (button.name == "DISPLAYED-Info_Button")
            {
                button.GetComponent<Button>().onClick.AddListener(delegate
                {
                    //  if (fdHLD != null)
                    //      fdHLD.ShutDownCamera();
                });
            }
        }
    }


    public void PageActivatedHandler()
    {
        foreach (UIB_Button uibb in GetComponentsInChildren<UIB_Button>())
        {
            uibb.enabled = true;
            uibb.GetComponent<Button>().enabled = true;
        }
        foreach (UIB_Button uibb in GameObject.Find("DisplayedNarrativesList_Page").GetComponentsInChildren<UIB_Button>())
        {
            uibb.enabled = true;
            uibb.GetComponent<Button>().enabled = true;
        }

        //if (!fdHLD.isRunning)
        // fdHLD.CamInit();

        if (UAP_AccessibilityManager.IsActive())
        {
            var page = GameObject.Find("DisplayedNarrativesList_Page").GetComponent<UIB_Page>();
            page.GetComponent<UIB_Page>().StartCoroutine("MoveScreenIn", false);
            GoToList();
            return;
        }
        else
        {
            var page = GameObject.Find("DisplayedNarrativesList_Page").GetComponent<UIB_Page>();
            page.GetComponent<UIB_Page>().StartCoroutine("MoveScreenIn", false);

            page.OnActivated += delegate
          {
              UIB_PageManager.CurrentPage = GameObject.Find("DisplayedNarrativesBluetooth_Page");
          };


            Debug.Log("restart scanner");
            iBeaconReceiver.Scan();
        }
    }

    public void PageDeActivatedHandler()
    {
        GameObject.Find("DisplayedNarrativesList_Page").GetComponent<UIB_Page>().StartCoroutine("MoveScreenOut", false);

        iBeaconReceiver.Stop();
        //if (fdHLD != null && fdHLD.isRunning)
        //    fdHLD.ShutDownCamera();
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
        UIB_PageManager.CurrentPage = GameObject.Find("DisplayedNarrativesList_Page");
        UIB_PageManager.LastPage = GameObject.Find("DisplayedNarrativesList_Page");

        foreach (UIB_Button uibb in GetComponentsInChildren<UIB_Button>())
        {
            uibb.enabled = false;
            uibb.GetComponent<Button>().enabled = false;
        }

        foreach (UIB_Button uibb in GameObject.Find("DisplayedNarrativesList_Page").GetComponentsInChildren<UIB_Button>())
        {
            uibb.enabled = true;
            uibb.GetComponent<Button>().enabled = true;

        }

        /* if (fdHLD != null)
         {
             fdHLD.EndRecognizer();
             StopCoroutine("InitializeRecognizer");
         }
         */
    }

    IEnumerator InitializeRecognizer()
    {
        /* while (!fdHLD.shouldRecognize)
          {
              try
              {
                  fdHLD.BeginRecognizer();
              }
              catch (Exception e)
              {
                  Debug.Log(e);
              }
              yield return null;
          }
          */
        yield break;

    }
    void OnBeaconRangeChanged(Beacon[] beacons)
    { // 
        foreach (Beacon b in beacons)
        {
            var index = mybeacons.IndexOf(b);
            if (index == -1)
            {
                mybeacons.Add(b);
                Debug.Log("Adding beacon");
            }
            else
            {
                mybeacons[index] = b;
                Debug.Log("Already have beacon");
            }
        }
        for (int i = mybeacons.Count - 1; i >= 0; --i)
        {
            if (mybeacons[i].lastSeen.AddSeconds(10) < DateTime.Now)
            {
                Debug.Log("removing beacon after it's been around a while");
                mybeacons.RemoveAt(i);
            }
        }

        CheckBeaconsForDistance();
    }

    void CheckBeaconsForDistance(){
        foreach(Beacon b in mybeacons)
        {
            DebugText.text = b.range + " " + Enum.GetName(typeof(DancerMajors), b.major-1).ToString();
            Debug.Log("Checking Beacon " + b.major + ", " + b.range + ", " + b.strength + ", " + b.rssi + ", ");
            if (b.range == BeaconRange.IMMEDIATE)
            {
                CountRecognized(b.major);
            }
        }

    }

    IEnumerator StartupBluetoothService()
    {
        BluetoothState.Init();

        while(BluetoothState.GetBluetoothLEStatus().Equals(BluetoothLowEnergyState.UNKNOWN))
        {
            BluetoothState.EnableBluetooth();
            yield return null;
        }
        iBeaconReceiver.Restart();
      //  iBeaconReceiver.Scan();

        yield break;
    }


    private void OpenPageFromFace(int label)
    {
        var dancer = Enum.GetNames(typeof(DancerMajors))[label];
        dancer = dancer.Replace("_", "");
        Debug.Log("Dancer " + dancer);

        switch (label)
        {
            default:
                if (GameObject.Find(dancer + "_Button") != null)
                {
                    GameObject.Find(dancer + "_Button").GetComponent<Button>().onClick.Invoke();
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
            Debug.Log("label " + label);
            dancersDetected[label] = dancersDetected[label] + 1;
            Debug.Log("detected " + Enum.GetNames(typeof(DancerMajors))[label - 1] + " count: " + dancersDetected[label]);
        }
        else
        {
            dancersDetected.Add(label, 0);
        }

        if (dancersDetected[label] > detectionsRecquired) //lucky number 7
        {
            OpenPageFromFace(label - 1);
            dancersDetected.Clear();
        }

        if (cnt > 60)
        {
            cnt = 0;
            dancersDetected.Clear();
        }

    }
}