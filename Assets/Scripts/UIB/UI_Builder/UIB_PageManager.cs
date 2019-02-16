using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIB_PageManager : MonoBehaviour {

    public static GameObject LastPage;
    public static GameObject CurrentPage;

    public static bool InternetActive { get; internal set; }

    // Use this for initialization
    void Start () {
        CurrentPage = GameObject.Find("Landing_Page");
	}
	
	// Update is called once per frame
	void Update () {
      //  Debug.Log("Currently On " + CurrentPage + " came from " + LastPage);
    }
}
