﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UI_Builder
{
    public class UIB_AssetBundleHelper : MonoBehaviour
    {

        static List<string> AssetBundleList;

        public static void InsertAssetBundle(string s)
        {
            if (AssetBundleList == null)
            {
                AssetBundleList = new List<string>();
                AssetBundleList.Add(s);
            }
            else
                AssetBundleList.Add(s);
        }

        public IEnumerator LoadAssetBundlesInBackground()
        {
            var size = AssetBundleList.Count;

            while (true)
            {
                for (int i = 0; i < size; i++)
                {
                    var str_name = AssetBundleList[i];
                    var newPath = UIB_PlatformManager.persistentDataPath + UIB_PlatformManager.platform + str_name;
#if UNITY_ANDROID && !UNITY_EDITOR
                    newPath = UIB_PlatformManager.persistentDataPath +"android/assets/"+ UIB_PlatformManager.platform + str_name; 
#endif
                    //we need to load the asset bundle
                    yield return tryLoadAssetBundle(newPath);
                }
                yield return null;

            }
        }
        public static IEnumerator tryLoadAssetBundle(string path)
        {
            AssetBundleCreateRequest bundleLoadRequest = null;
            if (!File.Exists(path))
            {
                Debug.LogWarning("file does not exist:" + path);
                yield break;
            }

            foreach (AssetBundle ab in AssetBundle.GetAllLoadedAssetBundles())
            {
                var p = path.Split(UIB_PlatformManager.platform)[1];
                if (ab.name == p)
                    yield break;
            }

            try
            {
                bundleLoadRequest = AssetBundle.LoadFromFileAsync(path);

                AssetBundle myLoadedAssetBundle = bundleLoadRequest.assetBundle;

                if (myLoadedAssetBundle == null)
                {
                    Debug.LogWarning("Failed to load AssetBundle " + path);
                }

                string bundle_name = path.Split("hld")[1];

                //if the loaded bundle is hld/general, update the url links
                //initialize the url buttons
                if (bundle_name == "/general")
                {
                    foreach (AssignUrlFromAssetBundle e in FindObjectsOfType(typeof(AssignUrlFromAssetBundle)))
                    {
                        e.UpdateURL();
                    }
                }
            }
            catch
            {
                Debug.LogWarning("EXCEPTION: But this should be resolved when the app relaods");
            }

            yield break;
        }



        private void OnDisable()
        {
            StopAllCoroutines();

            foreach (AssetBundle ab in AssetBundle.GetAllLoadedAssetBundles())
            {
                //Debug.Log("unloading asset bundle:" + ab.name);
                ab.Unload(true);
            }
        }


    }
}