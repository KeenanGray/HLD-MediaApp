using System.Collections;
using System.Collections.Generic;
using UI_Builder;
using UnityEngine;
using static HLD_JSON_Structs;

public class DisplayedNarrativesList_Page : UIB_ScrollMenu {

    //The implementation of the page generator for this pages submenu
    public override void MakeLinkedPages()
    {
        foreach (Biography bioJson in OrderedByName)
        {
            Name_Suffix = bioJson.Name.Replace(" ", "");
            GameObject go = null;
            ObjPoolManager.RetrieveFromPool(ObjPoolManager.Pool.Narrative, ref go);

          
            if (go != null)
            {
                go.transform.SetParent(Page_Parent.transform);
                go.name = (Name_Suffix +"_Page");

                /*
                Bio_Page bp = go.GetComponent<Bio_Page>();
                bp.SetName(bioJson.Name);
                bp.SetTitle(bioJson.Title);
                bp.SetImage("DancerPhotos/" + bioJson.Name.Replace(" ", "_"));
                bp.SetDesc(bioJson.Bio);
                */               
            }
        }
    }
}
