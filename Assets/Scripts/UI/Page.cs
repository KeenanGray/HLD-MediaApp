using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Page : MonoBehaviour {

    public List<RectTransform> views;

    RectTransform rt;
    GameObject MainCanvas;
    RectTransform ViewContainer;
    GameObject View_Slider;

    float timeOfTravel = 5; //time after object reach a target place 
    float currentTime = 0; // actual floting time 
    float normalizedValue;

    RectTransform CurrentView;
    Button close_button;
    float Res;

    // Use this for initialization
    public void Initialize () {
        //TODO: prevent hardcoding of value here, use screen width insteads
        MainCanvas = GameObject.FindWithTag("MainCanvas");

        if (MainCanvas == null)
        {
            Debug.LogWarning("Canvas tagged with \"Main Canvas\" Not Found");
        }

        Res = MainCanvas.GetComponent<RectTransform>().rect.width;

        rt = GetComponent<RectTransform>();
        if (rt == null)
        {
            Debug.LogWarning("difficulty finding rect transform attached to this gameobject");
        }

        rt.anchoredPosition = new Vector3(Res, 0, 0);

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
            Debug.LogWarning(gameObject.name + ": You need to create a button named \"close_button\" as a child of this gameobject, otherwise the screen will never be closed");
        }

        //Get the container gameobject for each View
        ViewContainer = transform.Find("Views").GetComponent<RectTransform>();

        //Get the View_Slider
        View_Slider = GameObject.Find("View_Slider");

        //Collect the views for the game screen
        RectTransform vrt;
        foreach (View v in GetComponentsInChildren<View>())
        {
            views.Add(v.GetComponent<RectTransform>());
            //TODO: Sort the list to ensure the screens appear in order
            vrt = v.GetComponent<RectTransform>();
            vrt.rect.Set(0, 0, rt.rect.width, rt.rect.height);

            // Debug.Log("View: " + v.gameObject.name);
        }

        //Arrange the views side-by-side
        for (int i = 0; i < views.Count; i++)
        {
            vrt = views[i].GetComponent<RectTransform>();
            vrt.anchoredPosition = new Vector2(rt.rect.width * i, 0);
        }

        CurrentView = views[0].GetComponent<RectTransform>();

        InputManager.SwipeDelegate += SwipeHandler;

        gameObject.SetActive(false);
    }

    void SwipeHandler(SwipeData swipe)
    {
        if (gameObject.activeSelf)
        {
            //slide screen while user is swiping
            if (views.IndexOf(CurrentView) == 0 || views.IndexOf(CurrentView) == views.Count)
                ViewContainer.anchoredPosition += new Vector2(swipe.value * 1, 0);
            else
                ViewContainer.anchoredPosition += new Vector2(swipe.value * 3, 0);

            if (swipe.full)
            {
                //Pull in the new screen if swipe has gone far enough
                if (swipe.value < -100)
                {
                    StartCoroutine(SwitchView(Direction.LEFT));
                }
                else if (swipe.value > 100)
                {
                    StartCoroutine(SwitchView(Direction.RIGHT));
                }
                else
                {
                    ViewContainer.anchoredPosition = new Vector3(-Res * views.IndexOf(CurrentView), 0, 0);
                }
            }
        }
    }

    int GetNextView(){
        int i = views.IndexOf(CurrentView);
        if (i < views.Count-1)
        {
            return i + 1;
        }
        else{
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
    IEnumerator SwitchView(Direction dir){
        if(dir==Direction.RIGHT){
            CurrentView = views[GetPreviousView()];
            ViewContainer.anchoredPosition = new Vector3(-Res * views.IndexOf(CurrentView), 0, 0);
        }
        if (dir==Direction.LEFT){
            CurrentView = views[GetNextView()];
            ViewContainer.anchoredPosition = new Vector3(-Res * views.IndexOf(CurrentView), 0, 0);
        }

        yield break;
    }

    public void Activate (){
       // close_button.gameObject.SetActive(true);
    }

    void DeActivate()
    {
        StopAllCoroutines();
        StartCoroutine("MoveScreenOut");
    }


    //When a button is pressed, the app screen will slide in at a specified rate. Rate=1.0f will move instantly reveal the screen,
    //Other rates will allow the screen to slide in from the right.
    public float rate = 1.0f;
    IEnumerator MoveScreenIn()
    {
        if(rt==null){
            Debug.LogWarning("Rect Transform is null or is not activated");
        }
        rt.anchoredPosition = new Vector3(Res, 0, 0);

        float lerp = 0;

        currentTime = 0;
        while (currentTime <= timeOfTravel)
        {
            currentTime += Time.deltaTime;
            normalizedValue = currentTime / timeOfTravel; // we normalize our time 

            rt.anchoredPosition = Vector3.Lerp(rt.anchoredPosition, new Vector3(0, 0, 0), lerp);
            lerp += rate;
            if (rt.anchoredPosition == new Vector2(0, 0))
            {
                break;
            }

            yield return null;
        }
        close_button.gameObject.SetActive(true);
        yield break;
    }

    //Converse of "MoveScreenIn". When the close button is pressed the screen will move out.s
    IEnumerator MoveScreenOut()
    {
        rt.anchoredPosition = new Vector3(0, 0, 0);
        float lerp = 0;

        close_button.gameObject.SetActive(false);
        currentTime = 0;
        while (currentTime <= timeOfTravel)
        {
            rt.anchoredPosition = Vector3.Lerp(rt.anchoredPosition, new Vector3(1334, 0, 0), lerp);
            lerp += rate;

            if (rt.anchoredPosition == new Vector2(1334, 0)){
                break;
            }

            yield return null;
        }
        yield break;
    }

}
