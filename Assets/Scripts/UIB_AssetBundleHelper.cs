using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;

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

            Debug.Log("count is " + AssetBundle.GetAllLoadedAssetBundles().Count() + " total is " + AssetBundleList.Count);
            while (AssetBundle.GetAllLoadedAssetBundles().Count() < AssetBundleList.Count)
            {
                Debug.Log("While True");
                for (int i = 0; i < size; i++)
                {
                    var str_name = AssetBundleList[i];
                    var newPath = UIB_PlatformManager.persistentDataPath + UIB_PlatformManager.platform + str_name;
#if UNITY_ANDROID && !UNITY_EDITOR
                    //this newPath points to the streaming assets path on android
                   // newPath = UIB_PlatformManager.persistentDataPath + "android/assets/" + UIB_PlatformManager.platform + str_name;

                   //this path points to the persistant data path, where correct asset bundles are stored
                    newPath = UIB_PlatformManager.persistentDataPath + UIB_PlatformManager.platform + str_name;
                    Debug.Log("New path:" + newPath);

#endif
                    //we need to load the asset bundle
                    yield return tryLoadAssetBundle(newPath);
                }
                yield return null;
            }
            Debug.Log("Done Loading Asset Bundles");
            InitializationManager.doneLoadingAssetBundles = true;
            yield break;
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

            bundleLoadRequest = AssetBundle.LoadFromFileAsync(path);

            AssetBundle myLoadedAssetBundle = bundleLoadRequest.assetBundle;

            if (myLoadedAssetBundle == null)
            {
                Debug.LogWarning("Failed to load AssetBundle " + path);
                yield break;
            }

            string bundle_name = path.Split("hld")[1];

            //if the loaded bundle is hld/general, update the url links
            //initialize the url buttons
            if (bundle_name == "/general")
            {
                foreach (
                    AssignUrlFromAssetBundle e in FindObjectsOfType(
                        typeof(AssignUrlFromAssetBundle)
                    )
                )
                {
                    e.UpdateURL();
                }
            }

            yield break;
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            AssetBundleList.Clear();

            foreach (AssetBundle ab in AssetBundle.GetAllLoadedAssetBundles())
            {
                ab.Unload(true);
            }
        }
    }
}
