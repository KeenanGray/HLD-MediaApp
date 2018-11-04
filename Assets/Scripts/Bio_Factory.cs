using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using MongoDB.Bson;

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
        string JSONToParse = "{\"users\":" + BiographiesJSON + "}";

        BioArray myObject = JsonUtility.FromJson<BioArray>(JSONToParse);

        foreach(Bio b in myObject.users){
            Debug.Log(b.Name);
        }
       
	}
}
