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
                break;
            case NetworkReachability.ReachableViaCarrierDataNetwork:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   
}
