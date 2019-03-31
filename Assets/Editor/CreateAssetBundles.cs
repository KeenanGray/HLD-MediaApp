﻿using UnityEditor;
using System.IO;
using UnityEngine;

public class CreateAssetBundles
{
    [MenuItem("Tools/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        string assetBundleDirectory = Application.streamingAssetsPath;
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        if (!Directory.Exists(assetBundleDirectory + "/ios"))
        {
            Directory.CreateDirectory(assetBundleDirectory+"/ios");
        }
        if (!Directory.Exists(assetBundleDirectory+"/android"))
        {
            Directory.CreateDirectory(assetBundleDirectory+"/android");
        }


        BuildPipeline.BuildAssetBundles(assetBundleDirectory + "/ios", 
        BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.iOS);


        BuildPipeline.BuildAssetBundles(assetBundleDirectory + "/android",
  BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);
    }
}