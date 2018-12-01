using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Bio_Factory : MonoBehaviour {

    [System.Serializable]
    public class BioArray
    {
        public Biography[] data;
    }

    [System.Serializable]
    public class Biography{
        public string _id;
        public string Name;
        public string Title;
        public string Bio;
    }

    public static bool Constructed;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

    static string Bio_GameObject_Name = "Pages";
    static string Bio_MenuGameObject_Name = "TheDisplayed_SubMenu";

    public static void CreateBioPages(string BiographiesJSON){
        GameObject Bio_Button_Root=null;
        if (GameObject.Find(Bio_MenuGameObject_Name) !=null)
            Bio_Button_Root = GameObject.Find(Bio_MenuGameObject_Name).GetComponentInChildren<ScrollRect>().content.gameObject;

        GameObject Bio_Page_Root=null;
        if (GameObject.Find(Bio_GameObject_Name) !=null)
            Bio_Page_Root = GameObject.Find(Bio_GameObject_Name);

        if(Bio_Page_Root == null || Bio_Button_Root==null){
            Debug.LogError("Required gameobjects were not found.");
            return;
        }

        if (BiographiesJSON==null || BiographiesJSON=="")
        {
            //TODO:What else needs to be done prior to updating?
            Debug.Log("No JSON read from file");
            return;
        }
       
        int i = 0;
        //Initialize and position all the biographies in the pool
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("App_Biography"))
        {
            go.GetComponent<Bio_Page>().Initialize();
            //go.GetComponent<RectTransform>().anchoredPosition = new Vector2(-2000, i * AspectRatioManager.ScreenHeight);
            i++;
        }
        BioArray myObject = JsonUtility.FromJson<BioArray>(BiographiesJSON);

        //Grab objects from the pool and insert them as independent pages
         var OrderedByName = myObject.data.OrderBy(x => x.Name);
        
        //Make the pages first
        foreach (Biography bioJson in OrderedByName)
        {
            GameObject go = ObjPoolManager.RetrieveFromPool(ObjPoolManager.Pool.Bio);
            if (go != null)
            {
                go.transform.SetParent(Bio_Page_Root.transform);
                //   go.GetComponent<Bio_Page>().SetButtonText(b.Name);
                //    go.GetComponent<Bio_Page>().Button_Opens = App_Button.Button_Activates.Page;
                go.name = (bioJson.Name + "_Page");
                go.tag = "Untagged";

                Bio_Page bp = go.GetComponent<Bio_Page>();
                bp.SetName(bioJson.Name);
                bp.SetTitle(bioJson.Title);
                bp.SetDesc(bioJson.Bio);

            }
        }

        //Make the buttons
        //They will be assigned to their buttons with 'Init'
        foreach (Biography b in OrderedByName)
        {
            GameObject go = ObjPoolManager.RetrieveFromPool(ObjPoolManager.Pool.Button);
            if (go != null)
            {
                go.name = (b.Name + "_Button");

                App_Button script = go.GetComponent<App_Button>();
                go.transform.SetParent(Bio_Button_Root.transform);

                //update parent for accessibility
                go.GetComponent<Special_AccessibleButton>().m_ManualPositionParent = go.GetComponentInParent<AccessibleUIGroupRoot>().gameObject;
                
                script.SetButtonText(b.Name);
                script.Button_Opens = App_Button.Button_Activates.Page;
                
                go.tag = "Untagged";

                script.Init();
            }
        }

      

    }
}
