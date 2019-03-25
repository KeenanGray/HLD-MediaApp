using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UI_Builder
{
    public class UIB_PlatformManager
    {
        public static string persistantDataPath;
        public static string platform;

        public static void Init()
        {
            persistantDataPath = Application.persistentDataPath;

            platform = "android/";
#if UNITY_IOS && !UNITY_EDITOR
        platform="ios/";
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
        platform = "android/";
#endif
        }

    }
}
