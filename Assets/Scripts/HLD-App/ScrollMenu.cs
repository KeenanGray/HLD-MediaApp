using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UI_Builder;
using UnityEngine;
using UnityEngine.UI;
using static HLD.JSON_Structs;

namespace HLD
{
    public interface IScrollMenu
    {
        void MakeLinkedPages();
        GameObject GetCurrentlySelectedListElement();
    }

    public abstract class ScrollMenu : MonoBehaviour, UIB_IPage, IScrollMenu
    {
        protected ScrollRect scroll;
        protected string SourceJson;

        protected GameObject CurrentlySelectedListElement = null;

        GameObject center;

        string ShowName;

        //The name of the root gameobject that all newly linked pages will be parented to. 
        public string Page_Parent_Name = "Pages";
        //The gameobject to parent to.
        protected GameObject Page_Parent;
        //The name of the json file to pull data from
        public string json_file;

        protected string Name_Suffix;

        protected BiographyArray myObject;
        protected IOrderedEnumerable<Biography> OrderedByName;
        protected string[] listOfDancers;
        private bool pageActivatedBefore;

        GameObject closest; //the gameobject closest to the middle of the scrollrect on screen

        private void Start()
        {
            GetComponent<UIB_Page>().OnActivated += PageActivatedHandler;
            GetComponent<UIB_Page>().OnDeActivated += PageDeActivatedHandler;
        }

        void UIB_IPage.Init()
        {
            GetComponent<UIB_Page>().AssetBundleRequired = true;
            //          UIB_AssetBundleHelper.InsertAssetBundle("hld/bios/json");

            if (GetComponent<UIB_Page>().AssetBundleRequired)
            {
                //Debug.Log("do we have to do something here");
            }

            Page_Parent = null;

            if (GameObject.Find(Page_Parent_Name) != null)
                Page_Parent = GameObject.Find(Page_Parent_Name);

            scroll = GetComponentInChildren<ScrollRect>();

        }
        public void InitJsonList()
        {
            ShowName = name.Split('-')[0];
            if (ShowName == "CompanyDancers_Page")
            {
                SourceJson = UIB_FileManager.ReadTextAssetBundle("bios", "hld/bios/json");
                if (SourceJson == null || SourceJson == "")
                {
                    return;
                }
                myObject = JsonUtility.FromJson<BiographyArray>(SourceJson);
                OrderedByName = myObject.data.OrderBy(x => x.Name.Split(' ')[1]);
            }
            else
            {
                SourceJson = UIB_FileManager.ReadTextAssetBundle(ShowName + "ListOfDancers", "hld/general");
                if (SourceJson == null || SourceJson == "")
                {
                    return;
                }
                listOfDancers = SourceJson.Replace("\n", "").Split(',');

            }
        }


        public void Update()
        {
            if (InitializationManager.InitializeTime == 0)
                return;

            var scrollTransform = scroll.content.transform;

            if (scrollTransform.childCount == 0)
                return;

            center = scroll.transform.Find("Center").gameObject;
            var contentMiddle = center.transform.position;

            // Debug.DrawLine(scroll.viewport.position, contentMiddle, Color.green);

            for (int i = 0; i < scrollTransform.childCount; i++)
            {
                if (scrollTransform.GetChild(i).tag != "App_SubMenuButton")
                    continue;

                if (closest == null)
                    closest = scrollTransform.GetChild(i).gameObject;


                if (scrollTransform.GetChild(i).gameObject != closest)
                    scrollTransform.GetChild(i).gameObject.GetComponent<UIB_Button>().ResetButtonColors();

                if (Vector3.Distance(scrollTransform.GetChild(i).position, contentMiddle) < Vector3.Distance(closest.transform.position, contentMiddle))
                {
                    closest = scrollTransform.GetChild(i).gameObject;
                }
            }

            //if UAP is enabled, we use a different button to focus
            if (UAP_AccessibilityManager.IsEnabled())
            {
                if (UAP_AccessibilityManager.GetCurrentFocusObject() != null)
                {
                    if (UAP_AccessibilityManager.GetCurrentFocusObject().tag.Equals("App_SubMenuButton"))
                        closest = UAP_AccessibilityManager.GetCurrentFocusObject();
                }
            }


            CurrentlySelectedListElement = closest;

            CurrentlySelectedListElement.GetComponent<UIB_Button>().SetupButtonColors();

            //Update the background based on the scroll box
            Sprite ImageToUse = null;
            AssetBundle tmp = null;
            foreach (AssetBundle b in AssetBundle.GetAllLoadedAssetBundles())
            {
                if (b.name == "hld/" + ShowName.ToLower() + "/narratives/photos")
                    tmp = b;
            }

            var outStr = "";
            if (GetCurrentlySelectedListElement() != null)
            {
                outStr = UIB_Utilities.SplitOnFinalUnderscore(GetCurrentlySelectedListElement().name);
                outStr = UIB_Utilities.SplitCamelCase(outStr);
                outStr = outStr.Replace(" ", "_");
                outStr = UIB_Utilities.CleanUpHyphenated(outStr);
                outStr = UIB_Utilities.RemoveAllButLastUnderscore(outStr);
            }

            try
            {
                Debug.Log("out string: " + outStr);
                ImageToUse = tmp.LoadAsset<Sprite>(outStr);
                Debug.Log("image " + ImageToUse.name);
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

        public void PageActivatedHandler()
        {
            // UAP_AccessibilityManager.PauseAccessibility(true);
            //Debug.Break();

            scroll.content.GetComponent<RectTransform>().pivot = new Vector2(0, 1);

            if (pageActivatedBefore)
                return;

            pageActivatedBefore = true;
            InitJsonList();

            ObjPoolManager.RefreshPool();

            //   if (!pageActivatedBefore)
            //  {
            //Make the pages first
            MakeLinkedPages();
            //    }

            //Make the buttons
            //They will be assigned to their buttons with 'Init'
            int traversalOrder = 0;

            ShowName = name.Split('-')[0];
            if (ShowName == "CompanyDancers_Page")
            {
                if (OrderedByName == null)
                {
                    Debug.LogWarning("Warning: There was no list to iterate through on page activated");
                    return;
                }

                foreach (Biography b in OrderedByName)
                {
                    Name_Suffix = b.Name.Replace(" ", "");
                    Debug.Log("Name suffix : " + Name_Suffix);

                    GameObject go = null;
                    ObjPoolManager.RetrieveFromPool(ObjPoolManager.Pool.Button, ref go);
                    if (go != null)
                    {
                        go.name = (Name_Suffix + "_Button");

                        UI_Builder.UIB_Button UIB_btn = go.GetComponent<UI_Builder.UIB_Button>();
                        go.transform.SetParent(scroll.content.transform);

                        //update parent for accessibility
                        var sab = go.GetComponent<UAP_BaseElement>();

                        sab.m_ManualPositionParent = go.GetComponentInParent<AccessibleUIGroupRoot>().gameObject;
                        sab.m_ManualPositionOrder = traversalOrder;
                        traversalOrder++;

                        UIB_btn.SetButtonText(UIB_Utilities.SplitCamelCase(b.Name));
                        UIB_btn.Button_Opens = UI_Builder.UIB_Button.UIB_Button_Activates.Page;

                        //custom backgrounds
                        UIB_btn.Special_Background = Resources.Load("DancerPhotos/" + b.Name.Replace(" ", "_")) as Sprite;

                        go.GetComponent<Button>().enabled = true;
                        go.GetComponent<UAP_BaseElement>().enabled = true;

                        UIB_btn.Init();
                    }
                }
            }
            else
            {
                foreach (string s in listOfDancers)
                {
                    Name_Suffix = s.Replace("_", "");
                    GameObject go = null;
                    ObjPoolManager.RetrieveFromPool(ObjPoolManager.Pool.Button, ref go);
                    if (go != null)
                    {
                        go.name = (Name_Suffix + "_Button");

                        UI_Builder.UIB_Button UIB_btn = go.GetComponent<UI_Builder.UIB_Button>();
                        go.transform.SetParent(scroll.content.transform);

                        //update parent for accessibility
                        var sab = go.GetComponent<UAP_BaseElement>();

                        sab.m_ManualPositionParent = go.GetComponentInParent<AccessibleUIGroupRoot>().gameObject;
                        sab.m_ManualPositionOrder = traversalOrder;
                        traversalOrder++;

                        UIB_btn.SetButtonText(UIB_Utilities.SplitCamelCase(s.Replace("_", " ")));
                        UIB_btn.Button_Opens = UI_Builder.UIB_Button.UIB_Button_Activates.Page;

                        foreach (Image image in transform.GetComponentsInParent<Image>())
                        {

                        }
                        //custom backgrounds
                        UIB_btn.Special_Background = Resources.Load("DancerPhotos/" + s.Replace("_", "_")) as Sprite;

                        go.GetComponent<Button>().enabled = true;
                        go.GetComponent<UAP_BaseElement>().enabled = true;

                        //For some reason you have to do this
                        //So that the names appear in the right order for accessibility
                        gameObject.SetActive(false);
                        gameObject.SetActive(true);

                        UIB_btn.Init();
                    }
                }
            }

            scroll.GetComponent<UIB_ScrollingMenu>().playedOnce = false;
            scroll.GetComponent<UIB_ScrollingMenu>().Playing = false;
            scroll.GetComponent<UIB_ScrollingMenu>().Setup();

            //create the top and bottom buffer for the scrollrect so that center selection can be highlighted
            var tmp = Resources.Load("UI_Buffer") as GameObject;
            var topBuffer = Instantiate(tmp, scroll.content.transform) as GameObject;
            var botBuffer = Instantiate(tmp, scroll.content.transform) as GameObject;

            topBuffer.GetComponent<RectTransform>().sizeDelta = new Vector2(scroll.viewport.rect.width, scroll.viewport.rect.height / 2);
            botBuffer.GetComponent<RectTransform>().sizeDelta = new Vector2(0, scroll.viewport.rect.height / 2);

            topBuffer.transform.SetAsFirstSibling();
            botBuffer.transform.SetAsLastSibling();

            pageActivatedBefore = true;

            GetComponentInParent<UIB_Page>().StartCoroutine(GetComponentInParent<UIB_Page>().ResetUAP(true));
            // UAP_AccessibilityManager.PauseAccessibility(false);
            UAP_AccessibilityManager.Say(" ");

            StartCoroutine("DisableCover");

        }

        public void PageDeActivatedHandler()
        {
            ObjPoolManager.RefreshPool();
            pageActivatedBefore = false;

            //delete the buffer game objects
            for (int i = 0; i < transform.childCount; i++)
            {
                try
                {
                    if (scroll.content.GetChild(i).name.Contains("UI_Buffer"))
                        Destroy(scroll.content.GetChild(i).gameObject);
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(NullReferenceException))
                    {

                    }
                }
            }

            transform.Find("ScrollMenuLoadCover").gameObject.SetActive(true);

        }

        public abstract void MakeLinkedPages();

        public abstract GameObject GetCurrentlySelectedListElement();

        IEnumerator DisableCover()
        {
            yield return new WaitForEndOfFrame();
            transform.Find("ScrollMenuLoadCover").gameObject.SetActive(false);
            yield break;
        }
    }
}
