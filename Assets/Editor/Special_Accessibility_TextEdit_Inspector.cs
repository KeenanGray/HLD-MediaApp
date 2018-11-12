using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Special_AccessibleTextEdit)), CanEditMultipleObjects]
public class Special_Accessibility_TextEdit_Inspector : Accessibility_InspectorShared
{
	//////////////////////////////////////////////////////////////////////////

	void OnEnable()
	{
	}

	//////////////////////////////////////////////////////////////////////////

	public override void OnInspectorGUI()
	{
		SetupGUIStyles();
		serializedObject.Update();

		EditorGUILayout.Separator();
		EditorGUILayout.LabelField("Text Edit", myHeadingStyle);
		EditorGUILayout.Separator();

		// Name
		DrawNameSection();

		// Reference / Target 
		DrawTargetSection("Text Field");

		// Positioning / Traversal
		DrawPositionOrderSection();

		// Speech Output
		DrawSpeechOutputSection();


		serializedObject.ApplyModifiedProperties();
		DrawDefaultInspectorSection();
	}

}
