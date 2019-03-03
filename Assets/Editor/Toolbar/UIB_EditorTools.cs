using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UI_Builder;

public class UIB_EditorTools : ScriptableWizard
{
    [MenuItem("Tools/UIB/Page Count...")]
    static void Example1()
    {
        Debug.Log("There are {pagecount} UIB pages in this scene");
    }

    [MenuItem("Tools/UIB/Tools Window...")]
    static void Example2()
    {
        UIB_Tools_Window.Init();
        // UIB_Tools_Window.Init();
        //Debug.Log("There are {pagecount} UAB pages in this scene");
    }

    [MenuItem("Tools/Build Mobile")]
    public static void MyBuild()
    {
        //Android Build
        BuildPlayerOptions androidPlayerOptions = new BuildPlayerOptions();

        PlayerSettings.Android.keystoreName = "/Users/keenangray/Docs/AndroidKeystore/keystore.keystore";
        PlayerSettings.Android.keystorePass = "michigan45";
        PlayerSettings.Android.keyaliasName = "hld";
        PlayerSettings.Android.keyaliasPass = "michigan45";

        androidPlayerOptions.scenes = new[] { "Assets/Scenes/AppScene.unity", "Assets/Scenes/BluetoothSim.unity" };
        androidPlayerOptions.locationPathName = "Builds/AndroidBuild.apk";
        androidPlayerOptions.target = BuildTarget.Android;
        androidPlayerOptions.options = BuildOptions.None;

        BuildReport androidReport = BuildPipeline.BuildPlayer(androidPlayerOptions);
        BuildSummary androidSummary = androidReport.summary;

        if (androidSummary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + androidSummary.totalSize + " bytes");
        }

        if (androidSummary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
            return;

        }

        //IOS Build
        BuildPlayerOptions iosBuildPlayerOptions = new BuildPlayerOptions();
        iosBuildPlayerOptions.scenes = new[] { "Assets/Scenes/AppScene.unity", "Assets/Scenes/BluetoothSim.unity" };
        iosBuildPlayerOptions.locationPathName = "Builds/iOSBuild";
        iosBuildPlayerOptions.target = BuildTarget.iOS;
        iosBuildPlayerOptions.options = BuildOptions.AcceptExternalModificationsToPlayer;

        BuildReport iosReport = BuildPipeline.BuildPlayer(iosBuildPlayerOptions);
        BuildSummary iosSummary = iosReport.summary;

        if (iosSummary.result == BuildResult.Succeeded)
        {
            Debug.Log("IOS Build succeeded: " + iosSummary.totalSize + " bytes");
        }

        if (iosSummary.result == BuildResult.Failed)
        {
            Debug.Log("IOS Build failed");
        }
    }
}