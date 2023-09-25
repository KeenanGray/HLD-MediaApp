using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UI_Builder;
using System.IO;

public class UIB_EditorTools : ScriptableWizard
{
    [MenuItem("Tools/UIB/Page Count...")]
    static void Example1()
    {
        Debug.LogAssertion("There are {pagecount} UIB pages in this scene");
    }

    [MenuItem("Tools/UIB/Tools Window...")]
    static void Example2()
    {
        UIB_Tools_Window.Init();
    }

    [MenuItem("Tools/Build Mobile")]
    public static void BuildMobile()
    {
        //Android Build
        BuildPlayerOptions androidPlayerOptions = new BuildPlayerOptions();

        PlayerSettings.Android.keystoreName =
            "/Users/keenangray/Docs/AndroidKeystore/keystore.keystore";
        PlayerSettings.Android.keystorePass = "michigan45";
        PlayerSettings.Android.keyaliasName = "hld";
        PlayerSettings.Android.keyaliasPass = "michigan45";

        androidPlayerOptions.scenes = new[] { "Assets/Scenes/AppScene.unity" };
        androidPlayerOptions.locationPathName = "Builds/AndroidBuild.apk";
        androidPlayerOptions.target = BuildTarget.Android;
        androidPlayerOptions.options = BuildOptions.None;

        BuildReport androidReport = BuildPipeline.BuildPlayer(androidPlayerOptions);
        BuildSummary androidSummary = androidReport.summary;

        if (androidSummary.result == BuildResult.Succeeded)
        {
            Debug.LogAssertion("Android Build succeeded: " + androidSummary.totalSize + " bytes");
        }

        if (androidSummary.result == BuildResult.Failed)
        {
            Debug.LogAssertion("Android Build failed");
            return;
        }

        //IOS Build
        BuildPlayerOptions iosBuildPlayerOptions = new BuildPlayerOptions();
        iosBuildPlayerOptions.scenes = new[] { "Assets/Scenes/AppScene.unity" };
        iosBuildPlayerOptions.locationPathName = "Builds/iOSBuild";
        iosBuildPlayerOptions.target = BuildTarget.iOS;
        iosBuildPlayerOptions.options = BuildOptions.AcceptExternalModificationsToPlayer;

        BuildReport iosReport = BuildPipeline.BuildPlayer(iosBuildPlayerOptions);
        BuildSummary iosSummary = iosReport.summary;

        if (iosSummary.result == BuildResult.Succeeded)
        {
            Debug.LogAssertion("IOS Build succeeded: " + iosSummary.totalSize + " bytes");
        }

        if (iosSummary.result == BuildResult.Failed)
        {
            Debug.LogAssertion("IOS Build failed");
        }
    }

    [MenuItem("Tools/Delete Player Prefs")]
    public static void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    [MenuItem("Tools/AssetBundles/PrepAssetBundlesForS3Upload")]
    public static void CleanFolderForS3()
    {
        CleanHelper(Application.streamingAssetsPath);

        CopyHelper(Application.streamingAssetsPath);

        CleanHelper(Application.persistentDataPath + "/android");
        CleanHelper(Application.persistentDataPath + "/ios");

        var path = Application.persistentDataPath; // + "/heidi-latsky-dance/";
        EditorUtility.RevealInFinder(path);

        Debug.LogAssertion("Clean Successful");
    }

    static void CleanHelper(string dir)
    {
        foreach (string d in Directory.GetDirectories(dir))
        {
            CleanHelper(d);

            foreach (string file in Directory.GetFiles(d))
            {
                if (
                    file.Contains(".meta")
                    || file.Contains(".manifest")
                    || file.Contains(".DS_Store")
                )
                {
                    File.Delete(file);
                }
            }
        }
    }

    static void CopyHelper(string dir)
    {
        //for each directory in streaming assets path
        foreach (string d in Directory.GetDirectories(dir))
        {
            //copy the file from the streaming assets directory to persistent data
            var directory = "";
            CopyHelper(d);

            //start by getting each of the files
            foreach (string file in Directory.GetFiles(d))
            {
                var cont = 0;
                var name = "";
                //get the name of the file, by splitting it into many parts.
                //and taking the full directory after the streaming assets path
                foreach (string i in file.Split('/'))
                {
                    cont++;
                    if (cont >= file.Split('/').Length)
                    {
                        name = file.Replace(Application.streamingAssetsPath, "/");
                        break;
                    }
                    else { }
                }

                //there is ane xtra '/' in the front here, remove it
                name = name.Remove(0, 1);

                directory =
                    Application.persistentDataPath
                    + name.Replace(name.Split('/')[name.Split('/').Length - 1], "");

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                var src = Application.streamingAssetsPath + UIB_PlatformManager.platform + name;
                var dest = Application.persistentDataPath + name;

                try
                {
                    if (!File.Exists(dest))
                        File.Copy(src, dest);
                }
                catch
                {
                    Debug.LogWarning("src:" + src);
                    Debug.LogWarning("dest:" + dest);
                }
            }
        }
    }
}
