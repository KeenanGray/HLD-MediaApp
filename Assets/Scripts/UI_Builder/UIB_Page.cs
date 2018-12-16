using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

namespace UI_Builder
{
    //Page Interface
    /// <summary>
    /// Declares the "OnActivated" Function which will be used by all Pages
    /// A "Page" is an instance of an App-Screen. Pages take up the entire screen. 
    /// Pages can be individually set to swipe in, at a custom speed, from the "Left" "Top" "Bottom" or "Right" of the screen.
    /// Specify "Instant" to have a page instantly appear on button press
    /// </summary>

    public interface IPage
    {
        //Pages are activated by button presses. 
        //A Page should be named <name of page>_Page
        //This matches a corresponding App_Button <name of page>_Button
        void PageActivatedHandler();
        void PageDeActivatedHandler();
    }

    //App_Page
    //App_Page implements the standard behavior for ALL pages
    //
    public class UIB_Page : MonoBehaviour, IPage
    {
        public delegate void Activated();
        public event Activated OnActivated;

        public delegate void DeActivated();
        public event Activated OnDeActivated;

        GameObject mainCanvas;
        GameObject subCanvas;

        public List<RectTransform> views;
        RectTransform rt;
        RectTransform ViewContainer;
        GameObject View_Slider;
        RectTransform CurrentView;
        UnityEngine.UI.Button close_button;
        public bool PageOnScreen;
        public float rate = 1.0f;

        public void Init()
        {
            OnActivated += new Activated(PageActivatedHandler);
            OnDeActivated += new Activated(PageDeActivatedHandler);

            views = new List<RectTransform>();

            rt = GetComponent<RectTransform>();
            if (rt == null)
            {
                Debug.LogWarning("difficulty finding rect transform attached to this gameobject");
            }

            rt.sizeDelta = new Vector2(UIB_AspectRatioManager.ScreenWidth, UIB_AspectRatioManager.ScreenHeight);

            //assign the close_button
            //gameobject must be named close_button and be a child of this gameobject
            foreach (UnityEngine.UI.Button b in GetComponentsInChildren<UnityEngine.UI.Button>())
            {
                if (b.gameObject.name.Equals("close_button"))
                    close_button = b;
            }

            if (close_button != null)
            {
                close_button.onClick.AddListener(DeActivate);
            }
            else
            {
                //            Debug.LogWarning(gameObject.name + ": You need to create a button named \"close_button\" as a child of this gameobject, otherwise the screen will never be closed");
            }

            //Get the container gameobject for each View
            if (transform.Find("Views") != null)
                ViewContainer = transform.Find("Views").GetComponent<RectTransform>();
            else
            {
                //            Debug.LogWarning("No ViewContainerOnThisPage");
                return;
            }

            //Get the View_Slider
            View_Slider = GameObject.Find("View_Slider");

            //Collect the views for the game screen
            RectTransform vrt;
            foreach (UIB_View v in GetComponentsInChildren<UIB_View>())
            {
                views.Add(v.GetComponent<RectTransform>());
                //TODO: Sort the list to ensure the screens appear in order
                vrt = v.GetComponent<RectTransform>();
                vrt.rect.Set(0, 0, UIB_AspectRatioManager.ScreenWidth, UIB_AspectRatioManager.ScreenHeight);
            }

            //Arrange the views side-by-side
            for (int i = 0; i < views.Count; i++)
            {
                vrt = views[i].GetComponent<RectTransform>();
                vrt.anchoredPosition = new Vector2(UIB_AspectRatioManager.ScreenWidth * i, 0);
            }
            if (views.Count > 0)
                CurrentView = views[0].GetComponent<RectTransform>();
        }

        private void OnDisable()
        {
            //        Debug.LogWarning("Should not be disabling gameobject " + name);
        }

        private void OnEnable()
        {
            // Debug.Log(name);
        }

        private void Start()
        {
            InputManager.SwipeDelegate += SwipeHandler;
        }

        #region SwipeHandler
        //Original swipe handler for Views
        //Unfortunately this code is not integrated with the Unity Accessibility plugin 
        //TODO: Update SwipeHandling + Views with UAP
        void SwipeHandler(SwipeData swipe)
        {
            if (PageOnScreen)
            {
                if (gameObject.activeSelf)
                {
                    if (ViewContainer != null)
                    {
                        //slide screen while user is swiping
                        if (views.IndexOf(CurrentView) == 0 || views.IndexOf(CurrentView) == views.Count)
                            ViewContainer.anchoredPosition += new Vector2(swipe.value * 1f, 0);
                        else
                            ViewContainer.anchoredPosition += new Vector2(swipe.value * 3f, 0);

                        if (swipe.full)
                        {
                            //Pull in the new screen if swipe has gone far enough
                            if (swipe.value < -100)
                            {
                                StopAllCoroutines();
                                StartCoroutine(SwitchView(Direction.LEFT));
                            }
                            else if (swipe.value > 100)
                            {
                                StopAllCoroutines();
                                StartCoroutine(SwitchView(Direction.RIGHT));
                            }
                            else
                            {
                                ViewContainer.anchoredPosition = new Vector3(-UIB_AspectRatioManager.ScreenWidth * views.IndexOf(CurrentView), 0, 0);
                            }
                        }
                    }

                    else
                    {
                        //Debug.LogWarning("Something wrong with ViewContainer " + gameObject.name);
                    }
                }
            }
        }
        #endregion

        #region ViewHandling
        //Views are pieces of pages that can be slid in and out. 
        //Views are under development and not used in the HLD app
        //TODO: extend View Handling
        private int GetNextView()
        {
            int i = views.IndexOf(CurrentView);
            if (i < views.Count - 1)
            {
                return i + 1;
            }
            else
            {
                return views.IndexOf(CurrentView);
            }
        }
        private int GetPreviousView()
        {
            int i = views.IndexOf(CurrentView);
            if (i > 0)
                return i - 1;
            else
            {
                return views.IndexOf(CurrentView);
            }
        }

        //Moves in a view at a rate based on the speed of a swipe.
        IEnumerator SwitchView(Direction dir)
        {
            if (dir == Direction.RIGHT)
            {
                var index = GetPreviousView();
                if (index < views.Count && index > -1)
                    CurrentView = views[index];
                //This looks weird, but YES these values are both supposed to be negative
                ViewContainer.anchoredPosition = new Vector3(-UIB_AspectRatioManager.ScreenWidth * views.IndexOf(CurrentView), 0, 0);
            }
            if (dir == Direction.LEFT)
            {
                var index = GetNextView();
                if (index < views.Count - 1 && index > -1)
                    CurrentView = views[index];
                //This looks weird, but YES these values are both supposed to be negative
                ViewContainer.anchoredPosition = new Vector3(-UIB_AspectRatioManager.ScreenWidth * views.IndexOf(CurrentView), 0, 0);
            }

            yield break;
        }
        #endregion

        //When a button is pressed, the app screen will slide in at a specified rate. Rate=1.0f will move instantly reveal the screen,
        //Other rates will allow the screen to slide in from the right.
        public IEnumerator MoveScreenIn(bool initializing=false)
        {
            OnActivated?.Invoke();

            float lerp = 0;
            var tmp = rate;
            Debug.Log("init " + initializing);

            if (initializing)
                tmp = 1;

            while (true)
            {
                rt.anchoredPosition = Vector3.Lerp(rt.anchoredPosition, new Vector2(0, 0), lerp);
                lerp += rate;
                if (rt.anchoredPosition == new Vector2(0, 0))
                {
                    break;
                }

                yield return null;
            }
            PageOnScreen = true;
            GetComponent<AspectRatioFitter>().enabled = true;
            yield break;
        }

        //Converse of "MoveScreenIn". When the close button is pressed the screen will move out.s
        public IEnumerator MoveScreenOut(bool initializing=false)
        {
            OnDeActivated?.Invoke();

          //  yield return new WaitForEndOfFrame();
            rt.anchoredPosition = new Vector3(0, 0, 0);
            float lerp = 0;

            var tmp = rate;
            if (initializing)
                tmp = 1;

            while (true)
            {
                rt.anchoredPosition = Vector3.Lerp(rt.anchoredPosition, new Vector3(GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<CanvasScaler>().referenceResolution.x, 0, 0), lerp);
                lerp += tmp;

                if (Mathf.Approximately(rt.anchoredPosition.x , GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<CanvasScaler>().referenceResolution.x) || 
                rt.anchoredPosition.x+lerp >= GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<CanvasScaler>().referenceResolution.x)
                {
                    break;
                }

                yield return null;
            }
            PageOnScreen = false;
            GetComponent<AspectRatioFitter>().enabled = false;
            DeActivateUAP();
            yield break;

        }

        public void DeActivate()
        {
            //        Debug.Log("Deactivating Page " + gameObject.name);
            DeActivateButtonsOnScreen();
            DeActivateUAP();

            if (ViewContainer != null)
                ViewContainer.anchoredPosition = new Vector3(0, 0, 0);

            if (views != null && views.Count > 0)
                CurrentView = views[0];

            StartCoroutine("MoveScreenOut",false);
        }

        public void PageActivatedHandler()
        {
            // Debug.Log("Page Activated " + name);
            //  gameObject.SetActive(true);
            ActivateButtonsOnScreen();
            ActivateUAP();
        }

        public void PageDeActivatedHandler()
        {
            // Debug.Log("Page De-Activated " + name);
            if (GetComponent<AccessibleUIGroupRoot>() != null)
                GetComponent<AccessibleUIGroupRoot>().m_Priority = 0;
        }

        #region Helpers
        public void SetOnScreen(bool Enabled)
        {
            PageOnScreen = Enabled;
        }

        private void ActivateButtonsOnScreen()
        {
            foreach (UnityEngine.UI.Button b in GetComponentsInChildren<UnityEngine.UI.Button>())
            {
                if (b.GetComponent<UIB_Button>() != null)
                    b.GetComponent<UIB_Button>().Activate();
            }
        }

        private void DeActivateButtonsOnScreen()
        {
            foreach (UnityEngine.UI.Button b in GetComponentsInChildren<UnityEngine.UI.Button>())
            {
                if (b.GetComponent<UIB_Button>() != null)
                    b.GetComponent<UIB_Button>().DeActivate();
            }
        }

        void ActivateUAP()
        {
            foreach (UAP_BaseElement uap in GetComponentsInChildren<UAP_BaseElement>())
            {
                uap.enabled = true;
            }
        }

        void DeActivateUAP()
        {
            foreach (UAP_BaseElement uap in GetComponentsInChildren<UAP_BaseElement>())
            {
                uap.enabled = false;
            }
        }
        #endregion
    }
}