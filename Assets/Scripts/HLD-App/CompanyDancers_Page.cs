using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI_Builder;
using static HLD.JSON_Structs;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class CompanyDancers_Page : UIB_ScrollMenu {

  //  ScrollRect scroll;
    GameObject Bio_Page_Root;

    //  private string BiographiesJSON;
    //  BioArray myObject;
    //  private IOrderedEnumerable<Biography> OrderedByName;

    //The implementation of the page generator for this pages submenu
    public override void MakeLinkedPages()
    {
        if (OrderedByName == null)
            return;

        ObjPoolManager.BeginRetrieval();
        foreach (Biography bioJson in OrderedByName)
        {
            Name_Suffix = bioJson.Name.Replace(" ","");
            GameObject go = null;
            ObjPoolManager.RetrieveFromPool(ObjPoolManager.Pool.Bio, ref go);

            if (go != null)
            {
                go.transform.SetParent(Page_Parent.transform);
                go.name = (Name_Suffix + "_Page");

                Bio_Page bp = go.GetComponent<Bio_Page>();
                if (bp.Name.text != bioJson.Name)
                {
                    bp.SetName(bioJson.Name);
                    bp.SetTitle(bioJson.Title);
                    bp.SetImage(Application.persistentDataPath + "/hld-general/Bio_Photos/" + bioJson.Name.Replace(" ", "_"));
                    bp.SetDesc(bioJson.Bio);
                }
            }
        }
        ObjPoolManager.EndRetrieval();
    }
}
