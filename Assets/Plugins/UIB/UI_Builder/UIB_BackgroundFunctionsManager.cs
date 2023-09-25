using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIB_BackgroundFunctionsManager : MonoBehaviour
{
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            //TODO: run background behaviors

#if UNITY_ANDROID && !UNITY_EDITOR
            Debug.LogAssertion("Android Pause");
#endif

#if UNITY_IOS && !UNITY_EDITOR
            Debug.LogAssertion("iOS Pause");
#endif
        }
        else
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            Debug.LogAssertion("Android unPause");
#endif

#if UNITY_IOS && !UNITY_EDITOR
            Debug.LogAssertion("iOS unPause");
#endif
        }
    }
}
