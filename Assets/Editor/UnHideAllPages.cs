using UnityEngine;
using System.Collections;
using UnityEditor;

public class AppTools : ScriptableWizard
{

    public string searchTag = "Your tag here";

    [MenuItem("App Tools/UnHideAllPages...")]
    static void UnHideAllPages()
    {
        foreach(Page p in FindObjectsOfType<Page>()){
            p.Init();
            p.ToggleRenderer(true);
        }
        foreach (SubMenu sm in FindObjectsOfType<SubMenu>())
        {
            sm.Init();
            sm.ToggleRenderer(true);
        }
    }

   
}