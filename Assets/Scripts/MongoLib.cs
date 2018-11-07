using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MongoLib : MonoBehaviour {

    //db name heroku_pm1crn83
    //TODO: IS IT OKAY TO USE API KEY HERE? READ ONLY USAGE?!!?!??!
    // Use this for initialization
    public TextAsset config;

    private string baseURL = "https://api.mlab.com/api/1/";
    private string API_Key = "";
    private string db_name = "heroku_pm1crn83";

    private string collection_name;
    private string db_result;
    
    private string GetAPIKey(string v)
    {
        foreach(string s in v.Split('\n')){
            if(s.Contains("api_key")){
                return s.Split('=')[1];
            }
        }
        return v;
    }

    public IEnumerator UpdateFromDatabase()
    {
        API_Key = GetAPIKey(config.text);
        yield return StartCoroutine("UpdateBiographies");
        
    }

    string GenerateCollectionRequestString(string collection){
        var myString = "";
        myString += baseURL;
        myString += ("databases/" + db_name + "/collections/" +collection + "?apiKey="+API_Key);

        return myString;
    }

    IEnumerator UpdateBiographies(){
        //Get the biographies from the database
        collection_name = "The_Displayed";
        yield return StartCoroutine("GetCollectionFromDatabase");
        Bio_Factory.CreateBioPages(db_result);
        yield break;
    }

    IEnumerator GetCollectionFromDatabase(){
        var url = GenerateCollectionRequestString(collection_name);
        using (WWW www = new WWW(url))
        {
            while (!www.isDone)
            {
                yield return www;
            }
            if(www.responseHeaders.ContainsKey("STATUS")){
                if(www.responseHeaders["STATUS"] == "HTTP/1.1 200 OK")
                {
                //   Debug.Log(www.text);
                    db_result = www.text;
                }
                else
                {
                    Debug.LogWarning("Error with response" + www.responseHeaders["STATUS"]);
                }
            }
            yield break;
        }

    }
}
