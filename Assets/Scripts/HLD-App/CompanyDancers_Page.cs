using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI_Builder;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using HLD;
using static HLD.JSON_Structs;
using System;

public class CompanyDancers_Page : HLD.ScrollMenu
{

    //  ScrollRect scroll;
    GameObject Bio_Page_Root;

    //  private string BiographiesJSON;
    //  BioArray myObject;
    //  private IOrderedEnumerable<Biography> OrderedByName;

    public void Init()
    {
        GetComponent<UIB_Page>().AssetBundleRequired = true;
        UIB_AssetBundleHelper.InsertAssetBundle("hld/bios/photos");

        GetComponent<UIB_Page>().OnActivated += onPageActivated;
    }

    private void onPageActivated()
    {
        StartCoroutine("updateWait");
    }

    IEnumerator updateWait()
    {
        var scrollrect = GetComponentInChildren<ScrollRect>();

        while (scrollrect.content.GetComponent<RectTransform>().rect.height <= 0)
        {
            yield return null;
        }

        scrollrect.content.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -scrollrect.GetComponent<RectTransform>().rect.height);
        yield break;
    }

    //The implementation of the page generator for this pages submenu
    public override void MakeLinkedPages()
    {
        if (OrderedByName == null)
        {
            Debug.LogWarning("Warning: There was no list to iterate through");
            return;
        }

        ObjPoolManager.BeginRetrieval();
        var BiographyOrderedByName = (System.Linq.IOrderedEnumerable<Biography>)OrderedByName;

        foreach (Biography bioJson in BiographyOrderedByName)
        {
            Name_Suffix = bioJson.Name.Replace(" ", "");
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
                    bp.SetImageFromAssetBundle(bioJson.Name.Replace(" ", "_").ToLower(), "hld/bios/photos");
                    bp.SetDesc(bioJson.Bio);
                }
            }
        }

        ObjPoolManager.EndRetrieval();
        StartCoroutine(GetComponent<UIB_Page>().ResetUAP(true));



    }
}
