using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UI_Builder;

public class ManageHiddenAccess : MonoBehaviour {
    GameObject AudioDescription_Button;

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
        AudioDescription_Button = GameObject.Find("AudioDescription_Button");
        GetComponent<TMP_InputField>().onEndEdit.AddListener(CheckIsCorrect);

        ls = GameObject.Find("LandingScreen");

       // hiddenPages = new List<GameObject>();
       // foreach (GameObject go in GameObject.FindGameObjectsWithTag("Hidden"))
       // {
       //    hiddenPages.Add(go);
       // }

    }

    private void CheckIsCorrect(string arg0)
    {
        var res = MongoLib.ReadJson("AccessCode.json");
        if (res != ""){
            PassPhraseArray myObject = JsonUtility.FromJson<PassPhraseArray>(res);

//compare the two strings
//non-case sensitive
            if(myObject==null)
            {
                    Debug.LogError("Uh oh");
                    return;
            }
            if (arg0.ToLower() == myObject.data[0].Code.ToLower())
            {
                /*    foreach (GameObject go in hiddenPages)
                  {
                      go.SetActive(true);
                  }


                  GetComponentInParent<Page>().MoveScreenOut();
                      ls.GetComponent<Page>().MoveScreenIn();
          */
                StartCoroutine("OnCorrectCode");

            }
            else
            {
                Debug.Log("try enterring passcode " + myObject.data[0].Code);
            }

        }
    }

    // Update is called once per frame
    void Update () {
		
	}

    IEnumerator OnCorrectCode(){

        var go = GameObject.Find("DISPLAYED-Code_Button");
        if (go != null)
        {
            GameObject.Find("DISPLAYED-Code_Button").name = "DISPLAYED-Info_Button";
        }
        else{
        }
        var button = GameObject.Find("DISPLAYED-Info_Button").GetComponent<UnityEngine.UI.Button>();

        GetComponentInParent<UIB_Page>().DeActivate();

            var ab = button.GetComponent<UI_Builder.UIB_Button>();
            ab.Init();

            var audioDesc = GameObject.Find("AudioDescription_Button");
            audioDesc.SetActive(false);

            ab.SetVO(audioDesc);

            button.onClick.Invoke();
            audioDesc.SetActive(true);

            yield return new WaitForSeconds(0.0f);
            audioDesc.GetComponent<UnityEngine.UI.Button>().enabled = true;
            audioDesc.GetComponent<Special_AccessibleButton>().enabled = true;
            yield return new WaitForSeconds(0.0f);
            UAP_AccessibilityManager.SelectElement(audioDesc);
            yield break;

    }

}
