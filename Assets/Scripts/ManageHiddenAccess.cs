using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ManageHiddenAccess : MonoBehaviour {
    public GameObject ViewToShow;
    public GameObject ViewToHide;

    public class PassPhraseArray
    {
        public Passphrase[] data;
    }

    [System.Serializable]
    public class Passphrase
    {
        public string Date;
        public string Code;
    }

    GameObject ls;
    List<GameObject> hiddenPages;

    // Use this for initialization
    public void Start () {
        GetComponent<TMP_InputField>().onEndEdit.AddListener(CheckIsCorrect);
        ls = GameObject.Find("LandingScreen");

       // hiddenPages = new List<GameObject>();
       // foreach (GameObject go in GameObject.FindGameObjectsWithTag("Hidden"))
       // {
       //    hiddenPages.Add(go);
       // }

        ViewToShow.SetActive(false);
        ViewToHide.SetActive(true);
    }

    private void CheckIsCorrect(string arg0)
    {
        var res = MongoLib.ReadJson("AccessCode.json");
        if (res != ""){
            PassPhraseArray myObject = JsonUtility.FromJson<PassPhraseArray>(res);

//compare the two strings
//non-case sensitive
            if (arg0.ToLower() == myObject.data[0].Code.ToLower())
            {
              /*    foreach (GameObject go in hiddenPages)
                {
                    go.SetActive(true);
                }
            

                GetComponentInParent<Page>().MoveScreenOut();
                    ls.GetComponent<Page>().MoveScreenIn();
*/
        ViewToShow.SetActive(true);
        ViewToHide.SetActive(false);            
            }
            else
            {
                Debug.Log("try enterring passcode " + myObject.data[0].Code);
            }


        }
//        Debug.Log("access code " + res);
    }

    // Update is called once per frame
    void Update () {
		
	}


}
