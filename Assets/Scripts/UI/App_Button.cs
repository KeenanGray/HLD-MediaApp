﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

//This Script is used on buttons that will change the screen to another view of the app when pressed.

//NOTE:: Buttons should have be named following the syntax NAME_Button
//Script will find a screen for each button that matches NAME_Screen

[AddComponentMenu("App_Button_Editor")]
[RequireComponent(typeof(Button))]
public class App_Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{
    public enum Button_Activates
    {
        None,
        Page,
        SubMenu,
        SpecificPage,
        Video,
        Website
    }

    public GameObject newScreen;
    public GameObject VO_Select;
    public Button_Activates Button_Opens;
    public TextMeshProUGUI buttonText;
    public string WebUrl;
    private Color originalColor;

    public void Init()
    {
        if (gameObject.name == "App_SubMenuButton")
        {
            DeActivate();
            return;
        }
        if (Button_Opens == Button_Activates.None)
        {
            DeActivate();
            return;
        }

        var screenName = gameObject.name.ToString().Split('_')[0];
        var typeName = Button_Opens.ToString().Replace(" ", "");
        screenName = screenName + ("_" + typeName);
        var PageObject = GameObject.Find(screenName);

        if (PageObject != null)
        {
            newScreen = PageObject;
        }

        var myBtn = GetComponent<Button>();
        if (myBtn != null)
        {
            GetComponent<Button>().onClick.AddListener(OnButtonPressed);
        }
        else
            Debug.LogWarning(gameObject.name + ": There is no button component on this UI element. It cannot use the App_Button script without a button");

        if (Button_Opens == Button_Activates.Video)
        {
            newScreen = GameObject.FindWithTag("App_VideoPlayer");
        }

        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText == null)
        {
           // Debug.LogError("no buttonText " + gameObject.name);
        }
            DeActivate();
        }

        void OnButtonPressed()
        {
            switch (Button_Opens)
            {
                case Button_Activates.Page:
                    newScreen.GetComponent<Page>().StartCoroutine("MoveScreenIn");
                    break;
                case Button_Activates.SpecificPage:
                    newScreen.GetComponent<Page>().StartCoroutine("MoveScreenIn");
                    break;
                case Button_Activates.SubMenu:
                    newScreen.GetComponent<SubMenu>().StartCoroutine("MoveScreenIn");
                    break;
                case Button_Activates.Video:
                    newScreen.GetComponent<Page>().StartCoroutine("MoveScreenIn");
                    break;
                case Button_Activates.Website:
                    if (WebUrl != null)
                        Application.OpenURL(WebUrl);
                    else
                        Debug.LogWarning("Button not assigned a url");
                    break;
                default:
                    Debug.Log("No Activity for this button");
                    break;
            }

            var CurrPage = GetComponentInParent<Page>();
            if (CurrPage != null)
                CurrPage.DeActivate();

            var CurrSubMenu = GetComponentInParent<SubMenu>();
            if (CurrSubMenu != null)
                CurrSubMenu.DeActivate();

        if (VO_Select != null)
        {
            UAP_AccessibilityManager.SelectElement(VO_Select);
        }

        }

        public void Activate()
        {
            GetComponent<Button>().enabled = true;
            GetComponent<Special_AccessibleButton>().enabled = true;
        }
        public void DeActivate()
        {
            GetComponent<Button>().enabled = false;
        }

        public void SetButtonText(string newtext)
        {
            GetComponentInChildren<TextMeshProUGUI>().text = newtext;
            GetComponent<Special_AccessibleButton>().AutoFillTextLabel();

        }

        private void OnEnable()
        {
          
        }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Color32 highlightColor = new Color32(193, 200, 47, 255);
        if (GetComponent<Button>().image.sprite != null)
            originalColor = GetComponent<Button>().image.color;
        else
            originalColor = new Color32(230, 230, 230, 255);

        if (GetComponent<Button>().image.sprite != null)
        {
            if (GetComponent<Button>().image.sprite.name == "X_w")
                GetComponent<Button>().image.color = highlightColor;
        }
            if (buttonText != null)
                buttonText.color = highlightColor;
        
    }

        public void OnPointerExit(PointerEventData eventData)
        {
        if (GetComponent<Button>().image.sprite != null)
            GetComponent<Button>().image.color = originalColor;
        
        if (buttonText != null)
            buttonText.color = originalColor;
        }

    public void SetVO(GameObject target){
        VO_Select = target;
    }
}
