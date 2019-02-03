using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Amazon.S3.Model;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class FileManager : MonoBehaviour {

    //db name heroku_pm1crn83
    // Use this for initialization
    public TextAsset config;

    private string baseURL = "https://api.mlab.com/api/1/";
    private string API_Key = "";
    private string db_name = "heroku_pm1crn83";

    private string collection_name;
    private string db_result;

    public float TimeOutLength;

    private void Start()
    {

    }
    private void Update()
    {

    }

    internal static void WriteFileFromResponse(GetObjectResponse response, string fileName)
    {
        string destination = Application.persistentDataPath + "/" + fileName;
        StreamWriter sw;
        FileStream file;

        string binary = response.Key;

        //If the file does not exist, create the file and write data to it
        file = File.Create(destination);
        file.Close();
        sw = new StreamWriter(destination, true);
        sw.WriteLine(binary);
        sw.Close();
    }

    void WriteJsonFromWeb(string data, string fileName){
        string destination = Application.persistentDataPath + "/"+fileName;
        FileStream file;
        StreamReader sr;
        StreamWriter sw;

        //if no data returned, do not continue
        if (data == "" || data == null)
        {
            return;
        }
        string jsonToWrite = "{\"data\":" + data + "}";

        //Open the local file
        if (File.Exists(destination))
        {
            //If the file exists, compare the two versions
            //If they are different. overwrite the old version
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

    public static void WriteJsonUnModified(string data, string fileName)
    {
        string destination = Application.persistentDataPath + "/" + fileName;
        FileStream file;
        StreamReader sr;
        StreamWriter sw;

        //if no data returned, do not continue
        if (data == "" || data == null)
        {
            return;
        }
        string jsonToWrite = data;

        //Open the local file
        if (File.Exists(destination))
        {
            //If the file exists, compare the two versions
            //If they are different. overwrite the old version
            sr = File.OpenText(destination);
            var oldJson = sr.ReadToEnd();
            sr.Close();

            oldJson = oldJson.Remove(oldJson.Length - 1, 1);
            //   oldJson = oldJson.Remove(oldJson.Length - 1, 1);
            if (oldJson.Equals(jsonToWrite))
            {
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
