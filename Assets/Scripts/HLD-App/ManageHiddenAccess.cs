using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UI_Builder;
using UnityEngine.EventSystems;

public class ManageHiddenAccess : MonoBehaviour, ISelectHandler
{
    GameObject AudioDescription_Button = null;
    GameObject CodeButton;

    public string CodeButtonName;
    public string PageLinkButtonName;

    private GameObject frame;
    private Vector2 moveDist;
    private Vector2 initPos;
    private Vector2 initSize;

    string oldValue;
    bool hasMoved;

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
        GetComponent<InputField>().onEndEdit.AddListener(CheckIsCorrect);
        GetComponent<InputField>().onEndEdit.AddListener(fieldDeSelected);
        GetComponent<InputField>().onValueChanged.AddListener(valueChanged);

        GetComponent<InputField>().shouldHideMobileInput = true;

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

        frame = GetComponentInParent<Mask>().gameObject;
        initSize = frame.GetComponent<RectTransform>().sizeDelta;
        initPos = frame.GetComponent<RectTransform>().localPosition;

        oldValue = "";
    }

    private void valueChanged(string currentVal)
    {
        if (currentVal.Length > oldValue.Length)
        {
            //say character added and full sentence
            UAP_AccessibilityManager.Say(currentVal[currentVal.Length - 1].ToString() + " added " + currentVal);
        }
        else
        {
            //say character deleted
            if (UAP_AccessibilityManager.IsActive())
            {
                UAP_AccessibilityManager.Say(oldValue[oldValue.Length - 1].ToString() + " deleted");
            }
        }
        oldValue = currentVal;
    }

    public void OnSelect(BaseEventData eventData)
    {
        fieldSelected();
    }

    private void fieldSelected()
    {
        StartCoroutine("fieldSelectedCo");
    }

    IEnumerator fieldSelectedCo()
    {
        hasMoved = false;

        //move the text field up so it is not obscured by keyboard
        var h = 0f;
        if (TouchScreenKeyboard.isSupported)
        {
            while (!TouchScreenKeyboard.visible)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogError("Mobile keyboard not supported");
#endif
        }

        h = TouchScreenKeyboard.area.height;

#if UNITY_EDITOR
        h = 873; //iphone X
        h = 264; //ipad 12-9
#endif
        //we have to change the mask size in case movement causes colision with logo and back button
        var sizeAdjust = new Vector2(0, GetComponent<RectTransform>().rect.height * 3);
        frame.GetComponent<RectTransform>().sizeDelta -= sizeAdjust;
        moveDist = new Vector2(0, h);

        //set to elevated position;
        frame.GetComponent<RectTransform>().anchoredPosition += moveDist;
        hasMoved = true;
        yield break;
    }

    private void fieldDeSelected(string arg0)
    {
        if (TouchScreenKeyboard.isSupported)
        {
            if (hasMoved && !TouchScreenKeyboard.visible)
            {
                //set back to initial position;
                frame.GetComponent<RectTransform>().localPosition = initPos;
                frame.GetComponent<RectTransform>().sizeDelta = new Vector2(initSize.x, initSize.y);
                hasMoved = false;
            }
        }
        else
        {
            //set back to initial position;
            frame.GetComponent<RectTransform>().localPosition = initPos;
            frame.GetComponent<RectTransform>().sizeDelta = new Vector2(initSize.x, initSize.y);
            hasMoved = false;
        }

#if UNITY_IOS || UNITY_ANDROID && !UNITY_EDITOR
        StartCoroutine("SetInputPositionBack");
#endif

    }


    private void CheckIsCorrect(string arg0)
    {
        if (TouchScreenKeyboard.isSupported)
        {
            if (GetComponent<InputField>().touchScreenKeyboard.status != TouchScreenKeyboard.Status.Done)
            {
                Debug.Log("NOT DONE");
                return;
            }
        }

        var res = "";
        res = UIB_FileManager.ReadTextAssetBundle("AccessCode", "hld/general");

        if (res != "")
        {
            if (arg0.ToLower() == res.ToString().ToLower())
            {
                //Debug.Log("Answered " + arg0.ToLower() + " " + res.ToString().ToLower());
                StartCoroutine("OnCorrectCode1");
            }
            else
            {
                //  Debug.Log("try enterring passcode " + res.ToString());
                if (UAP_AccessibilityManager.IsActive())
                {
                    UAP_AccessibilityManager.Say(" \n\r");
                    GameObject.Find("Accessibility Manager").GetComponent<UAP_AccessibilityManager>().SayPause(.1f);
                    UAP_AccessibilityManager.SayAs("Incorrect Code: Enter Code again", UAP_AudioQueue.EAudioType.App);
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
            UAP_AccessibilityManager.SelectElement(UAP_AccessibilityManager.GetCurrentFocusObject());
            UAP_AccessibilityManager.StopSpeaking();
            UAP_AccessibilityManager.Say("Correct Code: Welcome to the show.", false, true, UAP_AudioQueue.EInterrupt.All);
        }
        GetComponent<InputField>().enabled = false;

        if (UAP_AccessibilityManager.IsActive())
            yield return new WaitForSeconds(1.0f);

        while (UAP_AccessibilityManager.IsSpeaking())
            yield return new WaitForSeconds(0.25f);

        GetComponent<InputField>().enabled = true;

        //HACK: call coroutine twice for it to work?!?

        yield return OnCorrectCode2();
        yield return OnCorrectCode2();

    }

    IEnumerator OnCorrectCode2()
    {
        yield return new WaitForEndOfFrame();
        GetComponent<InputField>().text = "";
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
            //Debug.Log("Code Button is Null");
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

    IEnumerator SetInputPositionBack()
    {
        while (true)
        {
            Debug.Log("THIS IS HAPPENING 2");
            if (TouchScreenKeyboard.isSupported && TouchScreenKeyboard.visible)
            {
                yield return null;
            }
            else
            {
                //set back to initial position;
                frame.GetComponent<RectTransform>().localPosition = initPos;
                frame.GetComponent<RectTransform>().sizeDelta = new Vector2(initSize.x, initSize.y);
                hasMoved = false;
                yield break;
            }
        }
    }
}
