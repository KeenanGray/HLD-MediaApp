using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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
    
    private void Start()
    {

    }
    private void Update()
    {

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
        //yield return StartCoroutine("UpdateAbout");
        //yield return StartCoroutine("UpdateProgram");
        //yield return StartCoroutine("UpdateWatch");
        yield return StartCoroutine("UpdatePassCode");
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
        WriteJson(db_result, "Bios.json");
        yield break;
    }
    IEnumerator UpdateWatch()
    {
        //Get the biographies from the database
        collection_name = "Watch";
        yield return StartCoroutine("GetCollectionFromDatabase");
      //  WriteJson(db_result, "Watch.json");
        yield break;
    }
    IEnumerator UpdateAbout()
    {
        //Get the biographies from the database
        collection_name = "About";
        yield return StartCoroutine("GetCollectionFromDatabase");
      //  WriteJson(db_result, "About.json");
        yield break;
    }
    IEnumerator UpdateProgram()
    {
        //Get the biographies from the database
        collection_name = "Program";
        yield return StartCoroutine("GetCollectionFromDatabase");
      //  WriteJson(db_result, "Program.json");
        yield break;
    }

    IEnumerator UpdatePassCode()
    {
        //Get the biographies from the database
        collection_name = "D.I.S.P.L.A.Y.E.D";
        yield return StartCoroutine("GetCollectionFromDatabase");
        WriteJson(db_result, "AccessCode.json");
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

    void WriteJson(string data, string fileName){
        string destination = Application.persistentDataPath + "/"+fileName;
        FileStream file;
        StreamReader sr;
        StreamWriter sw;

        string jsonToWrite = "{\"data\":" + data + "}";

        //Open the local file
        if (File.Exists(destination))
        {
            //If the file exists, compare the two versions
            //If they are differnet. overwrite the old version
            sr = File.OpenText(destination);
            var oldJson = sr.ReadToEnd();
            sr.Close();
            
            oldJson = oldJson.Remove(oldJson.Length-1, 1);
            //   oldJson = oldJson.Remove(oldJson.Length - 1, 1);
            if (oldJson.Equals(jsonToWrite)){
//                Debug.Log("JSON matches");
            }
            else
            {
                file = File.Create(destination);
                file.Close();
      //          Debug.Log("New JSON");
                sw = new StreamWriter(destination, true);
                sw.WriteLine(jsonToWrite);
                sw.Close();
            }
        }
        else
        {
            //If the file does not exist, create the file and write data to it

            file = File.Create(destination);
            file.Close();
            sw = new StreamWriter(destination, true);
            sw.WriteLine(jsonToWrite);
            sw.Close();
        }
    }

    public static string ReadJson(string fileName){
        string destination = Application.persistentDataPath + "/" + fileName;
        StreamReader sr;

        string jsonStr="";
        //Open the local file
        if (File.Exists(destination))
        {
            //If the file exists, read the file
            //TODO:
            sr = File.OpenText(destination);
            jsonStr = sr.ReadToEnd();
            sr.Close();

            return jsonStr;
        }
        else{
            Debug.LogWarning("no file found");
            return "";
        }
        
    }


}
