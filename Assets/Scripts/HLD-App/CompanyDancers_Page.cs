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

    Sprite ImageToUse = null;
    AssetBundle tmp = null;

    //  private string BiographiesJSON;
    //  BioArray myObject;
    //  private IOrderedEnumerable<Biography> OrderedByName;

    public void Init()
    {
        GetComponent<UIB_Page>().AssetBundleRequired = true;
        // UIB_AssetBundleHelper.InsertAssetBundle("hld/bios/photos");

        GetComponent<UIB_Page>().OnActivated += onPageActivated;
        GetComponent<UIB_Page>().OnDeActivated += onPageDeactivated;

    }

    private void onPageDeactivated()
    {
        transform.Find("ScrollMenuLoadCover").gameObject.SetActive(true);
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

        yield return new WaitForEndOfFrame();
        transform.Find("ScrollMenuLoadCover").gameObject.SetActive(false);
        yield break;
    }

    public override void Update()
    {
        base.Update();

        foreach (AssetBundle b in AssetBundle.GetAllLoadedAssetBundles())
        {
            if (b.name == "hld/bios/photos")
                tmp = b;
        }

        var outStr = "";
        if (GetCurrentlySelectedListElement() != null)
        {
            outStr = UIB_Utilities.SplitOnFinalUnderscore(GetCurrentlySelectedListElement().name);
            outStr = UIB_Utilities.SplitCamelCase(outStr);
            outStr = outStr.Replace(" ", "_");
        }

        try
        {
            ImageToUse = tmp.LoadAsset<Sprite>(outStr);
        }
        catch (Exception e)
        {
            if (e.GetBaseException().GetType() == typeof(NullReferenceException))
            {
            }
        }

        var BgPhoto = transform.Find("UIB_Background").Find("Background_Mask").Find("Background_Image")
            .GetComponent<Image>();

        BgPhoto.sprite = ImageToUse;

        if (BgPhoto != null)
        {
            BgPhoto.sprite = ImageToUse;

            //set recttransform aspect based on image and aspect ratio of screen
            var ar = UIB_AspectRatioManager.ScreenWidth / UIB_AspectRatioManager.ScreenHeight;
            var imgAR = 9f / 16f;

            if (!ar.Equals(imgAR))
            {
                try
                {
                    if (ImageToUse != null)
                        BgPhoto.rectTransform.sizeDelta = new Vector2(ImageToUse.rect.width, ImageToUse.rect.height * ar);
                }
                catch (Exception e)
                {
                    if (e.GetBaseException().GetType() == typeof(NullReferenceException))
                    {
                    }

                }
            }
        }
    }

    //The implementation of the page generator for this pages submenu
    public override void MakeLinkedPages()
    {
        if (OrderedByName == null)
        {
            Debug.LogWarning("Warning: There was no list to iterate through");
            return;
        }

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

        StartCoroutine(GetComponent<UIB_Page>().ResetUAP(true));
    }

    public override GameObject GetCurrentlySelectedListElement()
    {
        return CurrentlySelectedListElement;
    }
}
