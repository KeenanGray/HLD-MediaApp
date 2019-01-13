using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using OpenCvSharp.Face;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using NWH;
using System;
using UI_Builder;

enum DancersFromPhotos
{
    LouisaMann,
    PeterTrojic
}

public class FaceDetectionHLD : MonoBehaviour
{
    public bool editorStart;

    public RawImage rawImage;
    public int cameraIndex = 0;

    private WebCamTexture webCamTexture;
    private CascadeClassifier haarCascade;
    private Texture2D tex;
    private Mat mat;

    private bool initialized = false;

    LBPHFaceRecognizer recognizer;
    bool recognize;

    private void Start()
    {
        recognize = false;
    }

    public void BeginRecognizer()
    {
        recognize = true;
    }
    public void EndRecognizer()
    {
        recognize = false;
    }

    void Update()
    {
        if (editorStart)
        {
            editorStart = false;
            StartCoroutine("StartInEditor");
        }

        // Wait for camera to return the first frame and then grab the correct width and height.
        if (!initialized && CvUtil.CameraReturnedFirstFrame(webCamTexture))
        {
            // Initialize mat and tex. Avoid doing this in Update() due to high cost of GC.
            tex = new Texture2D(webCamTexture.width, webCamTexture.height, TextureFormat.RGBA32, false);
            mat = new Mat(webCamTexture.height, webCamTexture.width, MatType.CV_8UC4);

            // Load file needed for face detection.
            haarCascade = new CascadeClassifier(
                CvUtil.GetStreamingAssetsPath("haarcascade_frontalface_alt2.xml"));

            initialized = true;
        }
        else
        {
            // Only call webcam update if needed
            if (webCamTexture != null && webCamTexture.didUpdateThisFrame && webCamTexture.isPlaying)
            {
                CamUpdate();
            }
        }
    }

    void CamUpdate()
    {
        // Get Mat from WebCamTexture
        CvUtil.GetWebCamMat(webCamTexture, ref mat);

        // Run face detection
        mat = DetectFace(haarCascade, mat);

        // Convert Mat to Texture2D for display
        CvConvert.MatToTexture2D(mat, ref tex);
        if (rawImage == null)
            Debug.LogWarning("Raw Image has not been assigned");

        if (rawImage != null)
        {
            // Assign Texture2D to GUI element
            rawImage.texture = tex;

            rawImage.GetComponent<RectTransform>().sizeDelta = new Vector2(rawImage.texture.width, rawImage.texture.height);
        }

    }

    /// <summary>
    /// Initialize new WebCamTexture
    /// </summary>
    public void CamInit()
    {
        CvUtil.CheckIfCameraExists();
        // Do not try to stop WebCamTexture if it does not exist
        if (webCamTexture != null)
            webCamTexture.Stop();

        // Initialize new texture with requested device
        webCamTexture = new WebCamTexture(WebCamTexture.devices[cameraIndex].name);

        recognizer = OpenCvSharp.Face.LBPHFaceRecognizer.Create();
        recognizer.Read(CvUtil.GetStreamingAssetsPath("trainer.yml"));

        // Start playback
        webCamTexture.Play();
    }

    public void CamShutdown()
    {
        if (webCamTexture != null)
            webCamTexture.Stop();
        rawImage.texture = null;
    }

    /// <summary>
    /// Switches between multiple cameras if they exist.
    /// </summary>
    public void ChangeCamera()
    {
        cameraIndex++;

        if (cameraIndex >= WebCamTexture.devices.Length)
            cameraIndex = 0;

        CamInit();
    }

    /// <summary>
    /// Run face detection using Haar Cascades.
    /// </summary>
    private Mat DetectFace(CascadeClassifier cascade, Mat src)
    {
        Mat result;

        using (var gray = new Mat())
        {
            result = src.Clone();

            if (recognize)
            {
                Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

                // Detect faces
                OpenCvSharp.Rect[] faces = cascade.DetectMultiScale(
                    gray, 1.08, 3, HaarDetectionType.ScaleImage, new Size(124, 124));

                // Render all detected faces
                foreach (OpenCvSharp.Rect face in faces)
                {
                    int id = -1;
                    double conf = -1;
                    int label = -1;

                    using (var just = gray[face])
                    {
                        recognizer.Predict(just, out label, out conf);
                    }

                    if (conf > 60.0f)
                    {
                        //                    Debug.Log(label + " " + conf + " " + id);
                        OpenPageFromFace(label);
                    }

                    var center = new Point
                    {
                        X = (int)(face.X + face.Width * 0.5),
                        Y = (int)(face.Y + face.Height * 0.5)
                    };
                    var axes = new Size
                    {
                        Width = (int)(face.Width * 0.5),
                        Height = (int)(face.Height * 0.5)
                    };
                    Cv2.Ellipse(result, center, axes, 0, 0, 360, new Scalar(255, 255, 255, 128), 4);
                }
            }
            else
            {
               // Debug.Log("recognizer is off");
            }
        }
        return result;
    }

    private void OpenPageFromFace(int label)
    {
        var dancer = Enum.GetNames(typeof(DancersFromPhotos))[label];
        Debug.Log("Dancer " + dancer);

        switch (label)
        {
            case 0:
                GameObject.Find(dancer + "_Button").GetComponent<Button>().onClick.Invoke();
                // GetComponent<UIB_Page>().StartCoroutine("MoveScreenIn", false);
                //GameObject.Find("DisplayedNarrativesFR_Page").GetComponent<UIB_Page>().DeActivate();
                break;
            case 1:
                GameObject.Find(dancer + "_Button").GetComponent<Button>().onClick.Invoke();
                // GameObject.Find(dancer + "_Page").GetComponent<UIB_Page>().StartCoroutine("MoveScreenIn", false);
                // GameObject.Find("DisplayedNarrativesFR_Page").GetComponent<UIB_Page>().DeActivate();
                break;
            default:
                break;
        }

        var DeviceCameraManager = GameObject.Find("CameraManager").GetComponent<CameraManager>();
        DeviceCameraManager.PauseFaceDetection();

    }

    private void OnDestroy()
    {
        if (webCamTexture != null) webCamTexture.Stop();
    }

}
