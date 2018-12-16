using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

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

    static string BioParentGameObjectName = "CompanyDancers_Page";
    static string BioButtonGameObjectName = "CompanyDancers_Button";

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}


    public static void CreateBioPages(string BiographiesJSON){

     

      

    }
}
