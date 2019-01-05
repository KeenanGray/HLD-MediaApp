using UnityEngine;
using System.Collections;
using UnityEditor;
using UI_Builder;
using System;

public class UIB_Tools_Window : EditorWindow
{

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
        GUILayout.Label("This is a UI Building plugin for making mobile apps in Unity" );
        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
        EditorGUILayout.LabelField("Page Title"); EditorGUILayout.TextField("...");
        EditorGUILayout.EndHorizontal();
        GUILayout.Button("Create Page", GUILayout.MaxWidth(100.0f), GUILayout.MaxHeight(20.0f));
        EditorGUILayout.EndVertical();
    }
}