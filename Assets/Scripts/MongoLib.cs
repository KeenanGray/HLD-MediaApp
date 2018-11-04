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

    private void  Start () {
        API_Key = GetAPIKey(config.text);
        StartCoroutine("GetCollectionFromDatabase");
    }

    private string GetAPIKey(string v)
    {
        foreach(string s in v.Split('\n')){
            if(s.Contains("api_key")){
                return s.Split('=')[1];
            }
        }
        return v;
    }

    // Update is called once per frame
    void Update () {
    }

    string GenerateCollectionRequestString(string collection){
        var myString = "";
        myString += baseURL;
        myString += ("databases/" + db_name + "/collections/" +collection + "?apiKey="+API_Key);
       // Debug.Log(myString);
        return myString;
    }

    IEnumerator GetCollectionFromDatabase(){
        var url = GenerateCollectionRequestString("The_Displayed");

        var one = "https://api.mlab.com/api/1/databases/heroku_pm1crn83/collections/The_Displayed?apiKey=1XimX81CHyHjHFoHBM4SfoASyTLfSd8R";
        var two = url;

        Debug.Log(one);
        Debug.Log(url);

        using (WWW www = new WWW(url))
        {
         
            while (!www.isDone)
            {
                yield return www;
            }
            if(www.responseHeaders.ContainsKey("STATUS")){
                if(www.responseHeaders["STATUS"] == "HTTP/1.1 200 OK")
                {
                    Debug.Log(www.text);
                }
                else{
                    Debug.LogWarning("Error with response" + www.responseHeaders["STATUS"]);
                }
            }    
            yield break;
        }

    }
}
