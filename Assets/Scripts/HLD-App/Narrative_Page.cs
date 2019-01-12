using System.Collections;
using System.Collections.Generic;
using UI_Builder;
using UnityEngine;

public class Narrative_Page : MonoBehaviour, UIB_IPage {

    GameObject backButton;
    public void Init()
    {
        GetComponent<UIB_Page>().OnActivated += PageActivatedHandler;
        GetComponent<UIB_Page>().OnDeActivated += PageDeActivatedHandler;

        backButton = transform.Find("Interface").Find("DisplayedNarrativesFR_Button").gameObject;
        if (backButton == null)
            Debug.LogWarning("Bad error checking");
    }

    //When the narrative page is activated, we want to set the back button to either the 
    //list of dancer's, or the camera face recognizer
    public void PageActivatedHandler()
    {
        backButton.name = UIB_PageManager.LastPage.name.Split('_')[0]+"_Button";
    }

    public void PageDeActivatedHandler()
    {

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
