using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class MongoLib : MonoBehaviour {

    //db name heroku_pm1crn83
    // Use this for initialization
    public TextAsset config;

    private string baseURL = "https://api.mlab.com/api/1/";
    private string API_Key = "";
    private string db_name = "heroku_pm1crn83";

    private string collection_name;
    private string db_result;

    IEnumerator myCoroutine;
    public bool updateThis;

    private void Start()
    {
        //   myCoroutine = UpdateBiographies;
#if UNITY_EDITOR
        EditorApplication.update += EditorUpdate;
        myCoroutine = UpdateFromDatabase();
#endif

    }

    void EditorUpdate()
    {
        if (updateThis)
        {
            myCoroutine.MoveNext();

        }
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

    public IEnumerator UpdateFromDatabase()
    {
        API_Key = GetAPIKey(config.text);
        yield return StartCoroutine("UpdateBiographies");
        yield return StartCoroutine("UpdateAbout");
        yield return StartCoroutine("UpdateProgram");
        yield return StartCoroutine("UpdateWatch");
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
    IEnumerator UpdateWatch()
    {
        //Get the biographies from the database
        collection_name = "Watch";
        yield return StartCoroutine("GetCollectionFromDatabase");
        Watch_Factory.CreateWatchPage(db_result);
        yield break;
    }
    IEnumerator UpdateAbout()
    {
        //Get the biographies from the database
        collection_name = "About";
        yield return StartCoroutine("GetCollectionFromDatabase");
        About_Factory.CreateAboutPages(db_result);
        yield break;
    }
    IEnumerator UpdateProgram()
    {
        //Get the biographies from the database
        collection_name = "Program";
        yield return StartCoroutine("GetCollectionFromDatabase");
        Program_Factory.CreateProgramPage(db_result);
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
            if (www.responseHeaders.ContainsKey("STATUS"))
            {
                if (www.responseHeaders["STATUS"] == "HTTP/1.1 200 OK")
                {
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
