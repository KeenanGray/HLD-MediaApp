using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static HLD.JSON_Structs;

namespace UI_Builder
{
    public interface UIB_IScrollMenu
    {
        void MakeLinkedPages();
    }

    [RequireComponent(typeof(ScrollRect))]
    public abstract class UIB_ScrollMenu : MonoBehaviour, UIB_IPage, UIB_IScrollMenu
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
            GetComponent<UIB_Page>().OnActivated += PageActivatedHandler;
            GetComponent<UIB_Page>().OnDeActivated += PageDeActivatedHandler;

            Page_Parent = null;

            if (GameObject.Find(Page_Parent_Name) != null)
                Page_Parent = GameObject.Find(Page_Parent_Name);

            scroll = GetComponentInChildren<ScrollRect>();

            SourceJson = FileManager.ReadTextFile(json_file);
            if (SourceJson == null || SourceJson == "")
            {
                //TODO:What else needs to be done prior to updating?
                Debug.Log("No JSON read from file");
                return;
            }
            myObject = JsonUtility.FromJson<BiographyArray>(SourceJson);
            OrderedByName = myObject.data.OrderBy(x => x.Name.Split(' ')[1]);
        }

        public void PageActivatedHandler()
        {
            ObjPoolManager.RefreshPool();
            //Make the pages first
            MakeLinkedPages();

            //Make the buttons
            //They will be assigned to their buttons with 'Init'
            int traversalOrder = 0;
            ObjPoolManager.BeginRetrieval();

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
    }
}