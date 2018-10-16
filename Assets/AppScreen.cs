using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppScreen : MonoBehaviour {

    RectTransform rt;

    float timeOfTravel = 5; //time after object reach a target place 
    float currentTime = 0; // actual floting time 
    float normalizedValue;

    Button close_button;

    // Use this for initialization
    void Start () {
        //TODO: prevent hardcoding of value here, use screen width insteads
        Debug.Log("Screen Width " + ResolutionManager.GetCurrentResolution().width);

        rt = GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector3(1334, 0, 0);

        //assign the close_button
        //gameobject must be named close_button and be a child of this gameobject
        foreach (Button b in GetComponentsInChildren<Button>())
        {
            Debug.Log("foreach");
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


    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void DeActivate()
    {
       StartCoroutine("MoveScreenOut");
    }

    //TODO:Fix the coroutine not getting to 'yield break' fast enough
    IEnumerator MoveScreenIn()
    {
        currentTime = 0;
        while (currentTime <= timeOfTravel)
        {
            currentTime += Time.deltaTime;
            normalizedValue = currentTime / timeOfTravel; // we normalize our time 

            rt.anchoredPosition = Vector3.Lerp(rt.anchoredPosition, new Vector3(0, 0, 0), normalizedValue);

            if (rt.anchoredPosition == new Vector2(0, 0))
            {
                break;
            }

            yield return null;
        }
        close_button.gameObject.SetActive(true);
        yield break;
    }

    IEnumerator MoveScreenOut()
    {
        close_button.gameObject.SetActive(false);
        currentTime = 0;
        while (currentTime <= timeOfTravel)
        {
            currentTime += Time.deltaTime;
            normalizedValue = currentTime / timeOfTravel; // we normalize our time 

            rt.anchoredPosition = Vector3.Lerp(rt.anchoredPosition, new Vector3(1334, 0, 0), normalizedValue);

            if (rt.anchoredPosition == new Vector2(1334, 0)){
                break;
            }

            yield return null;
        }
        yield break;
    }

}
