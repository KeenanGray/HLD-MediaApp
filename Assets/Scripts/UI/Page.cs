using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class Page : MonoBehaviour
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
    Button close_button;
    public bool PageOnScreen;

    public void Init()
    {
       OnActivated += new Activated(HandleActivated);
        OnDeActivated += new Activated(HandleDeActivated);

        views = new List<RectTransform>();

        rt = GetComponent<RectTransform>();
        if (rt == null)
        {
            Debug.LogWarning("difficulty finding rect transform attached to this gameobject");
        }

        rt.sizeDelta = new Vector2(AspectRatioManager.ScreenWidth, AspectRatioManager.ScreenHeight);

        //assign the close_button
        //gameobject must be named close_button and be a child of this gameobject
        foreach (Button b in GetComponentsInChildren<Button>())
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
        foreach (View v in GetComponentsInChildren<View>())
        {
            views.Add(v.GetComponent<RectTransform>());
            //TODO: Sort the list to ensure the screens appear in order
            vrt = v.GetComponent<RectTransform>();
            vrt.rect.Set(0, 0, AspectRatioManager.ScreenWidth, AspectRatioManager.ScreenHeight);
        }

        //Arrange the views side-by-side
        for (int i = 0; i < views.Count; i++)
        {
            vrt = views[i].GetComponent<RectTransform>();
            vrt.anchoredPosition = new Vector2(AspectRatioManager.ScreenWidth * i, 0);
        }
        if(views.Count>0)
            CurrentView = views[0].GetComponent<RectTransform>();
    }

    private void HandleDeActivated()
    {
        //Debug.Log("Default DeActivate Handler " + name);
    }

    void HandleActivated()
    {
//        Debug.Log("Default Activate Handler " + name);
    }

    private void OnDisable()
    {
        Debug.LogWarning("Should not be disabling gameobject " + name);
    }
    private void OnEnable()
    {
       // Debug.Log(name);
    }
    private void Start()
    {
        InputManager.SwipeDelegate += SwipeHandler;
    }

    void SwipeHandler(SwipeData swipe)
    {
        if(PageOnScreen){
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
                            ViewContainer.anchoredPosition = new Vector3(-AspectRatioManager.ScreenWidth * views.IndexOf(CurrentView), 0, 0);
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

    int GetNextView()
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
    int GetPreviousView()
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
            if(index<views.Count && index>-1)
                CurrentView = views[index];
            //This looks weird, but YES these values are both supposed to be negative
            ViewContainer.anchoredPosition = new Vector3(-AspectRatioManager.ScreenWidth * views.IndexOf(CurrentView), 0, 0);
        }
        if (dir == Direction.LEFT)
        {
            var index = GetNextView();
            if (index < views.Count-1 && index>-1)
                CurrentView = views[index];
            //This looks weird, but YES these values are both supposed to be negative
            ViewContainer.anchoredPosition = new Vector3(-AspectRatioManager.ScreenWidth * views.IndexOf(CurrentView), 0, 0);
        }

        yield break;
    }

    //When a button is pressed, the app screen will slide in at a specified rate. Rate=1.0f will move instantly reveal the screen,
    //Other rates will allow the screen to slide in from the right.
    public float rate = 1.0f;
    public IEnumerator MoveScreenIn()
    {
        if(OnActivated!=null)
            OnActivated();

     //  gameObject.SetActive(true);
        ActivateButtonsOnScreen();
        ActivateUAP();
        if (rt == null)
        {
            Debug.LogWarning("Rect Transform is null or is not activated");
        }
        rt.anchoredPosition = new Vector3(AspectRatioManager.ScreenWidth, 0, 0);

        float lerp = 0;

        while (true)
        {
            rt.anchoredPosition = Vector3.Lerp(rt.anchoredPosition, new Vector3(0, 0, 0), lerp);
            lerp += rate;
            if (rt.anchoredPosition == new Vector2(0, 0))
            {
                break;
            }

            yield return null;
        }
        PageOnScreen = true;
        yield break;
    }

    //Converse of "MoveScreenIn". When the close button is pressed the screen will move out.s
    public IEnumerator MoveScreenOut()
    {
        if (OnDeActivated != null)
            OnDeActivated();

        if (GetComponent<AccessibleUIGroupRoot>() != null)
            GetComponent<AccessibleUIGroupRoot>().m_Priority = 0;

        yield return new WaitForEndOfFrame();
        rt.anchoredPosition = new Vector3(0, 0, 0);
        float lerp = 0;

        while (true)
        {
            rt.anchoredPosition = Vector3.Lerp(rt.anchoredPosition, new Vector3(AspectRatioManager.ScreenWidth, 0, 0), lerp);
            lerp += rate;

            if (rt.anchoredPosition == new Vector2(AspectRatioManager.ScreenWidth, 0))
            {
                break;
            }

            yield return null;
        }
        PageOnScreen = false;
        DeActivateUAP();
        yield break;

    }

    public void DeActivate()
    {
//        Debug.Log("Deactivating Page " + gameObject.name);
        DeActivateButtonsOnScreen();
        DeActivateUAP();

        if(ViewContainer!=null)
            ViewContainer.anchoredPosition = new Vector3(0, 0, 0);

        if(views!=null&&views.Count>0)
            CurrentView = views[0];

        StartCoroutine("MoveScreenOut");
    }

    public void SetOnScreen(bool Enabled){
        PageOnScreen = Enabled;
        }

    void ActivateButtonsOnScreen()
    {
        foreach (Button b in GetComponentsInChildren<Button>())
        {
            if(b.GetComponent<App_Button>()!=null)
                b.GetComponent<App_Button>().Activate();
        }
    }
    void DeActivateButtonsOnScreen()
    {
        foreach (Button b in GetComponentsInChildren<Button>())
        {
            if (b.GetComponent<App_Button>() != null)
                b.GetComponent<App_Button>().DeActivate();
        }
    }

    void DeActivateUAP(){
        foreach(UAP_BaseElement uap in GetComponentsInChildren<UAP_BaseElement>()){
            uap.enabled = false;
        }
    }

    void ActivateUAP()
    {
        foreach (UAP_BaseElement uap in GetComponentsInChildren<UAP_BaseElement>())
        {
            uap.enabled = true;
        }
    }
}
