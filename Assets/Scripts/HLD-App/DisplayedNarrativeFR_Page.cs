using System.Collections;
using System.Collections.Generic;
using UI_Builder;
using UnityEngine;
using UnityEngine.UI;

public class DisplayedNarrativeFR_Page : MonoBehaviour, UIB_IPage
{
    GameObject GoToListBtn;
    CameraManager DeviceCameraManager;

    public bool ShouldRecreatePages;

    public void Init()
    {
        GetComponent<UIB_Page>().OnActivated += PageActivatedHandler;
        GetComponent<UIB_Page>().OnDeActivated += PageDeActivatedHandler;

        ShouldRecreatePages = false;
        DeviceCameraManager = GameObject.Find("CameraManager").GetComponent<CameraManager>();
        if (DeviceCameraManager == null)
            Debug.LogWarning("Primitive Error Handling");
    }

    public void PageActivatedHandler()
    {
        DeviceCameraManager.StartFaceDetection();
        GameObject.Find("DisplayedNarrativesList_Page").GetComponent<UIB_Page>().StartCoroutine("MoveScreenIn",false);
    }

    public void  PageDeActivatedHandler()
    {
        if(!ShouldRecreatePages)
            GameObject.Find("DisplayedNarrativesList_Page").GetComponent<UIB_Page>().StartCoroutine("MoveScreenOut", false);

        DeviceCameraManager.EndFaceDetection();

        ShouldRecreatePages = false;
    }

    // Use this for initialization
    void Start()
    {
        GoToListBtn = GameObject.Find("ListOfDancersButton");
        GoToListBtn.GetComponent<Button>().onClick.AddListener(GoToList);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GoToList()
    {
        ShouldRecreatePages = true;
        GetComponent<UIB_Page>().DeActivate();
    }
}
