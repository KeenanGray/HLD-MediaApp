using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//This Script is used on buttons that will change the screen to another view of the app when pressed.

//NOTE:: Buttons should have be named following the syntax NAME_Button
//Script will find a screen for each button that matches NAME_Screen
public class ChangeScreen : MonoBehaviour {

    public GameObject newScreen;

    // Use this for initialization
    void Start () {
        var screenName = gameObject.name.ToString().Split('_')[0];
        screenName = screenName + "_Screen";
        Debug.Log(screenName);
        newScreen = GameObject.Find(screenName);

        if (newScreen != null)
        {
            //nothing to do here
        }
        else
            Debug.LogWarning(gameObject.name + ": Something is mismatched for this button. It has the ChangeScreen script, but no screen has been assigned" +
                             " check the names of these gameobjects");

        var myBtn = GetComponent<Button>();
        if (myBtn != null)
            GetComponent<Button>().onClick.AddListener(Activate);
        else
            Debug.LogWarning(gameObject.name+ ": There is no button component on this UI element. It cannot use the ChangeScreen script without a button");

        ResolutionManager.GetCurrentResolution();
    }

    // Update is called once per frame
    void Update () {
		
	}

    void Activate(){
        newScreen.SetActive(true);
        newScreen.GetComponent<AppScreen>().StartCoroutine("MoveScreenIn");
    }

   

  
}
