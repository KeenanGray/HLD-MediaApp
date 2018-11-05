using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Bio_Factory : MonoBehaviour {

    [System.Serializable]
    public class BioArray
    {
        public Bio[] users;
    }

    [System.Serializable]
    public class Bio{
        public string _id;
        public string Name;
        public string Title;
        public string Desc;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    [ExecuteInEditMode]
    public static void CreateBioPages(string BiographiesJSON){
        GameObject Bio_Page_Root = GameObject.Find("Biographies_Links");
                                            
        string JSONToParse = "{\"users\":" + BiographiesJSON + "}";

        int i = 0;
        //Initialize and position all the biographies in the pool
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("App_Biography")) {
            go.GetComponent<Bio_Page>().Initialize();
            Debug.Log(AspectRatioManager.ScreenHeight);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(-2000, i * AspectRatioManager.ScreenHeight);
            i++;
        }

        BioArray myObject = JsonUtility.FromJson<BioArray>(JSONToParse);

        //Grab objects from the pool and insert them as independent pages

        foreach(Bio b in myObject.users){

            GameObject go = ObjPoolManager.RetrieveFromPool(ObjPoolManager.Pool.Button);
            if (go != null)
            {
                go.transform.SetParent(Bio_Page_Root.transform);
                go.GetComponent<App_Button>().SetButtonText(b.Name);
            }
;            Debug.Log(b.Name);
        }
       
	}
}
