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
            p.gameObject.SetActive(true);
            p.Init();
            p.SetOnScreen(true);
        }
        foreach (SubMenu sm in FindObjectsOfType<SubMenu>())
        {
            sm.gameObject.SetActive(true);
            sm.Init();
            sm.SetOnScreen(true);
        }
        foreach (GameObject asm in GameObject.FindGameObjectsWithTag("App_SubMenuButton"))
        {
            asm.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        }
    }

   
}