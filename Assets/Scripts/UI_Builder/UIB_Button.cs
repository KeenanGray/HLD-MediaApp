using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

//This Script is used on buttons that will change the screen to another view of the app when pressed.

//NOTE:: Buttons should have be named following the syntax NAME_Button
//Script will find a screen for each button that matches NAME_Screen
namespace UI_Builder
{
    [AddComponentMenu("App_Button_Editor")]
    [RequireComponent(typeof(UnityEngine.UI.Button))]
    public class UIB_Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public enum UIB_Button_Activates
        {
            None,
            Page,
            SubMenu,
            SpecificPage,
            Video,
            Website,
            Accessibletext
        }

        public GameObject newScreen;
        public GameObject VO_Select;
        public UIB_Button_Activates Button_Opens;
        public TextMeshProUGUI buttonText;
        public string myText;
        private Color originalColor;

        public void Init()
        {
            if (gameObject.name == "App_SubMenuButton")
            {
                DeActivate();
                return;
            }
            if (Button_Opens == UIB_Button_Activates.None)
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

            var myBtn = GetComponent<UnityEngine.UI.Button>();
            if (myBtn != null)
            {
                myBtn.onClick.AddListener(OnButtonPressed);
            }
            else
                Debug.LogWarning(gameObject.name + ": There is no button component on this UI element. It cannot use the App_Button script without a button");

            if (Button_Opens == UIB_Button_Activates.Video)
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
            bool shouldDeActivatePage = true;

            switch (Button_Opens)
            {
                case UIB_Button_Activates.Page:
                    newScreen.GetComponent<UIB_Page>().StartCoroutine("MoveScreenIn",false);
                    break;
                case UIB_Button_Activates.SpecificPage:
                    newScreen.GetComponent<UIB_Page>().StartCoroutine("MoveScreenIn",false);
                    break;
                case UIB_Button_Activates.Video:
                    newScreen.GetComponent<UIB_Page>().StartCoroutine("MoveScreenIn",false);
                    break;
                case UIB_Button_Activates.Website:
                    shouldDeActivatePage = false;
                    if (myText != null)
                        Application.OpenURL(myText);
                    else
                        Debug.LogWarning("Button not assigned a url");
                    break;
                case UIB_Button_Activates.Accessibletext:
                    shouldDeActivatePage = false;
                    UAP_AccessibilityManager.Say(myText);
                    break;
                default:
                    Debug.Log("No Activity for this button");
                    break;
            }

            if (shouldDeActivatePage)
            {
                var mainC = GameObject.Find("MainCanvas");
                foreach (UIB_Page page in mainC.GetComponentsInChildren<UIB_Page>())
                {
                    if (page.PageOnScreen)
                        page.DeActivate();
                }

                /*      var CurrPage = GetComponentInParent<Page>();
                      if (CurrPage != null)
                          CurrPage.DeActivate();

                      var CurrSubMenu = GetComponentInParent<SubMenu>();
                      if (CurrSubMenu != null)
                          CurrSubMenu.DeActivate();
                          */

            }

            if (VO_Select != null)
            {
                UAP_AccessibilityManager.SelectElement(VO_Select);
            }

        }

        public void Activate()
        {
            GetComponent<UnityEngine.UI.Button>().enabled = true;
            GetComponent<Special_AccessibleButton>().enabled = true;
        }
        public void DeActivate()
        {
            GetComponent<UnityEngine.UI.Button>().enabled = false;
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
            if (GetComponent<UnityEngine.UI.Button>().image.sprite != null)
                originalColor = GetComponent<UnityEngine.UI.Button>().image.color;
            else
                originalColor = new Color32(230, 230, 230, 255);

            if (GetComponent<UnityEngine.UI.Button>().image.sprite != null)
            {
                if (GetComponent<UnityEngine.UI.Button>().image.sprite.name == "X_w")
                    GetComponent<UnityEngine.UI.Button>().image.color = highlightColor;
                return;
            }

            if (buttonText != null)
            {
                originalColor = buttonText.color;

                //TODO: reconsider this wierd special case
                //just for the displayed button
                if (buttonText.color == highlightColor)
                    buttonText.color = new Color32(163, 170, 17, 255);

                originalColor = buttonText.color;
                buttonText.color = highlightColor;
            }

        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (GetComponent<UnityEngine.UI.Button>().image.sprite != null)
                GetComponent<UnityEngine.UI.Button>().image.color = originalColor;

            if (buttonText != null)
                buttonText.color = originalColor;
        }

        public void SetVO(GameObject target)
        {
            VO_Select = target;
        }
    }
}