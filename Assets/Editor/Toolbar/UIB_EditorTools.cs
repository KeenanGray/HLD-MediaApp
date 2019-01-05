using UnityEngine;
using System.Collections;
using UnityEditor;
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


}