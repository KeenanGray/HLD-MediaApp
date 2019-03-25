using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


//TODO: THIS SHOULDN"T BE USED RIGHT NOW HAVE TO FIGURE OUT HOW TO CONVER THIS FOR UI_BUILDER PLUGIN
namespace UI_Builder
{
    public interface UIB_IScrollMenu
    {
        void MakeLinkedPages();
    }

    [RequireComponent(typeof(ScrollRect))]
    public abstract class UIB_ScrollMenu : MonoBehaviour, UIB_IPage, UIB_IScrollMenu
    {
        /*
        protected ScrollRect scroll;
        protected string SourceJson;

        //The name of the root gameobject that all newly linked pages will be parented to. 
        public string Page_Parent_Name = "Pages";
        //The gameobject to parent to.
        protected GameObject Page_Parent;
        //The name of the json file to pull data from
        public string json_file;

        protected string Name_Suffix;

        protected UIB_Struct myObject;
        protected IOrderedEnumerable<UIB_Struct> OrderedByName;

        void UIB_IPage.Init()
        {
            GetComponent<UIB_Page>().AssetBundleRequired = true;
            UIB_AssetBundleHelper.InsertAssetBundle("hld/bios/json");

            if (GetComponent<UIB_Page>().AssetBundleRequired)
            {
                Debug.Log("do we have to do something here");
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
                Name_Suffix = b.Name.Replace(" ","");
                GameObject go = null;
                ObjPoolManager.RetrieveFromPool(ObjPoolManager.Pool.Button, ref go);
                if (go != null)
                {
                    go.name = (Name_Suffix + "_Button");

                    UI_Builder.UIB_Button UIB_btn = go.GetComponent<UI_Builder.UIB_Button>();
                    go.transform.SetParent(scroll.content.transform);

                    //update parent for accessibility
                    var sab = go.GetComponent<Special_AccessibleButton>();

                    //TODO accessibility select the first element in the list
                    //get the button for the biographies and tell it to select
                    //the first element in the list
                    if (traversalOrder == 0)
                    {
                    }

                    sab.m_ManualPositionParent = go.GetComponentInParent<AccessibleUIGroupRoot>().gameObject;
                    sab.m_ManualPositionOrder = traversalOrder;
                    traversalOrder++;

                    UIB_btn.SetButtonText(b.Name);
                    UIB_btn.Button_Opens = UI_Builder.UIB_Button.UIB_Button_Activates.Page;
//                    Debug.Log("DancerPhotos/"+ b.Name.Replace(" ", "_"));

                    foreach(Image image in transform.GetComponentsInParent<Image>()) {
//                        Debug.Log("name " + image.gameObject.name);
                   //     UIB_Button.backgroundImage = GameObject.Find("J");

                    }
                    //custom backgrounds
                    UIB_btn.Special_Background = Resources.Load("DancerPhotos/" + b.Name.Replace(" ", "_"))as Sprite;

                    //For some reason you have to do this
                    //So that the names appear in the right order for accessibility
                    gameObject.SetActive(false);
                    gameObject.SetActive(true);

                    //set the text in the button to right align.
                    go.GetComponentInChildren<TextMeshProUGUI>().alignment = TMPro.TextAlignmentOptions.Right;

                    UIB_btn.Init();
                    GetComponent<UIB_Page>().ActivateButtonsOnScreen();
                }
            }
            ObjPoolManager.EndRetrieval();
        }

        public void PageDeActivatedHandler()
        {
            ObjPoolManager.RefreshPool();
        }

        public abstract void MakeLinkedPages();
        */
        public void Init()
        {
            throw new System.NotImplementedException();
        }

        public void MakeLinkedPages()
        {
            throw new System.NotImplementedException();
        }

        public void PageActivatedHandler()
        {
            throw new System.NotImplementedException();
        }

        public void PageDeActivatedHandler()
        {
            throw new System.NotImplementedException();
        }
    }

}