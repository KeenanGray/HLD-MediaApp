using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//This Script is used on buttons that will change the screen to another view of the app when pressed.

//NOTE:: Buttons should have be named following the syntax NAME_Button
//Script will find a screen for each button that matches NAME_SubMenu
public class Button_OpenSubMenu : MonoBehaviour {

    GameObject subM;

    // Use this for initialization
    public void Initialize () {
        var screenName = gameObject.name.ToString().Split('_')[0];
        screenName = screenName + "_SubMenu";
        subM = GameObject.Find(screenName);

        if (subM != null)
        {
            //nothing to do here
        }
        else
        {
            Debug.LogWarning(gameObject.name + ": Something is mismatched for this button. It has the OpenSubMenu script, but no SubMenu has been assigned" +
                             " check the names of these gameobjects");
            gameObject.SetActive(false);
            return;
        }

        var myBtn = GetComponent<Button>();
        if (myBtn != null)
            GetComponent<Button>().onClick.AddListener(OnButtonPressed);
        else
        {
            Debug.LogWarning(gameObject.name + ": There is no button component on this UI element. It cannot use the ChangeScreen script without a button");
            gameObject.SetActive(false);
            return;
        }

        //Deactivate the attached screen so the app starts at the first view
        //This is done in a coroutine to avoid disabling the gameobject before it is finished initializing
       //StartCoroutine("Wait_DeActivate");

        ResolutionManager.GetCurrentResolution();
    }

    // Update is called once per frame
    void Update () {
		
	}

    void OnButtonPressed(){
        subM.SetActive(true);
        var AppScript = subM.GetComponent<SubMenu>();

        if(AppScript==null){
            Debug.LogWarning("Component not found");
            gameObject.SetActive(false);
        }
        AppScript.StopAllCoroutines();
        AppScript.Activate();
        AppScript.StartCoroutine("MoveScreenIn");

    }

    void DeActivate(){
       // subM.SetActive(false);
    }

   

  
}
