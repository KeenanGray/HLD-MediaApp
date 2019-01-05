using System.Collections;
using System.Collections.Generic;
using UI_Builder;
using UnityEngine;
using static HLD_JSON_Structs;

public class DisplayedNarratives_Page : UIB_ScrollMenu {


    //The implementation of the page generator for this pages submenu
    public override void MakeLinkedPages()
    {
        foreach (Biography bioJson in OrderedByName)
        {
            GameObject go = null;
            ObjPoolManager.RetrieveFromPool(ObjPoolManager.Pool.Bio, ref go);

            if (go != null)
            {
                go.transform.SetParent(Page_Parent.transform);
                go.name = (bioJson.Name + "_Page");

                Bio_Page bp = go.GetComponent<Bio_Page>();
                bp.SetName(bioJson.Name);
                bp.SetTitle(bioJson.Title);
                bp.SetImage("DancerPhotos/" + bioJson.Name.Replace(" ", "_"));
                bp.SetDesc(bioJson.Bio);
            }
        }
    }
}
