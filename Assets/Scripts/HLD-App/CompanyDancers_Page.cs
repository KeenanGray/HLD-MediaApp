using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI_Builder;
using static Bio_Factory;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class CompanyDancers_Page : MonoBehaviour, UIB_IPage {

    ScrollRect scroll;
    static string Bio_GameObject_Name = "Pages";
    GameObject Bio_Page_Root;
    private string BiographiesJSON;
    BioArray myObject;
    private IOrderedEnumerable<Biography> OrderedByName;

    // Use this for initialization
    public void Init () {
        GetComponent<UIB_Page>().OnActivated += PageActivatedHandler;
        GetComponent<UIB_Page>().OnDeActivated += PageDeActivatedHandler;

        Bio_Page_Root = null;

        if (GameObject.Find(Bio_GameObject_Name) != null)
            Bio_Page_Root = GameObject.Find(Bio_GameObject_Name);

        scroll = GetComponentInChildren<ScrollRect>();

        BiographiesJSON = MongoLib.ReadJson("Bios.json");
        if (BiographiesJSON == null || BiographiesJSON == "")
        {
            //TODO:What else needs to be done prior to updating?
            Debug.Log("No JSON read from file");
            return;
        }
        myObject = JsonUtility.FromJson<BioArray>(BiographiesJSON);
        OrderedByName = myObject.data.OrderBy(x => x.Name.Split(' ')[1]);
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void PageActivatedHandler()
    {


        //Make the pages first
        foreach (Biography bioJson in OrderedByName)
        {
            GameObject go = null; 
            ObjPoolManager.RetrieveFromPool(ObjPoolManager.Pool.Bio, ref go);

            if (go != null)
            {
                go.transform.SetParent(Bio_Page_Root.transform);
                go.name = (bioJson.Name + "_Page");

                Bio_Page bp = go.GetComponent<Bio_Page>();
                bp.SetName(bioJson.Name);
                bp.SetTitle(bioJson.Title);
                bp.SetImage("DancerPhotos/" + bioJson.Name.Replace(" ", "_"));
                bp.SetDesc(bioJson.Bio);           
            }
        }

        //Make the buttons
        //They will be assigned to their buttons with 'Init'
        int traversalOrder = 0;
        foreach (Biography b in OrderedByName)
        {
            GameObject go= null; 
            ObjPoolManager.RetrieveFromPool(ObjPoolManager.Pool.Button, ref go);
            if (go != null)
            {
                go.name = (b.Name + "_Button");

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
    }

    public void PageDeActivatedHandler()
    {
        ObjPoolManager.Init();
    }
}
