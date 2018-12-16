using UnityEngine;
using System.Collections;
using UnityEditor;
using UI_Builder;

public class AppTools : ScriptableWizard
{
    public string searchTag = "Your tag here";

    [MenuItem("App Tools/KeenanGray...")]
    static void Example1()
    {
        Debug.Log("You pressed the example");
    }

   
}