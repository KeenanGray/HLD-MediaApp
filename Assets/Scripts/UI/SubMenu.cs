using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;

public class SubMenu : MonoBehaviour
{
    RectTransform rt;
    GameObject MainCanvas;
    GameObject ScrollView;

    [ExecuteInEditMode]
    public void Init()
    {
        //TODO: prevent hardcoding of value here, use screen width insteads
        MainCanvas = GameObject.FindWithTag("MainCanvas");

        if (MainCanvas == null)
        {
            Debug.LogWarning("Canvas tagged with \"Main Canvas\" Not Found");
        }

        rt = GetComponent<RectTransform>();
        if (rt == null)
        {
            Debug.LogWarning("difficulty finding rect transform attached to this gameobject");
        }

        rt.sizeDelta = new Vector2(AspectRatioManager.ScreenWidth, AspectRatioManager.ScreenHeight);

     //for each button in the submenu
        //add a listener to deactivate the submenu onclick
        foreach (Button b in GetComponentsInChildren<Button>())
        {
            b.onClick.AddListener(DeActivate);
        }

        GetComponent<EventTrigger>().triggers.Clear();

        //TODO:Error handle this
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener((eventData) => { DeActivate(); });

        GetComponent<EventTrigger>().triggers.Add(entry);

        ScrollView = GetComponentInChildren<ScrollRect>().gameObject;
        ScrollView.GetComponent<EventTrigger>().triggers.Add(entry);

        foreach (EventTrigger et in ScrollView.GetComponentsInChildren<EventTrigger>())
        {
            et.GetComponent<EventTrigger>().triggers.Add(entry);
        }

        rt.anchoredPosition = new Vector3(AspectRatioManager.ScreenWidth, 0, 0);
       // DeActivate();
    }

    public void Activate (){

    }


    public void DeActivate()
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
        rt.anchoredPosition = new Vector3(AspectRatioManager.ScreenWidth, 0, 0);

        float lerp = 0;

        while (true)
        {
            rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, new Vector3(0, 0), lerp);
            lerp += rate;
            if (rt.anchoredPosition == new Vector2(0, 0))
            {
                break;
            }

            yield return null;
        }
        yield break;
    }

    //Converse of "MoveScreenIn". When the close button is pressed the screen will move out.s
    IEnumerator MoveScreenOut()
    {
        rt.anchoredPosition = new Vector3(0, 0, 0);
        float lerp = 0;
      //  currentTime = 0;

        while (true)
        {
            rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, new Vector2(AspectRatioManager.ScreenWidth, 0), lerp);
            lerp += rate;

            if (rt.anchoredPosition == new Vector2(AspectRatioManager.ScreenWidth, 0)){
                break;
            }

            yield return null;
        }
        gameObject.SetActive(false);
        yield break;
    }

}
