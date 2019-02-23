using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI_Builder
{
    public class UIB_AssetBundleHelper : MonoBehaviour
    {
        public static Dictionary<string, bool> bundlesLoading;

        public static void InsertAssetBundle(string s)
        {
            if (bundlesLoading == null)
            {
                bundlesLoading = new Dictionary<string, bool>();
            }

            if (bundlesLoading.ContainsKey(s))
            {
                //The key is already in the dictionary
            }
            else
            {
                //we have not put the key in the dictionary
               //Debug.Log("adding " + s);
                bundlesLoading.Add(s, false);
            }
    
        }

        public IEnumerator LoadAssetBundlesInBackground()
        {
            while (true)
            {
                Debug.Log("Waiting for asset bundles");
                try
                {
                    foreach (string s in bundlesLoading.Keys)
                    {
                        //Debug.Log("s is " + s);
                    }
                }
                catch (InvalidOperationException e)
                {
                    StartCoroutine("LoadAssetBundlesInBackground");
                    yield break;
                }

                foreach (string s in bundlesLoading.Keys)
                {
                    var newPath = Application.persistentDataPath + "/heidi-latsky-dance/" + InitializationManager.platform + s;
                    if (bundlesLoading[s])
                    {
                        //we have the asset bundle loaded.
                    }
                    else
                    {
                        //we need to load the asset bundle
//                        Debug.Log("trying to load bundle " + newPath);
                        yield return InitializationManager.tryLoadAssetBundle(newPath);

                        foreach (AssetBundle b in AssetBundle.GetAllLoadedAssetBundles())
                        {
                            bundlesLoading[b.name] = true;

                            if (!bundlesLoading.ContainsKey(newPath))
                            {
                                InsertAssetBundle(newPath);
                                bundlesLoading[newPath] = true;

                                StartCoroutine("LoadAssetBundlesInBackground");
                                yield break;

                            }
                        }
                    }
                }
                yield return null;
            }
        }
    }
}