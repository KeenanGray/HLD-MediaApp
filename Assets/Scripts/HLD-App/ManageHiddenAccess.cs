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

    public string CodeButtonName;
    public string PageLinkButtonName;

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
        GetComponent<TMP_InputField>().shouldHideMobileInput = true;

        if (CodeButtonName == null || CodeButtonName == "")
        {
            CodeButtonName = gameObject.name.Split('_')[0] + "-Code_Button";
        }

        if (PageLinkButtonName == null || PageLinkButtonName == "")
        {
            PageLinkButtonName = gameObject.name.Split('_')[0] + "-Info_Button";
        }

        CodeButton = GameObject.Find(CodeButtonName);
        ls = GameObject.Find("LandingScreen");

        GetComponentInParent<UIB_Page>().AssetBundleRequired = true;
        UIB_AssetBundleHelper.InsertAssetBundle("hld/general");
    }

    private void CheckIsCorrect(string arg0)
    {

        var res = "";
        res = UIB_FileManager.ReadTextAssetBundle("AccessCode", "hld/general");

        if (res != "")
        {
            if (arg0.ToLower() == res.ToString().ToLower())
            {
                StartCoroutine("OnCorrectCode1");
            }
            else
            {
                Debug.Log("try enterring passcode " + res.ToString());

                if (UAP_AccessibilityManager.IsActive())
                {
                    UAP_AccessibilityManager.Say("Incorrect Code: Enter Code again");
                }
            }
        }
        else
        {
            Debug.Log("res is not assigned");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator OnCorrectCode1()
    {
        if (UAP_AccessibilityManager.IsActive())
        {
            UAP_AccessibilityManager.Say("Correct Code: Welcome to the show.", false, true, UAP_AudioQueue.EInterrupt.All);
        }
        GetComponent<TMP_InputField>().enabled = false;

        yield return new WaitForSeconds(1.0f);


        while (UAP_AccessibilityManager.IsSpeaking())
            yield return new WaitForSeconds(0.25f);

        GetComponent<TMP_InputField>().enabled = true;

        //HACK: call coroutine twice for it to work?!?

        yield return OnCorrectCode2();
        yield return OnCorrectCode2();

    }

    IEnumerator OnCorrectCode2()
    {
        yield return new WaitForEndOfFrame();
        CodeButtonName = gameObject.name.Split('_')[0] + "-Code_Button";
        CodeButton = GameObject.Find(CodeButtonName);

        if (CodeButton != null)
        {
            CodeButton.name = PageLinkButtonName;

            //TODO: Figure out why this hack works. 
            var pageName = PageLinkButtonName.Replace("_Button", "_Page");
            var gmobj = GameObject.Find(pageName);

            gmobj.GetComponent<UIB_Page>().OnActivated += delegate
            {
                GetComponentInParent<UIB_Page>().DeActivate();
            };

            CodeButton.GetComponent<UIB_Button>().Init();
            CodeButton.SetActive(false);
            CodeButton.SetActive(true);
            CodeButton.GetComponent<UIB_Button>().Init();

            //add a player-pref that states we have accessed this page
            PlayerPrefs.SetString(pageName, DateTime.UtcNow.ToString());
        }
        else
        {
            Debug.Log("Code Button is Null");
        }

        var button = GameObject.Find("DISPLAYED-Info_Button").GetComponent<UnityEngine.UI.Button>();

        var ab = button.GetComponent<UIB_Button>();
        ab.Init();
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
        UAP_AccessibilityManager.SelectElement(audioDesc, true);
        yield break;

    }

}
