using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UI_Builder;
using System.Collections.Generic;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ObjdetectModule;
using OpenCVForUnity.FaceModule;
using OpenCVForUnity.UnityUtils;

#if UNITY_5_3 || UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif
using OpenCVForUnity;

namespace HLD
{
    enum DancersFromPhotos
    {
        CarmenSchoenster,
        PeterTrojic,
        LouisaMann,
        ChrisBraz,
        JillianHollis,
        JerronHerman,
        DesmondCadogan,
        MeredithFages,
        AmyMeisner,
        LeslieTaub
    }

    /// <summary>
    /// WebCamTextureToMat Example
    /// An example of converting a WebCamTexture image to OpenCV's Mat format.
    /// </summary>
    public class FaceDetection : MonoBehaviour
    {
        /// <summary>
        /// Set the name of the device to use.
        /// </summary>
        [SerializeField, TooltipAttribute("Set the name of the device to use.")]
        public string requestedDeviceName = null;

        /// <summary>
        /// Set the width of WebCamTexture.
        /// </summary>
        [SerializeField, TooltipAttribute("Set the width of WebCamTexture.")]
        public int requestedWidth = 100;

        /// <summary>
        /// Set the height of WebCamTexture.
        /// </summary>
        [SerializeField, TooltipAttribute("Set the height of WebCamTexture.")]
        public int requestedHeight = 100;

        /// <summary>
        /// Set FPS of WebCamTexture.
        /// </summary>
        [SerializeField, TooltipAttribute("Set FPS of WebCamTexture.")]
        public int requestedFPS = 30;

        /// <summary>
        /// Set whether to use the front facing camera.
        /// </summary>
        [SerializeField, TooltipAttribute("Set whether to use the front facing camera.")]
        public bool requestedIsFrontFacing = false;

        /// <summary>
        /// The webcam texture.
        /// </summary>
        public WebCamTexture webCamTexture;

        /// <summary>
        /// The webcam device.
        /// </summary>
        WebCamDevice webCamDevice;

        /// <summary>
        /// The rgba mat.
        /// </summary>
        Mat rgbaMat;

        /// <summary>
        /// The colors.
        /// </summary>
        Color32[] colors;

        /// <summary>
        /// The texture.
        /// </summary>
        Texture2D texture;

        /// <summary>
        /// Indicates whether this instance is waiting for initialization to complete.
        /// </summary>
        bool isInitWaiting = false;

        /// <summary>
        /// Indicates whether this instance has been initialized.
        /// </summary>
        bool hasInitDone = false;

        CascadeClassifier haarCascade;
        FaceRecognizer recognizer;

        MatOfRect faces;
        MatOfRect faces2;

        Point point;
        Point point2;

        Size size;
        Size size2;

        Mat gray;
        Mat gray2;
        Size sizeA;
        Size sizeB;

        int id;
        double[] conf;
        int[] label;

        public bool shouldRecognize = false;
        public bool isRunning;
        /// <summary>
        /// The FPS monitor.
        /// </summary>
        // FpsMonitor fpsMonitor;

        // Use this for initialization
        void Start()
        {
            //Initialize();
        }

        /// <summary>
        /// Initializes webcam texture.
        /// </summary>
        private void Initialize()
        {
            if (isInitWaiting)
                return;

            haarCascade = new CascadeClassifier();
            // haarCascade.load(Utils.getFilePath("lbpcascade_frontalface.xml"));
            haarCascade.load(Utils.getFilePath("haarcascade_frontalface_alt.xml"));

            recognizer = OpenCVForUnity.FaceModule.LBPHFaceRecognizer.create();
            recognizer.read(Utils.getFilePath("trainer.yml"));

            faces = new MatOfRect();
            faces2 = new MatOfRect();

            point = new Point();
            size = new Size();
            point2 = new Point();
            size2 = new Size();

            sizeA = new Size();
            sizeB = new Size();

            gray = new Mat();
            gray2 = new Mat();

            label = new int[1];
            conf = new double[1];

            //(int)(requestedHeight * (UIB_AspectRatioManager.ScreenHeight / UIB_AspectRatioManager.ScreenWidth));

#if UNITY_ANDROID && !UNITY_EDITOR
            // Set the requestedFPS parameter to avoid the problem of the WebCamTexture image becoming low light on some Android devices. (Pixel, pixel 2)
            // https://forum.unity.com/threads/android-webcamtexture-in-low-light-only-some-models.520656/
            // https://forum.unity.com/threads/released-opencv-for-unity.277080/page-33#post-3445178
            if (requestedIsFrontFacing) {
                int rearCameraFPS = requestedFPS;
                requestedFPS = 15;
                StartCoroutine (_Initialize ());
                requestedFPS = rearCameraFPS;
            } else {
                StartCoroutine (_Initialize ());
            }
#else
            StartCoroutine(_Initialize());
#endif
        }

        /// <summary>
        /// Initializes webcam texture by coroutine.
        /// </summary>
        private IEnumerator _Initialize()
        {
            if (hasInitDone)
                Dispose();

            isInitWaiting = true;

            // Creates the camera
            if (!String.IsNullOrEmpty(requestedDeviceName))
            {
                int requestedDeviceIndex = -1;
                if (Int32.TryParse(requestedDeviceName, out requestedDeviceIndex))
                {
                    if (requestedDeviceIndex >= 0 && requestedDeviceIndex < WebCamTexture.devices.Length)
                    {
                        Debug.Log(1);
                        webCamDevice = WebCamTexture.devices[requestedDeviceIndex];
                        webCamTexture = new WebCamTexture(webCamDevice.name, requestedWidth, requestedHeight, requestedFPS);
                    }
                }
                else
                {
                    for (int cameraIndex = 0; cameraIndex < WebCamTexture.devices.Length; cameraIndex++)
                    {
                        if (WebCamTexture.devices[cameraIndex].name == requestedDeviceName)
                        {
                            Debug.Log(2);
                            webCamDevice = WebCamTexture.devices[cameraIndex];
                            webCamTexture = new WebCamTexture(webCamDevice.name, requestedWidth, requestedHeight, requestedFPS);
                            break;
                        }
                    }
                }
                if (webCamTexture == null)
                    Debug.LogWarning("Cannot find camera device " + requestedDeviceName + ".");
            }

            if (webCamTexture == null)
            {

                // Checks how many and which cameras are available on the device
                for (int cameraIndex = 0; cameraIndex < WebCamTexture.devices.Length; cameraIndex++)
                {
                    if (WebCamTexture.devices[cameraIndex].isFrontFacing == requestedIsFrontFacing)
                    {
                        Debug.Log(3);
                        webCamDevice = WebCamTexture.devices[cameraIndex];
                        webCamTexture = new WebCamTexture(webCamDevice.name, requestedWidth * 10, requestedHeight * 10, requestedFPS);
                        webCamTexture.requestedHeight = 1920;
                        webCamTexture.requestedWidth = 10;
                        break;
                    }
                }
            }

            if (webCamTexture == null)
            {

                if (WebCamTexture.devices.Length > 0)
                {
                    //                Debug.Log(4);
                    webCamDevice = WebCamTexture.devices[0];
                    // webCamTexture = new WebCamTexture(webCamDevice.name, requestedWidth, requestedHeight, requestedFPS);
                    webCamTexture = new WebCamTexture(webCamDevice.name, 0, 0, requestedFPS);
                    webCamTexture.requestedHeight = 1200;
                    webCamTexture.requestedWidth = 1920;
                }
                else
                {
                    Debug.LogError("Camera device does not exist.");
                    isInitWaiting = false;
                    yield break;
                }
            }


            // Starts the camera.
            webCamTexture.Play();

            while (true)
            {
                // If you want to use webcamTexture.width and webcamTexture.height on iOS, you have to wait until webcamTexture.didUpdateThisFrame == 1, otherwise these two values will be equal to 16. (http://forum.unity3d.com/threads/webcamtexture-and-error-0x0502.123922/).
#if UNITY_IOS && !UNITY_EDITOR && (UNITY_4_6_3 || UNITY_4_6_4 || UNITY_5_0_0 || UNITY_5_0_1)
                if (webCamTexture.width > 16 && webCamTexture.height > 16) {
#else
                if (webCamTexture.didUpdateThisFrame)
                {
#if UNITY_IOS && !UNITY_EDITOR && UNITY_5_2
                    while (webCamTexture.width <= 16) {
                        webCamTexture.GetPixels32 ();
                        yield return new WaitForEndOfFrame ();
                    } 
#endif
#endif
                    Debug.Log("name:" + webCamTexture.deviceName + " width:" + webCamTexture.width + " height:" + webCamTexture.height + " fps:" + webCamTexture.requestedFPS);
                    Debug.Log("videoRotationAngle:" + webCamTexture.videoRotationAngle + " videoVerticallyMirrored:" + webCamTexture.videoVerticallyMirrored + " isFrongFacing:" + webCamDevice.isFrontFacing);

                    isInitWaiting = false;
                    hasInitDone = true;

                    OnInited();

                    yield break;
                }
                else
                {
                    yield return null;
                }
            }
        }

        /// <summary>
        /// Releases all resource.
        /// </summary>
        private void Dispose()
        {
            isInitWaiting = false;
            hasInitDone = false;

            if (webCamTexture != null)
            {
                webCamTexture.Stop();
                WebCamTexture.Destroy(webCamTexture);
                webCamTexture = null;
            }
            if (rgbaMat != null)
            {
                rgbaMat.Dispose();
                rgbaMat = null;
            }
            if (texture != null)
            {
                texture = null;
            }
        }

        /// <summary>
        /// Raises the webcam texture initialized event.
        /// </summary>
        private void OnInited()
        {
            if (colors == null || colors.Length != webCamTexture.width * webCamTexture.height)
                colors = new Color32[webCamTexture.width * webCamTexture.height];
            if (texture == null || texture.width != webCamTexture.width || texture.height != webCamTexture.height)
                texture = new Texture2D(webCamTexture.width, webCamTexture.height, TextureFormat.RGBA32, false);

            rgbaMat = new Mat(webCamTexture.height, webCamTexture.width, CvType.CV_8UC4);

            gameObject.GetComponent<Renderer>().material.mainTexture = texture;
            //  gameObject.transform.localScale = new Vector3(webCamTexture.width, webCamTexture.height, 1);
            gameObject.transform.localScale = new Vector3(webCamTexture.width / 3, webCamTexture.height / 3, 1);
            //        Debug.Log("Screen.width " + Screen.width + " Screen.height " + Screen.height + " Screen.orientation " + Screen.orientation);

            float width = rgbaMat.width();
            float height = rgbaMat.height();

            float widthScale = (float)Screen.width / width;
            float heightScale = (float)Screen.height / height;
            if (widthScale < heightScale)
            {
                Camera.main.orthographicSize = (width * (float)Screen.height / (float)Screen.width) / 2;
            }
            else
            {
                Camera.main.orthographicSize = height / 2;
            }

            isRunning = true;
            StartCoroutine("DetectFaces");

        }

        // Update is called once per frame
        void Update()
        {
        }

        /// <summary>
        /// Raises the destroy event.
        /// </summary>
        void OnDestroy()
        {
            Dispose();
        }

        /// <summary>
        /// Raises the back button click event.
        /// </summary>
        public void OnBackButtonClick()
        {
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
            SceneManager.LoadScene("OpenCVForUnityExample");
#else
            Application.LoadLevel ("OpenCVForUnityExample");
#endif
        }

        /// <summary>
        /// Raises the play button click event.
        /// </summary>
        public void OnPlayButtonClick()
        {
            if (hasInitDone)
                webCamTexture.Play();
        }

        /// <summary>
        /// Raises the pause button click event.
        /// </summary>
        public void OnPauseButtonClick()
        {
            if (hasInitDone)
                webCamTexture.Pause();
        }

        /// <summary>
        /// Raises the stop button click event.
        /// </summary>
        public void OnStopButtonClick()
        {
            if (hasInitDone)
                webCamTexture.Stop();
        }

        public void CamInit()
        {
            Initialize();
        }

        public void BeginRecognizer()
        {
            //        Debug.Log("Cam beginning");
            shouldRecognize = true;

            if (webCamTexture == null)
            {
                StopCoroutine("DetectFaces");
                Initialize();
            }

            /*    if (!webCamTexture.isPlaying)
                 {
                     webCamTexture.Play();
                 }
                 */
        }

        public void EndRecognizer()
        {
            shouldRecognize = false;
        }

        public void ShutDownCamera()
        {
            Debug.Log("cam shutting down:");
            webCamTexture.Stop();
            Dispose();
        }

        Dictionary<int, int> facesDetected;
        IEnumerator DetectFaces()
        {
            facesDetected = new Dictionary<int, int>();
            while (true)
            {
                //            Debug.Log("ShouldRecognize " + shouldRecognize);
                if (hasInitDone && webCamTexture.isPlaying && webCamTexture.didUpdateThisFrame)
                {
                    Utils.webCamTextureToMat(webCamTexture, rgbaMat, colors);

                    if (shouldRecognize)
                    {
                        // Detect faces
                        sizeA.width = 124;
                        sizeA.height = 124;
                        OpenCVForUnity.ImgprocModule.Imgproc.cvtColor(rgbaMat, gray, OpenCVForUnity.ImgprocModule.Imgproc.COLOR_BGR2GRAY);
                        haarCascade.detectMultiScale(gray, faces, 1.3f, 3, 0, // TODO: objdetect.CV_HAAR_SCALE_IMAGE
                                                        sizeA, sizeB);

                        // Render all detected faces
                        //vertical faces
                        foreach (OpenCVForUnity.CoreModule.Rect face in faces.toArray())
                        {
                            if (recognizer != null)
                            {
                                recognizer.predict(gray.submat(face), label, conf);

                                if (conf[0] > 25)
                                {
                                    // Debug.Log("recognized " + Enum.GetNames(typeof(DancersFromPhotos))[label[0]] + label[0] + " with confidence " + (int)conf[0]);
                                    CountRecognizedFaces(label[0]);
                                    //OpenPageFromFace(label[0]);
                                }

                                //Create a circle around the player

                                point.x = (int)(face.x + face.width * 0.5);
                                point.y = (int)(face.y + face.height * 0.5);

                                size.width = (int)(face.width * 0.5);
                                size.height = (int)(face.height * 0.5);

                                //   OpenCVForUnity.Imgproc.ellipse(rgbaMat, point, size, 0, 0, 360, new Scalar(255, 255, 255, 128), 4);

                            }
                            else
                                Debug.Log("recongizer is null");

                            yield return null;
                        }


                        //horizontal faces
                        OpenCVForUnity.CoreModule.Core.rotate(gray, gray2, OpenCVForUnity.CoreModule.Core.ROTATE_90_CLOCKWISE);


                        haarCascade.detectMultiScale(gray2, faces2, 1.3f, 3, 0, // TODO: objdetect.CV_HAAR_SCALE_IMAGE
                                                        sizeA, sizeB);

                        foreach (OpenCVForUnity.CoreModule.Rect face2 in faces2.toArray())
                        {
                            if (recognizer != null)
                            {
                                recognizer.predict(gray2.submat(face2), label, conf);

                                if (conf[0] > 30)
                                {
                                    // Debug.Log("recognized " + Enum.GetNames(typeof(DancersFromPhotos))[label[0]] + label[0] + " with confidence " + (int)conf[0]);
                                    CountRecognizedFaces(label[0]);
                                    //OpenPageFromFace(label[0]);
                                }

                                //Create a circle around the player

                                point2.y = (int)(face2.x + face2.width * 0.5);
                                point2.x = (int)(face2.y + face2.height * 0.5);

                                size2.width = (int)(face2.width * 0.5);
                                size2.height = (int)(face2.height * 0.5);

                                // OpenCVForUnity.Imgproc.ellipse(rgbaMat, point2, size2, 0, 0, 360, new Scalar(255, 0, 0, 150), 4);

                            }
                            else
                                Debug.Log("recongizer is null");

                            yield return null;
                        }
                    }
                    Utils.matToTexture2D(rgbaMat, texture, colors);
                }
                yield return null;
            }
        }

        private void OpenPageFromFace(int label)
        {
            var dancer = Enum.GetNames(typeof(DancersFromPhotos))[label];
            Debug.Log("Dancer " + dancer);

            switch (label)
            {
                default:
                    if (GameObject.Find(dancer + "_Button") != null)
                    {
                        GameObject.Find(dancer + "_Button").GetComponent<Button>().onClick.Invoke();
                    }
                    else
                    {
                        Debug.LogWarning("Page not found.");
                    }
                    break;
            }

            //  var DeviceCameraManager = GameObject.Find("CameraManager").GetComponent<CameraManager>();
            EndRecognizer();

        }

        int facesRequiredToScan = 18;
        int cnt = 0;
        void CountRecognizedFaces(int label)
        {
            cnt++;
            if (facesDetected.ContainsKey(label))
            {
                Debug.Log("label " + label);
                facesDetected[label] = facesDetected[label] + 1;
                Debug.Log("faces detected " + Enum.GetNames(typeof(DancersFromPhotos))[label - 1] + " count: " + facesDetected[label]);
            }
            else
            {
                facesDetected.Add(label, 0);
            }

            if (facesDetected[label] > facesRequiredToScan) //lucky number 7
            {
                OpenPageFromFace(label - 1);
                facesDetected.Clear();
            }

            if (cnt > 60)
            {
                cnt = 0;
                facesDetected.Clear();
            }


        }
    }
}