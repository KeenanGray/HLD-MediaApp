using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;
using UnityEngine.SceneManagement;

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
            Accessibletext,
            Scene
        }

        public static Image backgroundImage;
        public static Sprite OG_Background;
        public Sprite Special_Background;

        public GameObject newScreen;
        public GameObject VO_Select;
        public UIB_Button_Activates Button_Opens;
        public TextMeshProUGUI buttonText;
        public string s_link;
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
                    //if the new page is a template, we want to keep the current page on screen (this way the object pool won't be cleaned up)
                    shouldDeActivatePage = !newScreen.GetComponent<UIB_Page>().isTemplate;
                    newScreen.GetComponent<UIB_Page>().StartCoroutine("MoveScreenIn",false);
                    break;
                case UIB_Button_Activates.SpecificPage:
                    shouldDeActivatePage = !newScreen.GetComponent<UIB_Page>().isTemplate;
                    newScreen.GetComponent<UIB_Page>().StartCoroutine("MoveScreenIn",false);
                    break;
                case UIB_Button_Activates.Video:
                    newScreen.GetComponent<UIB_Page>().StartCoroutine("MoveScreenIn",false);
                    break;
                case UIB_Button_Activates.Website:
                    shouldDeActivatePage = false;
                    if (s_link != null)
                        Application.OpenURL(s_link);
                    else
                        Debug.LogWarning("Button not assigned a url");
                    break;
                case UIB_Button_Activates.Accessibletext:
                    shouldDeActivatePage = false;
                    UAP_AccessibilityManager.SaySkippable(s_link);
                    /*Accessibility Instructions
                     * 
                     * This app is integrated with the accessibility features of your phone
                     * The controls may differ slightly from what you are used to.
                     * Swipe up and down to navigate between elements on the page
                     * Swipe left and right to jump between blocks of elements
                     */                    
                    break;
                case UIB_Button_Activates.Scene:
                    SceneManager.LoadScene(s_link);
                    break;
                default:
                    Debug.Log("No Activity for this button");
                    break;
            }

            if (shouldDeActivatePage)
            {
                GetComponentInParent<UIB_Page>().DeActivate();
             //   var mainC = GameObject.Find("MainCanvas");
               /* foreach (UIB_Page page in mainC.GetComponentsInChildren<UIB_Page>())
                {
                    if (page.PageOnScreen)
                    {
                        Debug.Log("Page " + page.name + "Deactivated");
                        page.DeActivate();
                    }
                }
                */
            }

            if (VO_Select != null)
            {
                UAP_AccessibilityManager.SelectElement(VO_Select);
            }

            //Temporarily Deactivate the button when it's pressed so it can't be double clicked
            DeActivate();
            Activate();

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