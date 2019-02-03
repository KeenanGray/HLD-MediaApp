using UnityEngine;
using System.Collections;
using UnityEditor;
using UI_Builder;
using System;

public class UIB_Tools_Window : EditorWindow
{
    static string localPath = "Assets/UIB/Prefabs/";

    static string assetName = "UIB_Page.prefab";
    static string pageName;
    static bool shouldMakePage;

    public static void Init()
    {
        // Get existing open window or if none, make a new one:
        UIB_Tools_Window window = (UIB_Tools_Window)EditorWindow.GetWindow(typeof(UIB_Tools_Window));

        window.titleContent.text = "UIB Tools";
        window.position = new Rect(50 + 0, 50 + 0, 400, 285);
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical(GUILayout.MaxWidth(75.0f), GUILayout.MaxHeight(100.0f));

        //Label for tool
        GUILayout.Label("This is a UI Building plugin for making mobile apps in Unity");

        //Field of Page Generator
        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
        pageName = EditorGUILayout.TextField("Page Title", pageName);
        EditorGUILayout.EndHorizontal();

        //
        if (GUILayout.Button("Create Page", GUILayout.MaxWidth(100.0f), GUILayout.MaxHeight(20.0f)))
        {
            Debug.Log("Clicked Button");
            CreateUIB_Page(pageName);
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create Button", GUILayout.MaxWidth(100.0f), GUILayout.MaxHeight(20.0f)))
        {
            Debug.Log("Clicked Button");
            CreateUIB_Button(pageName);
        }

        EditorGUILayout.LabelField("Make a page with this button?");
        shouldMakePage = EditorGUILayout.Toggle(shouldMakePage);

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

    }

    private void CreateUIB_Button(string page_Name)
    {
        assetName = "UIB_Button.prefab";
        string assetPath = localPath + assetName;

        if (UnityEditor.Selection.transforms.Length <= 0)
        {
            Debug.LogWarning("Select the object you want to attach the button to");
            return;
        }

        GameObject tmp = (UnityEngine.GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)));
        tmp.transform.SetParent(UnityEditor.Selection.transforms[0].transform);
        tmp.name = page_Name + "_Button";

        if (shouldMakePage)
            CreateUIB_Page(page_Name);
    }

    void CreateUIB_Page(string page_name)
    {
        assetName = "UIB_Page.prefab";
        string assetPath = localPath + assetName;

        //Check if the Prefab and/or name already exists at the path
        if (AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)))
        {
            /*
             * //Create dialog to ask if User is sure they want to overwrite existing Prefab
            if (EditorUtility.DisplayDialog("Are you sure?",
                "The Prefab already exists. Do you want to overwrite it?",
                "Yes",
                "No"))

             */

            //If the user presses the yes button, create the Prefab
            {
                GameObject tmp = (UnityEngine.GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)));
                tmp.transform.SetParent(GameObject.Find("Pages").transform);
                tmp.name = page_name+"_Page";
            }
        }
        else
        {
            Debug.Log("No Prefab found");
        }
    }
}