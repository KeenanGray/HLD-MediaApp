using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class AppScreen : MonoBehaviour {

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

    // Use this for initialization
    void Start () {
        //TODO: prevent hardcoding of value here, use screen width insteads
        MainCanvas = GameObject.Find("AppCanvas");

        if(MainCanvas!=null){
            Debug.LogWarning("App Canvas Not Found");
        }

        Debug.Log("Screen Width " + MainCanvas.GetComponent<RectTransform>().rect.width);
        var Res = MainCanvas.GetComponent<RectTransform>().rect.width;

        rt = GetComponent<RectTransform>();
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
            close_button.gameObject.SetActive(false);
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
        foreach (View v in GetComponentsInChildren<View>()){
            views.Add(v.GetComponent<RectTransform>());
            //TODO: Sort the list to ensure the screens appear in order
            vrt = v.GetComponent<RectTransform>();
            vrt.rect.Set(0,0,rt.rect.width,rt.rect.height);

           // Debug.Log("View: " + v.gameObject.name);
        }

        //Arrange the views side-by-side
        for (int i=0; i < views.Count;i++){
            vrt = views[i].GetComponent<RectTransform>();

            vrt.anchoredPosition = new Vector2(rt.rect.width * i,0);
        }
        CurrentView = views[0].GetComponent<RectTransform>();

        InputManager.SwipeDelegate += SwipeHandler;
    }

    void SwipeHandler(SwipeData swipe)
    {
        //slide screen while user is swiping
        ViewContainer.anchoredPosition += new Vector2(swipe.value, 0);

        if(swipe.full){
            //Pull in the new screen if swipe has gone far enough
            Debug.Log("rt width " + ViewContainer.anchoredPosition.x + ". Next view " + GetNextView() * rt.rect.width);
            if(ViewContainer.anchoredPosition.x < (GetNextView() * rt.rect.width) ){
                Debug.Log("Swipe was strong enough");
            }
            else{
                Debug.Log("Swipe not strong enough");
            }

            StartCoroutine("SwitchView");
        }
    }

    int GetNextView(){
        int i = views.IndexOf(CurrentView);
        if (i < views.Count)
            return i + 1;
        else{
            return 0;
        }
    }
    //Moves in a view at a rate based on the speed of a swipe.
    IEnumerator SwitchView(){

        yield break;
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
        var Res = MainCanvas.GetComponent<RectTransform>().rect.width;
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
