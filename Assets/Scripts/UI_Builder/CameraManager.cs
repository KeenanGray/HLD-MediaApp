using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    GameObject FR;
    FaceDetectionHLD fd;

    // Use this for initialization
    void Start()
    {
        FR = GameObject.Find("FaceRecognizer");

        if (FR != null)
        {
            fd = FR.GetComponent<FaceDetectionHLD>();
            if (fd == null)
            {
                Debug.LogWarning("No object with this name");
            }
        }
        else
        {
            Debug.LogWarning("Bad at handling errors");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartFaceDetection()
    {
        fd.StartCamera();
    }

    public void EndFaceDetection()
    {
        fd.EndCamera();
    }

}
