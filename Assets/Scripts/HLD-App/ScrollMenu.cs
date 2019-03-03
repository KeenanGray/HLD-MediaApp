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
    }

    [RequireComponent(typeof(ScrollRect))]
    public abstract class ScrollMenu : MonoBehaviour, UIB_IPage, IScrollMenu
    {
        protected ScrollRect scroll;
        protected string SourceJson;

        //The name of the root gameobject that all newly linked pages will be parented to. 
        public string Page_Parent_Name = "Pages";
        //The gameobject to parent to.
        protected GameObject Page_Parent;
        //The name of the json file to pull data from
        public string json_file;

        protected string Name_Suffix;

        protected BiographyArray myObject;
        protected IOrderedEnumerable<Biography> OrderedByName;

        void UIB_IPage.Init()
        {
            GetComponent<UIB_Page>().AssetBundleRequired = true;
            UIB_AssetBundleHelper.InsertAssetBundle("hld/bios/json");

            if (GetComponent<UIB_Page>().AssetBundleRequired)
            {
                //Debug.Log("do we have to do something here");
            }

            GetComponent<UIB_Page>().OnActivated += PageActivatedHandler;
            GetComponent<UIB_Page>().OnDeActivated += PageDeActivatedHandler;

            Page_Parent = null;

            if (GameObject.Find(Page_Parent_Name) != null)
                Page_Parent = GameObject.Find(Page_Parent_Name);

            scroll = GetComponentInChildren<ScrollRect>();


        }
        public void InitJsonList()
        {
            SourceJson = UIB_FileManager.ReadTextAssetBundle(json_file, "hld/bios/json");
            if (SourceJson == null || SourceJson == "")
            {
                return;
            }
            myObject = JsonUtility.FromJson<BiographyArray>(SourceJson);
            OrderedByName = myObject.data.OrderBy(x => x.Name.Split(' ')[1]);

            // foreach(Biography sr in OrderedByName)
            //     Debug.Log(sr.Name);
        }

        public void PageActivatedHandler()
        {
            InitJsonList();

            ObjPoolManager.RefreshPool();
            //Make the pages first
            MakeLinkedPages();

            //Make the buttons
            //They will be assigned to their buttons with 'Init'
            int traversalOrder = 0;
            ObjPoolManager.BeginRetrieval();

            if (OrderedByName == null)
            {
                Debug.LogWarning("Warning: There was no list to iterate through on page activated");
                return;
            }

            foreach (Biography b in OrderedByName)
            {
                Name_Suffix = b.Name.Replace(" ", "");
                GameObject go = null;
                ObjPoolManager.RetrieveFromPool(ObjPoolManager.Pool.Button, ref go);
                if (go != null)
                {
                    go.name = (Name_Suffix + "_Button");

                    UI_Builder.UIB_Button UIB_btn = go.GetComponent<UI_Builder.UIB_Button>();
                    go.transform.SetParent(scroll.content.transform);

                    //update parent for accessibility
                    var sab = go.GetComponent<Special_AccessibleButton>();

                    sab.m_ManualPositionParent = go.GetComponentInParent<AccessibleUIGroupRoot>().gameObject;
                    sab.m_ManualPositionOrder = traversalOrder;
                    traversalOrder++;

                    UIB_btn.SetButtonText(b.Name);
                    UIB_btn.Button_Opens = UI_Builder.UIB_Button.UIB_Button_Activates.Page;
                    //                    Debug.Log("DancerPhotos/"+ b.Name.Replace(" ", "_"));

                    foreach (Image image in transform.GetComponentsInParent<Image>())
                    {
                        //                        Debug.Log("name " + image.gameObject.name);
                        //     UIB_Button.backgroundImage = GameObject.Find("J");

                    }
                    //custom backgrounds
                    UIB_btn.Special_Background = Resources.Load("DancerPhotos/" + b.Name.Replace(" ", "_")) as Sprite;

                    go.GetComponent<Button>().enabled = true;
                    go.GetComponent<UAP_BaseElement>().enabled = true;

                    //For some reason you have to do this
                    //So that the names appear in the right order for accessibility
                    gameObject.SetActive(false);
                    gameObject.SetActive(true);

                    UIB_btn.Init();
                }
            }
            ObjPoolManager.EndRetrieval();

            scroll.GetComponent<UIB_ScrollingMenu>().playedOnce = false;
            scroll.GetComponent<UIB_ScrollingMenu>().Playing = false;
            scroll.GetComponent<UIB_ScrollingMenu>().Setup();

        }

        public void PageDeActivatedHandler()
        {
            ObjPoolManager.RefreshPool();
        }

        public abstract void MakeLinkedPages();
    }

}