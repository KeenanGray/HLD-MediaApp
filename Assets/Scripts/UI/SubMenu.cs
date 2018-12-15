﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;


[RequireComponent(typeof(EventTrigger))]
public class SubMenu : MonoBehaviour
{
    RectTransform rt;
    GameObject ScrollView;
    bool MenuOnScreen;

    public void Init()
    {
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
            //     b.onClick.AddListener(DeActivate);
        }

        GetComponent<EventTrigger>().triggers.Clear();

        //TODO:Error handle this
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener((eventData) => { DeActivate(); });

        ScrollView = GetComponentInChildren<ScrollRect>().gameObject;
        //    ScrollView.GetComponent<EventTrigger>().triggers.Add(entry);

        foreach (EventTrigger et in ScrollView.GetComponentsInChildren<EventTrigger>())
        {
            //   et.GetComponent<EventTrigger>().triggers.Add(entry);
        }

        foreach (App_Button ab in GetComponentsInChildren<App_Button>())
        {
            ab.GetComponent<Button>().onClick.AddListener(DeActivate);
        }
    }

    private void Start()
    {
    }

    //When a button is pressed, the app screen will slide in at a specified rate. Rate=1.0f will move instantly reveal the screen,
    //Other rates will allow the screen to slide in from the right.
    public float rate = 1.0f;
    IEnumerator MoveScreenIn()
    {
        gameObject.SetActive(true);

        ActivateButtonsOnScreen();
        if (rt == null)
        {
            Debug.LogWarning("Rect Transform is null or is not activated");
        }
#if !UNITY_EDITOR
        rt.anchoredPosition = new Vector3(AspectRatioManager.ScreenWidth, 0, 0);
#endif
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
        MenuOnScreen = true;
        yield break;
    }

    public void DeActivate(){
        StartCoroutine("MoveScreenOut");
    }

    //Converse of "MoveScreenIn". When the close button is pressed the screen will move out.s
    public IEnumerator MoveScreenOut()
    {
      // yield return new WaitForEndOfFrame();
        DeActivateButtonsOnScreen();

        rt.anchoredPosition = new Vector3(0, 0, 0);
        float lerp = 0;
      //  currentTime = 0;

        while (true)
        {
            rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, new Vector2(GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<CanvasScaler>().referenceResolution.x, 0), lerp);
            lerp += rate;

            if (rt.anchoredPosition == new Vector2(GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<CanvasScaler>().referenceResolution.x, 0)){
                break;
            }

            yield return null;
        }
        MenuOnScreen = false;
        yield break;
    }

    public void SetOnScreen(bool Enabled)
    {
        MenuOnScreen = Enabled;
    }

    void ActivateButtonsOnScreen(){
        foreach(Button b in GetComponentsInChildren<Button>()){
            b.GetComponent<App_Button>().Activate();
        }
    }
    void DeActivateButtonsOnScreen()
    {
        foreach (Button b in GetComponentsInChildren<Button>())
        {
            b.GetComponent<App_Button>().DeActivate();
        }
    }
}
