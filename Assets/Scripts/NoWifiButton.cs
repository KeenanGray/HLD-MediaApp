using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoWifiButton : MonoBehaviour
{
    Color tmpColor;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(WifiButtonClicked);
    }

    private void WifiButtonClicked()
    {
        switch (Application.internetReachability)
        {
            case NetworkReachability.NotReachable:
                Debug.Log("internet not reachable");
                break;
            case NetworkReachability.ReachableViaLocalAreaNetwork:
                ReactivateInternet();
                break;
            case NetworkReachability.ReachableViaCarrierDataNetwork:
                ReactivateInternet();
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ReactivateInternet()
    {
        Debug.Log("Internet is Available");
        UIB_PageManager.InternetActive = true;

        tmpColor = GetComponentInChildren<Image>().color;
        GetComponentInChildren<Image>().color = new Color(tmpColor.r, tmpColor.g,tmpColor.b,0);
        GetComponent<Button>().interactable = false;

        GameObject.Find("MainCanvas").GetComponent<InitializationManager>().UpdateFilesIfNecessary();
    }
}
