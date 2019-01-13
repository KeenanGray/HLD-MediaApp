using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UI_Builder;

public class ManageHiddenAccess : MonoBehaviour
{
    GameObject AudioDescription_Button = null;
    GameObject CodeButton;

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

    GameObject ls = null;
    List<GameObject> hiddenPages;

    // Use this for initialization
    public void Start()
    {
        AudioDescription_Button = GameObject.Find("AudioDescription_Button");
        GetComponent<TMP_InputField>().onEndEdit.AddListener(CheckIsCorrect);
        CodeButton = GameObject.Find("DISPLAYED-Code_Button");
        ls = GameObject.Find("LandingScreen");

    }

    private void CheckIsCorrect(string arg0)
    {
        var res = MongoLib.ReadJson("AccessCode.json");
        if (res != "")
        {
            PassPhraseArray myObject = JsonUtility.FromJson<PassPhraseArray>(res);

            //compare the two strings
            //non-case sensitive
            if (myObject == null)
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
                      ls.GetComponent<Page>().
                MoveScreenIn();
          */
                //HACK: call coroutine twice for it to work?!?
                StartCoroutine("OnCorrectCode");
                StartCoroutine("OnCorrectCode");


            }
            else
            {
                Debug.Log("try enterring passcode " + myObject.data[0].Code);
            }

        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator OnCorrectCode()
    {
        yield return new WaitForEndOfFrame();

        if (CodeButton != null)
        {
            CodeButton.name = "DISPLAYED-Info_Button";

            // go.GetComponent<UIB_Button>().Init();

            //TODO: Figure out why this hack works. 
            var gmobj = GameObject.Find("DISPLAYED-Info_Page");

            gmobj.GetComponent<UIB_Page>().OnActivated +=delegate {
                GetComponentInParent<UIB_Page>().DeActivate();
            };

            CodeButton.GetComponent<UIB_Button>().Init();
            CodeButton.SetActive(false);
            CodeButton.SetActive(true);
            CodeButton.GetComponent<UIB_Button>().Init();

        }
        else
        {
            Debug.Log("Code Button is Null");
        }

        var button = GameObject.Find("DISPLAYED-Info_Button").GetComponent<UnityEngine.UI.Button>();
        
        var ab = button.GetComponent<UIB_Button>();
        ab.Init();
        Debug.Log(button.name);
        button.onClick.Invoke();

        UAP_AccessibilityManager.StopSpeaking();
        var audioDesc = GameObject.Find("AudioDescription_Button");
        audioDesc.SetActive(false);

        ab.SetVO(audioDesc);

        audioDesc.SetActive(true);

        yield return new WaitForSeconds(0.0f);
        audioDesc.GetComponent<UnityEngine.UI.Button>().enabled = true;
        audioDesc.GetComponent<Special_AccessibleButton>().enabled = true;
        yield return new WaitForSeconds(0.0f);
        UAP_AccessibilityManager.SelectElement(audioDesc,true);
        yield break;


    }

}
