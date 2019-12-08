using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UI_Builder;
using static UI_Builder.UIB_Page;

public class LoadProgramNotes : MonoBehaviour
{
    private void Start()
    {
        GetComponentInParent<UIB_Page>().OnActivated += new Activated(PageActivatedHandler);
        GetComponentInParent<UIB_Page>().OnDeActivated += new DeActivated(PageDeActivatedHandler);
    }
    private void PageActivatedHandler()
    {
        LoadNotes(gameObject.name.Split('-')[0]);
    }


    private void PageDeActivatedHandler()
    {
    }



    void LoadNotes(string ShowName)
    {
        var tmp = GetComponent<TextMeshProUGUI>();
        var name = ShowName + "ProgramNotes";

        var text = UIB_FileManager.ReadTextAssetBundle(ShowName + "ProgramNotes", "hld/general");

        tmp.text = text;

        if (tmp.text.Length <= 0)
        {
            tmp.enabled = false;
        }
        else
        {
            tmp.enabled = true;
        }

    }

}
