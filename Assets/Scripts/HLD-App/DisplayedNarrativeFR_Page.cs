using System.Collections;
using System.Collections.Generic;
using UI_Builder;
using UnityEngine;
using UnityEngine.UI;

public class DisplayedNarrativeFR_Page : MonoBehaviour, UIB_IPage
{
    GameObject GoToListBtn;
    FaceDetectionHLD fdHLD;

    public void Init()
    {
        GetComponent<UIB_Page>().OnActivated += PageActivatedHandler;
        GetComponent<UIB_Page>().OnDeActivated += PageDeActivatedHandler;

        var tmp = GameObject.Find("CameraViewTexture");
        if (tmp != null)
            fdHLD = tmp.GetComponent<FaceDetectionHLD>();

        if (fdHLD == null)
            Debug.LogWarning("Warning:No Camera Manager is present for face recognition");

        foreach (UIB_Button button in GetComponentsInChildren<UIB_Button>())
        {
            if (button.name == "DISPLAYED-Info_Button")
            {
                button.GetComponent<Button>().onClick.AddListener(delegate
                {
                    if (fdHLD != null)
                        fdHLD.ShutDownCamera();
                });
            }
        }

        var page = GameObject.Find("DisplayedNarrativesList_Page").GetComponent<UIB_Page>();
        foreach (UIB_Button button in page.GetComponentsInChildren<UIB_Button>())
        {
            if (button.name == "DISPLAYED-Info_Button")
            {
                button.GetComponent<Button>().onClick.AddListener(delegate
                {
                    if (fdHLD != null)
                        fdHLD.ShutDownCamera();
                });
            }
        }
    }

    public void PageActivatedHandler()
    {
        if (UAP_AccessibilityManager.IsActive())
        {
            var page = GameObject.Find("DisplayedNarrativesList_Page").GetComponent<UIB_Page>();
            page.GetComponent<UIB_Page>().StartCoroutine("MoveScreenIn", false);
            GoToList();
        }
        else
        {
            var page = GameObject.Find("DisplayedNarrativesList_Page").GetComponent<UIB_Page>();
            page.GetComponent<UIB_Page>().StartCoroutine("MoveScreenIn", false);

            page.OnActivated += delegate
          {
              UIB_PageManager.CurrentPage = GameObject.Find("DisplayedNarrativesFR_Page");
          };

            if (fdHLD != null)
            {
                if (fdHLD.isRunning)
                    fdHLD.BeginRecognizer();
                else
                    Debug.Log("here is something");
            }
        }
    }

    public void PageDeActivatedHandler()
    {
        GameObject.Find("DisplayedNarrativesList_Page").GetComponent<UIB_Page>().StartCoroutine("MoveScreenOut", false);

        if (fdHLD != null)
            fdHLD.ShutDownCamera();
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

        if (fdHLD != null)
            fdHLD.EndRecognizer();

    }
}
