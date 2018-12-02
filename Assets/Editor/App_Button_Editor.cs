//c# Example (App_Button_Editor.cs)
using UnityEngine;
using UnityEditor;
using static App_Button;
using UnityEngine.UI;

[CustomEditor(typeof(App_Button))]
[CanEditMultipleObjects]
public class App_Button_Editor : Editor
{
    SerializedProperty Opens;
    SerializedProperty VO_Select;
    SerializedProperty NewPage;
    SerializedProperty WebUrl;

    public Object source;
    void OnEnable()
    {
        Opens = serializedObject.FindProperty("Button_Opens");
        NewPage = serializedObject.FindProperty("newScreen");
        VO_Select = serializedObject.FindProperty("VO_Select");
        WebUrl = serializedObject.FindProperty("WebUrl");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(VO_Select, new GUIContent("Voiceover select"));
        EditorGUILayout.PropertyField(Opens, new GUIContent("Button Opens"));

        if (Opens.enumDisplayNames[Opens.enumValueIndex] == Button_Activates.Website.ToString())
        {
            EditorGUILayout.PropertyField(WebUrl, new GUIContent("Url"));
            serializedObject.ApplyModifiedProperties();
            return;
        }

        if (Opens.enumDisplayNames[Opens.enumValueIndex] == Button_Activates.None.ToString())
        {
            EditorGUILayout.HelpBox("Button is not set to open anything",MessageType.Info);
            serializedObject.ApplyModifiedProperties();
            return;
        }

        if (Opens.enumDisplayNames[Opens.enumValueIndex] == Button_Activates.Video.ToString())
        {
            EditorGUILayout.HelpBox("Button Will Open a Video", MessageType.Info);
            serializedObject.ApplyModifiedProperties();
            return;
        }
         App_Button myTarget = (App_Button)target;

        if (myTarget.Button_Opens == Button_Activates.SpecificPage)
        {
            EditorGUILayout.PropertyField(NewPage, new GUIContent("New Screen"));
        }
        else
        {
            var screenName = myTarget.gameObject.name.ToString().Split('_')[0];
            var typeName = Opens.enumDisplayNames[Opens.enumValueIndex].Replace(" ", "");
            screenName = screenName + ("_" + typeName);
            var PageObject = GameObject.Find(screenName);

            if (PageObject != null)
            {
                myTarget.newScreen = PageObject;

                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField("New Page", myTarget.newScreen, typeof(GameObject), true);
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                EditorGUILayout.HelpBox("No Object Found - " + screenName, MessageType.Warning);
            }
        }
        
        serializedObject.ApplyModifiedProperties();


    }
}