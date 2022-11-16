using UnityEditor;
using System.IO;
using UnityEngine;
using System;

public class CreateAssetBundles
{
    [MenuItem("Tools/AssetBundles/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        GameObject accessManager = null;
        //disabling accessibility manager is necessary to prevent issues
        try
        {
            accessManager = GameObject.Find("Accessibility Manager");
            accessManager.SetActive(false);
        }
        catch (Exception e)
        {
            Debug.LogWarning("no accessibility manager " + e);
        }

        string assetBundleDirectory = Application.streamingAssetsPath;
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        if (!Directory.Exists(assetBundleDirectory + "/ios"))
        {
            Directory.CreateDirectory(assetBundleDirectory + "/ios");
        }

        
        if (!Directory.Exists(assetBundleDirectory + "/android"))
        {
            Directory.CreateDirectory(assetBundleDirectory + "/android");
        }
        

        //Chunk based compression
        // BuildPipeline.BuildAssetBundles(assetBundleDirectory + "/ios",
        // BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.iOS);


        // BuildPipeline.BuildAssetBundles(assetBundleDirectory + "/android",
        // BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);

         BuildPipeline.BuildAssetBundles(assetBundleDirectory + "/android",
         BuildAssetBundleOptions.None, BuildTarget.Android);
         
        //default compression
         BuildPipeline.BuildAssetBundles(assetBundleDirectory + "/ios",
         BuildAssetBundleOptions.None, BuildTarget.iOS);




        //reenable accessibility manager at end
        try
        {
            accessManager.SetActive(true);
            accessManager.SetActive(false);
            accessManager.SetActive(true);
            accessManager.SetActive(true);
        }
        catch (Exception e)
        {
            Debug.LogWarning("no accessibility manager " + e);
        }
    }
}