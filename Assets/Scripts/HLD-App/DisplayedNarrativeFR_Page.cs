using System.Collections;
using System.Collections.Generic;
using UI_Builder;
using UnityEngine;
using UnityEngine.UI;

public class DisplayedNarrativeFR_Page : MonoBehaviour, UIB_IPage
{
    GameObject GoToListBtn;
    CameraManager DeviceCameraManager;
    
    public void Init()
    {
        GetComponent<UIB_Page>().OnActivated += PageActivatedHandler;
        GetComponent<UIB_Page>().OnDeActivated += PageDeActivatedHandler;

        DeviceCameraManager = GameObject.Find("CameraManager").GetComponent<CameraManager>();
        if (DeviceCameraManager == null)
            Debug.LogWarning("Primitive Error Handling");

        foreach (UIB_Button button in GetComponentsInChildren<UIB_Button>())
        {
            if (button.name == "DISPLAYED-Info_Button")
            {
                Debug.Log("HERE");
                button.GetComponent<Button>().onClick.AddListener(delegate {
                    DeviceCameraManager.ShutDownCamera();
                });
            }
        }

        var page = GameObject.Find("DisplayedNarrativesList_Page").GetComponent<UIB_Page>();
        foreach (UIB_Button button in page.GetComponentsInChildren<UIB_Button>())
        {
            if (button.name == "DISPLAYED-Info_Button")
            {
                Debug.Log("HERE");
                button.GetComponent<Button>().onClick.AddListener(delegate {
                    DeviceCameraManager.ShutDownCamera();
                });
            }
        }
    }

    public void PageActivatedHandler()
    {
        var page = GameObject.Find("DisplayedNarrativesList_Page").GetComponent<UIB_Page>();
        page.OnActivated +=  delegate {
            UIB_PageManager.CurrentPage = GameObject.Find("DisplayedNarrativesFR_Page");
        };

        if (DeviceCameraManager.cameraIsRunning)
            DeviceCameraManager.ResumeFaceDetection();
        else
            DeviceCameraManager.StartFaceDetection();
        GameObject.Find("DisplayedNarrativesList_Page").GetComponent<UIB_Page>().StartCoroutine("MoveScreenIn",false);
    }

    public void  PageDeActivatedHandler()
    {
        GameObject.Find("DisplayedNarrativesList_Page").GetComponent<UIB_Page>().StartCoroutine("MoveScreenOut", false);
        DeviceCameraManager.ShutDownCamera();
    }

    // Use this for initialization
    void Start()
    {
        GoToListBtn = GameObject.Find("ListOfDancersButton");
        GoToListBtn.GetComponent<Button>().onClick.AddListener(GoToList);
    }

    void GoToList()
    {
        GetComponent<Canvas>().enabled = false;
        UIB_PageManager.CurrentPage = GameObject.Find("DisplayedNarrativesList_Page");
        UIB_PageManager.LastPage = GameObject.Find("DisplayedNarrativesList_Page");
        DeviceCameraManager.PauseFaceDetection();

    }
}
