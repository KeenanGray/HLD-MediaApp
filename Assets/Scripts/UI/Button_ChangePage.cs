using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//This Script is used on buttons that will change the screen to another view of the app when pressed.

//NOTE:: Buttons should have be named following the syntax NAME_Button
//Script will find a screen for each button that matches NAME_Screen


[ExecuteInEditMode]
public class Button_ChangePage : MonoBehaviour {
    public enum Button_Activates
    {
        Page,
        SubMenu
    }

    GameObject newScreen;

    public Button_Activates Button_Opens;

    private void Awake()
    {
        var screenName = gameObject.name.ToString().Split('_')[0];
        screenName = screenName + ("_" + Button_Opens.ToString());
        newScreen = GameObject.Find(screenName);

        if (newScreen != null)
        {
            //nothing to do here
        }
        else
        {
            Debug.LogWarning(gameObject.name + ": Something is mismatched for this button. It has the ChangePage script, but no screen has been assigned" +
                             " check the names of these gameobjects");
            gameObject.SetActive(false);
            return;
        }

        var myBtn = GetComponent<Button>();
        if (myBtn != null)
            GetComponent<Button>().onClick.AddListener(OnButtonPressed);
        else
            Debug.LogWarning(gameObject.name + ": There is no button component on this UI element. It cannot use the ChangeScreen script without a button");
    }

    void OnButtonPressed(){
        newScreen.SetActive(true);

        if (Button_Opens == Button_Activates.Page)
        {
            var AppScript = newScreen.GetComponent<Page>();

            if (AppScript == null)
            {
                Debug.LogWarning("Component not found");
            }
            AppScript.StopAllCoroutines();
            AppScript.Activate();
            AppScript.StartCoroutine("MoveScreenIn");
        }

        else if(Button_Opens == Button_Activates.SubMenu){
            var AppScript = newScreen.GetComponent<SubMenu>();

            if (AppScript == null)
            {
                Debug.LogWarning("Component not found");
            }
            AppScript.StopAllCoroutines();
            AppScript.Activate();
            AppScript.StartCoroutine("MoveScreenIn");
        }

    }

    void DeActivate(){
        //newScreen.SetActive(false);
    }

   

  
}
