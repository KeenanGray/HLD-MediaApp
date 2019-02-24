using System.Collections;
using System.Collections.Generic;
using HLD;
using UI_Builder;
using UnityEngine;
using static HLD.JSON_Structs;

public class DisplayedNarrativesList_Page : HLD.ScrollMenu
{

    //The implementation of the page generator for this pages submenu
    public override void MakeLinkedPages()
    {
        ObjPoolManager.BeginRetrieval();
        var BiographyOrderedByName = (System.Linq.IOrderedEnumerable<Biography>)OrderedByName;

        foreach (Biography bioJson in BiographyOrderedByName)
        {
            Name_Suffix = bioJson.Name.Replace(" ", "");
            GameObject go = null;
            ObjPoolManager.RetrieveFromPool(ObjPoolManager.Pool.Narrative, ref go);

            if (go != null)
            {
                go.transform.SetParent(Page_Parent.transform);
                go.name = (Name_Suffix + "_Page");
                Narrative_Page np = go.GetComponent<Narrative_Page>();
                np.SetupPage(bioJson.Name, bioJson.Name.Replace(" ", "_").ToLower());
            }
        }

        ObjPoolManager.EndRetrieval();

    }
}
