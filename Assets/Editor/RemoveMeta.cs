using UnityEditor;
using System.IO;
using UnityEngine;
using System;

public class RemoveMeta
{
    [MenuItem("Tools/Meta/Remove Frome Streaming")]
    static void RemoveMetaFromStreamingAssets()
    {
        string assetBundleDirectory = "/Users/keenangray/Desktop/StreamingAssets/";
        //NOT WORKING
        //At the end we want to clean up .meta and ?.manifest? files
        foreach (string dir in Directory.GetDirectories(assetBundleDirectory))
        {
            RemoveFromDir(dir);
        }
    }

   static void RemoveFromDir(string myDir)
    {
        foreach (string dir in Directory.GetDirectories(myDir))
        {
            RemoveFromDir(dir);

            foreach (string s in Directory.GetFiles(dir))
            {
                if (File.Exists(s))
                {
                    var filename = s.Split('/')[s.Split('/').Length - 1];
                   //Debug.Log(filename);

                    if (filename.Split('.')[s.Split('.').Length-1]==("meta"))
                    {
                        Debug.Log(s);
                        File.Delete(s);
                    }
                }
            }
        }
    }
}