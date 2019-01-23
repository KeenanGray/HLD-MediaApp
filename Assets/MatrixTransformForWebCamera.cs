using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatrixTransformForWebCamera : MonoBehaviour
{
    // Attach to an object that has a Renderer component,
    // and use material with the shader below.
    public float rotateSpeed = 30f;
    public void Update()
    {
        if (GetComponent<FaceDetectionHLD>().webCamTexture != null){ 

        WebCamTexture wct = GetComponent<FaceDetectionHLD>().webCamTexture;
            GameObject displayCamera;

            Quaternion rot = Quaternion.Euler(0, 0, 0);

            // Construct a rotation matrix and set it for the shader
            if (wct != null) {
                if (wct.videoRotationAngle == 0)
                {
                    rot = Quaternion.Euler(0, 0, 180);
                }
                else if (wct.videoRotationAngle == 90)
                {
                    rot = Quaternion.Euler(0, 0, (180));
                }
            }
            else
            {
                Debug.LogWarning("texture is null");
            }

            Matrix4x4 m = Matrix4x4.TRS(new Vector3(0, 1, 0), rot, new Vector3(-1, -1, 1));
            GetComponent<Renderer>().material.SetMatrix("_TextureRotation", m);
        }
    }
}
