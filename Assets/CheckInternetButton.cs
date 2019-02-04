using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckInternetButton : MonoBehaviour
{
    GameObject noWifiButton;
    // Start is called before the first frame update
    void Start()
    {
        noWifiButton = GameObject.Find("NoWifiIcon");
        if (noWifiButton == null)
        {
            Debug.LogWarning("chekc the name of the gameobject");
        }

        GetComponent<Button>().onClick.AddListener(delegate
        {
            noWifiButton.GetComponent<Button>().onClick.Invoke();
        });
    }


}
