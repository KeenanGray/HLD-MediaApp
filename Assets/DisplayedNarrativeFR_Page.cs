using System.Collections;
using System.Collections.Generic;
using UI_Builder;
using UnityEngine;

public class DisplayedNarrativeFR_Page : MonoBehaviour, UIB_IPage
{

    CameraManager DeviceCameraManager;
    public void Init()
    {
        GetComponent<UIB_Page>().OnActivated += PageActivatedHandler;
        GetComponent<UIB_Page>().OnDeActivated += PageDeActivatedHandler;

        DeviceCameraManager = GameObject.Find("CameraManager").GetComponent<CameraManager>();
        if (DeviceCameraManager == null)
            Debug.LogWarning("Primitive Error Handling");
    }

    public void PageActivatedHandler()
    {
        DeviceCameraManager.StartFaceDetection();
    }

    public void  PageDeActivatedHandler()
    {
        DeviceCameraManager.EndFaceDetection();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
