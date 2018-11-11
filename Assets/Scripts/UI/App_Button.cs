using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//This Script is used on buttons that will change the screen to another view of the app when pressed.

//NOTE:: Buttons should have be named following the syntax NAME_Button
//Script will find a screen for each button that matches NAME_Screen

public class App_Button : MonoBehaviour {
    public enum Button_Activates
    {
        None,
        Page,
        SubMenu
    }

    GameObject newScreen;

    GameObject LandingPage;
    public Button_Activates Button_Opens;
  
    public void Init()
    {
        LandingPage = GameObject.Find("LandingScreen");
        if (LandingPage == null)
            Debug.LogWarning("No landing page");

        if (gameObject.name == "App_SubMenuButton"){
            Debug.LogWarning("This button is in the pool");
            return;
        }
        if (Button_Opens == Button_Activates.None)
        {
            return;
        }

        var screenName = gameObject.name.ToString().Split('_')[0];
        screenName = screenName + ("_" + Button_Opens.ToString());

        newScreen = GameObject.Find(screenName);

        if (newScreen != null)
        {
            //nothing to do here
        }
        else
        {
            Debug.LogWarning(gameObject.name + ": Something is mismatched for this button. It has the App_Button script, but no gameobject - "+ screenName + " has been assigned. Check the names of these gameobjects");
         //  gameObject.SetActive(false);
            return;
        }

        var myBtn = GetComponent<Button>();
        if (myBtn != null)
        {
            GetComponent<Button>().onClick.AddListener(OnButtonPressed);
        }
        else
            Debug.LogWarning(gameObject.name + ": There is no button component on this UI element. It cannot use the App_Button script without a button");
    }

    void OnButtonPressed(){
        switch(Button_Opens){
            case Button_Activates.Page:
                newScreen.GetComponent<Page>().ToggleRenderer(true);
                break;
            case Button_Activates.SubMenu:
                newScreen.GetComponent<SubMenu>().ToggleRenderer(true);
                break;
            default:
                Debug.Log("No Activity for this button");
                break;
        }

        newScreen.SetActive(true);

        if (Button_Opens == Button_Activates.Page)
        {
            var AppScript = newScreen.GetComponent<Page>();

            if (AppScript == null)
            {
                Debug.LogWarning("Component not found");
            }
            AppScript.StopAllCoroutines();
            AppScript.StartCoroutine("MoveScreenIn");
        }

        else if(Button_Opens == Button_Activates.SubMenu){
            var AppScript = newScreen.GetComponent<SubMenu>();

            if (AppScript == null)
            {
                Debug.LogWarning("Component not found");
            }
            AppScript.StopAllCoroutines();
            AppScript.StartCoroutine("MoveScreenIn");
        }
        LandingPage.SetActive(false);
    }

    void DeActivate(){
        //newScreen.SetActive(false);
    }

    public void SetButtonText(string newtext){
        GetComponentInChildren<TextMeshProUGUI>().text = newtext;
    }

  
}
