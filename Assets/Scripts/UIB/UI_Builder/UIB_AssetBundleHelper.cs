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
            var size = bundlesLoading.Keys.Count;
            var tmpArray = new string[size];
            while (true)
            {
                var s = "";
                for (int i = 0; i < size; i++)
                {
                    try
                    {
                        bundlesLoading.Keys.CopyTo(tmpArray, 0);
                        s = tmpArray[i];
                    }
                    catch (ArgumentException e)
                    {
                        Debug.Log(e);
                        Debug.Log("size issue " + bundlesLoading.Keys.Count + " " + tmpArray.Length);
                        tmpArray = new string[bundlesLoading.Keys.Count];
                        i = 0;
                        break;
                    }
                    var newPath = Application.persistentDataPath + "/heidi-latsky-dance/" + InitializationManager.platform + s;

                    if (bundlesLoading[s])
                    {
                        continue;
                    }
                    else
                    {
                        //we need to load the asset bundle
                        yield return InitializationManager.tryLoadAssetBundle(newPath);

                        //Take all the loaded asset bundles and mark them as "true"
                        foreach (AssetBundle b in AssetBundle.GetAllLoadedAssetBundles())
                        {
                            bundlesLoading[b.name] = true;
                        }

                        //Add our new paths to the bundle arry
                        if (!bundlesLoading.ContainsKey(newPath) && bundlesLoading[s])
                        {
//                            Debug.Log("Adding entire path to bundleDict");
                            InsertAssetBundle(newPath);
                            bundlesLoading[newPath] = true;

                            StartCoroutine("LoadAssetBundlesInBackground");
                            yield break;
                        }
                        else
                        {
                        }
                    }
                }
                yield return null;

            }
        }
    }
}