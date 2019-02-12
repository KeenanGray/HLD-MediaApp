﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Amazon.S3.Model;
using HLD;
using UnityEditor;
using UnityEngine;

namespace UI_Builder
{

    enum UIB_FileTypes
    {
        Images,
        Videos,
        Text
    }

    public class FileManager : MonoBehaviour
    {
        //db name heroku_pm1crn83
        // Use this for initialization
        public TextAsset config;
        
        public float TimeOutLength;

        private void Start()
        {

        }
        private void Update()
        {

        }

        private string GetAPIKey(string v)
        {
            foreach (string s in v.Split('\n'))
            {
                if (s.Contains("api_key"))
                {
                    return s.Split('=')[1];
                }
            }
            return v;
        }

        internal static byte[] ReadFromBytes(string path, UIB_FileTypes kind)
        {
            var ext = "";
            byte[] fileData = null;
            switch (kind)
            {
                case UIB_FileTypes.Images:
                    ext = ".jpg";
                    if (File.Exists(path + ext))
                    {
                        fileData = File.ReadAllBytes(path + ext);
                    }
                    ext = ".jpeg";
                    if (File.Exists(path + ext))
                    {
                        fileData = File.ReadAllBytes(path + ext);
                    }
                    ext = ".png";
                    if (File.Exists(path + ext))
                    {
                        fileData = File.ReadAllBytes(path + ext);
                    }
                    break;
                case UIB_FileTypes.Videos:
                    Debug.Log("Videos");
                    break;
                case UIB_FileTypes.Text:
                    Debug.Log("Text");
                    break;
                default:
                    Debug.Log("Default case");
                    break;
            }

            return fileData;
        }

        internal static void WriteFileFromResponse(GetObjectResponse response, string fileName)
        {
            string destination = Application.persistentDataPath + "/";

            //check if the directory exists
            //split filepath into directory and filename
            int cont = 0;
            string name = "";
            string directory = "";

            foreach (string i in fileName.Split('/'))
            {
                cont++;
                if (cont >= fileName.Split('/').Length)
                {
                    name = fileName.Split('/')[cont - 1];
                    break;
                }
                else
                {
                    directory = directory + "/" + i;
                }
            }
            //            Debug.Log("file " + name);
            //            Debug.Log("dir " + directory);

            var newpath = destination + "/" + directory;
            if (!Directory.Exists(newpath))
            {
                Directory.CreateDirectory(newpath);
            }
            else
            {
            }
            using (var fs = System.IO.File.Create(newpath + "/" + name))
            {
                byte[] buffer = new byte[81920];
                int count;
                while ((count = response.ResponseStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    fs.Write(buffer, 0, count);
                }
                fs.Flush();
            }

        }

        void WriteJsonFromWeb(string data, string fileName)
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
            string jsonToWrite = "{\"data\":" + data + "}";

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

        internal static bool FileExists(string fileName)
        {
            string destination = Application.persistentDataPath + "/" + fileName;
            //            Debug.Log(destination);
            return File.Exists(destination);
        }
        internal static bool FileExists(string path, UIB_FileTypes kind)
        {
            var ext = "";
            switch (kind)
            {
                case UIB_FileTypes.Images:
                    ext = ".jpg";
                    if (File.Exists(path + ext))
                    {
                        return true;
                    }
                    ext = ".jpeg";
                    if (File.Exists(path + ext))
                    {
                        return true;
                    }
                    ext = ".png";
                    if (File.Exists(path + ext))
                    {
                        return true;
                    }
                    break;
                case UIB_FileTypes.Videos:
                    Debug.Log("Videos");
                    break;
                case UIB_FileTypes.Text:
                    Debug.Log("Text");
                    break;
                default:
                    Debug.Log("Default case");
                    break;
            }
            return false;
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

        internal static string GetFileNameFromKey(string key)
        {
            //remove the file ext
            var val = key.Split('.')[0];
            //split the string by '/' and get the last one
            val = val.Split('/')[val.Split('/').Length - 1];
            return val;
        }

        public static string ReadTextFile(string fileName)
        {
            string destination = Application.persistentDataPath + "/" + fileName;
            StreamReader sr;

            string jsonStr = "";
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
            else
            {
                Debug.LogWarning("no file found " + fileName);
                return "";
            }

        }
    }
}