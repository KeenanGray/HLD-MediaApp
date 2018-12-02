﻿using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Special_AccessibleLabel)), CanEditMultipleObjects]
public class Special_AccessibleLabel_Inspector : Accessibility_InspectorShared
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
		EditorGUILayout.LabelField("Label", myHeadingStyle);
		EditorGUILayout.Separator();
		
		// Name
		DrawNameSection();

		// Positioning / Traversal
		DrawPositionOrderSection();

		// Speech Output
		DrawSpeechOutputSection();


		serializedObject.ApplyModifiedProperties();
		DrawDefaultInspectorSection();
	}

}