﻿using System;
using System.Collections;
using System.Collections.Generic;
using HLD;
using UI_Builder;
using UnityEngine;
using UnityEngine.UI;
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

    private void Start()
    {
        GetComponent<UIB_Page>().OnActivated += onPageActivated;
    }

    private void onPageActivated()
    {
        StartCoroutine("updateWait");
    }

    IEnumerator updateWait()
    {
        var scrollrect = GetComponentInChildren<ScrollRect>();
        scrollrect.content.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -1700);

        /*
        while (scrollrect.content.GetComponent<RectTransform>().rect.height <= 0)
        {
            Debug.Log("HERE 1");
            yield return null;
        }
        */
        scrollrect.content.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -scrollrect.GetComponent<RectTransform>().rect.height);
        yield break;
    }
}
