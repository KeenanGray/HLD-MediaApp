//c# Example (App_Button_Editor.cs)
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace UI_Builder
{
    [CustomEditor(typeof(UIB_Button))]
    [CanEditMultipleObjects]
    public class UIB_Button_Editor : Editor
    {
        SerializedProperty Opens;
        SerializedProperty VO_Select;
        SerializedProperty NewPage;
        SerializedProperty myText;
        SerializedProperty BackButton;

        public Object source;
        void OnEnable()
        {
            Opens = serializedObject.FindProperty("Button_Opens");
            NewPage = serializedObject.FindProperty("newScreen");
            VO_Select = serializedObject.FindProperty("VO_Select");
            myText = serializedObject.FindProperty("myText");
            BackButton = serializedObject.FindProperty("isBackButton");

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(VO_Select, new GUIContent("Voiceover select"));
            EditorGUILayout.PropertyField(Opens, new GUIContent("Button Opens"));
            EditorGUILayout.PropertyField(BackButton, new GUIContent("Is Back Button"));


            if (Opens.enumDisplayNames[Opens.enumValueIndex] == UI_Builder.UIB_Button.UIB_Button_Activates.Website.ToString())
            {
                EditorGUILayout.PropertyField(myText, new GUIContent("Url"));
                serializedObject.ApplyModifiedProperties();
                return;
            }

            if (Opens.enumDisplayNames[Opens.enumValueIndex] == UI_Builder.UIB_Button.UIB_Button_Activates.Scene.ToString())
            {
                EditorGUILayout.PropertyField(myText, new GUIContent("Scene Name"));
                serializedObject.ApplyModifiedProperties();
                return;
            }

            if (Opens.enumDisplayNames[Opens.enumValueIndex] == UI_Builder.UIB_Button.UIB_Button_Activates.Accessibletext.ToString())
            {
                //EditorGUILayout.PropertyField(myText, new GUIContent("Text to Say"));
                EditorStyles.textField.wordWrap = true;
                myText.stringValue = EditorGUILayout.TextArea(myText.stringValue);
      
        serializedObject.ApplyModifiedProperties();
                return;
            }
//            Debug.Log("Opens " + Opens.enumDisplayNames[Opens.enumValueIndex]);
            if (Opens.enumDisplayNames[Opens.enumValueIndex] == UI_Builder.UIB_Button.UIB_Button_Activates.None.ToString())
            {
                EditorGUILayout.HelpBox("Button is not set to open anything", MessageType.Info);
                serializedObject.ApplyModifiedProperties();
                return;
            }

            if (Opens.enumDisplayNames[Opens.enumValueIndex] == UI_Builder.UIB_Button.UIB_Button_Activates.Video.ToString())
            {
                EditorGUILayout.HelpBox("Button Will Open a Video", MessageType.Info);
                serializedObject.ApplyModifiedProperties();
                return;
            }

            UI_Builder.UIB_Button myTarget = (UI_Builder.UIB_Button)target;

            if (myTarget.Button_Opens == UI_Builder.UIB_Button.UIB_Button_Activates.SpecificPage)
            {
                EditorGUILayout.PropertyField(NewPage, new GUIContent("New Screen"));
            }
            else if (myTarget.Button_Opens == UI_Builder.UIB_Button.UIB_Button_Activates.Page)
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
}