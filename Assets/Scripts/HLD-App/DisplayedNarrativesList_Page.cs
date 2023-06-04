using System;
using System.Collections;
using System.Collections.Generic;
using HLD;
using UI_Builder;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using static HLD.JSON_Structs;

public class DisplayedNarrativesList_Page : HLD.ScrollMenu
{
    //The implementation of the page generator for this pages submenu
    public override void MakeLinkedPages()
    {
        //        Debug.Log(gameObject.name + " " +"makelinkedpages");
        var ShowName = name.Split('-')[0];
        if (ShowName == "CompanyDancers_Page")
        {
            var BiographyOrderedByName =
                (System.Linq.IOrderedEnumerable<Biography>)OrderedByName;
            foreach (Biography bioJson in BiographyOrderedByName)
            {
                Name_Suffix = bioJson.Name.Replace(" ", "").Trim();

                GameObject go = null;
                ObjPoolManager
                    .RetrieveFromPool(ObjPoolManager.Pool.Narrative, ref go);

                if (go != null)
                {
                    go.transform.SetParent(Page_Parent.transform);
                    go.name = (Name_Suffix + "_Page");
                    Narrative_Page np = go.GetComponent<Narrative_Page>();
                    np
                        .SetupPage(bioJson.Name,
                        bioJson.Name.Replace(" ", "_").ToLower());
                    np.SetShowName(name.Split('-')[0]);
                }
            }
        }
        else
        {
            //reload the scene instead of crashing
            if (listOfDancers == null)
            {
                SceneManager.LoadScene(1);
                return;
            }
            foreach (string s in listOfDancers)
            {
                Name_Suffix = s.Replace("_", "");

                GameObject go = null;
                ObjPoolManager
                    .RetrieveFromPool(ObjPoolManager.Pool.Narrative, ref go);

                if (go != null)
                {
                    go.transform.SetParent(Page_Parent.transform);
                    go.name = (Name_Suffix + "_Page");
                    Narrative_Page np = go.GetComponent<Narrative_Page>();

                    np.SetupPage(s, s.Replace("_", "_").ToLower());
                    np.SetShowName(name.Split('-')[0]);
                }
            }
        }
    }

    private void Start()
    {
        GetComponent<UIB_Page>().OnActivated += onPageActivated;
        GetComponent<UIB_Page>().OnDeActivated += onPageDeActivated;
    }

    private void onPageActivated()
    {
        StartCoroutine("updateWait");
        base.PageActivatedHandler();
    }

    private void onPageDeActivated()
    {
        //        Debug.Log("deactivated list");
        base.PageDeActivatedHandler();
    }

    IEnumerator updateWait()
    {
        ScrollRect scrollrect = GetComponentInChildren<ScrollRect>();
        scrollrect.content.GetComponent<RectTransform>().anchoredPosition =
            new Vector2(0, -1700);

        /*
        while (scrollrect.content.GetComponent<RectTransform>().rect.height <= 0)
        {
            Debug.Log("HERE 1");
            yield return null;
        }
        */
        scrollrect.content.GetComponent<RectTransform>().anchoredPosition =
            new Vector2(0,
                -scrollrect.GetComponent<RectTransform>().rect.height);
        yield break;
    }

    public override GameObject GetCurrentlySelectedListElement()
    {
        return CurrentlySelectedListElement;
    }
}
