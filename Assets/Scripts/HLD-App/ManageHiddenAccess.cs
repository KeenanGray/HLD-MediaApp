using System;
using System.Collections;
using System.Collections.Generic;
using UI_Builder;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

       // GetComponent<InputField>().shouldHideMobileInput = true;

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

        //  UIB_AssetBundleHelper.InsertAssetBundle("hld/general");
        frame = GetComponentInParent<Mask>().gameObject;
        initSize = frame.GetComponent<RectTransform>().sizeDelta;
        initPos = frame.GetComponent<RectTransform>().localPosition;

        oldValue = "";

        UAP_VirtualKeyboard.SetOnFinishListener (CheckIsCorrect);
    }

    private void CheckIsCorrect(string arg0, bool arg1)
    {
        CheckIsCorrect (arg0);
    }

    private void valueChanged(string currentVal)
    {
        if (currentVal.Length > oldValue.Length)
        {
            //say character added and full sentence
            UAP_AccessibilityManager
                .Say(currentVal[currentVal.Length - 1].ToString() +
                " added " +
                currentVal);
        }
        else
        {
            //say character deleted
            if (UAP_AccessibilityManager.IsActive())
            {
                UAP_AccessibilityManager
                    .Say(oldValue[oldValue.Length - 1].ToString() + " deleted");
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
#if !UNITY_ANDROID
        UAP_AccessibilityManager.BlockInput(true);
        StartCoroutine("fieldSelectedCo");
#endif
    }

    IEnumerator fieldSelectedCo()
    {

        //move the text field up so it is not obscured by keyboard
        var h = 873f;
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
            //Debug.LogError("Mobile keyboard not supported");
#endif
        }

        //we have to change the mask size in case movement causes colision with logo and back button
        var sizeAdjust =
            new Vector2(0, GetComponent<RectTransform>().rect.height * 3);
        frame.GetComponent<RectTransform>().sizeDelta -= sizeAdjust;

        //set to elevated position;
        frame.GetComponent<RectTransform>().anchoredPosition += moveDist;
        yield break;
    }

    private void fieldDeSelected(string arg0)
    {
        
#if !UNITY_ANDROID
/*
        UAP_AccessibilityManager.BlockInput(false);

        if (TouchScreenKeyboard.isSupported)
        {
            if (hasMoved && !TouchScreenKeyboard.visible)
            {
                //set back to initial position;
                frame.GetComponent<RectTransform>().localPosition = initPos;
                frame.GetComponent<RectTransform>().sizeDelta =
                    new Vector2(initSize.x, initSize.y);
                hasMoved = false;
            }
        }
        else
        {
            //set back to initial position;
            frame.GetComponent<RectTransform>().localPosition = initPos;
            frame.GetComponent<RectTransform>().sizeDelta =
                new Vector2(initSize.x, initSize.y);
            hasMoved = false;
        }
        */
#endif

    }

    private void CheckIsCorrect(string arg0)
    {
        Debug.Log (arg0);

        var ShowName = name.Split('_')[0];

        var res = "";

        res =
            UIB_FileManager
                .ReadTextAssetBundle(ShowName + "AccessCode", "hld/general");

        if (res != "")
        {
            if (arg0.ToLower().Trim() == res.ToString().ToLower().Trim())
            {
                StartCoroutine("OnCorrectCode1");
            }
            else
            {
                Debug.Log("Correct Code is: " + res + " you entered " + arg0);
                if (UAP_AccessibilityManager.IsActive())
                {
                    UAP_AccessibilityManager.Say(" \n\r");
                    GameObject
                        .Find("Accessibility Manager")
                        .GetComponent<UAP_AccessibilityManager>()
                        .SayPause(.1f);
                    UAP_AccessibilityManager
                        .SayAs("Incorrect Code: Enter Code again",
                        UAP_AudioQueue.EAudioType.App);
                }
            }
        }
        else
        {
            Debug.Log("res is not assigned " + gameObject.name);
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
            UAP_AccessibilityManager
                .SelectElement(UAP_AccessibilityManager
                    .GetCurrentFocusObject());
            UAP_AccessibilityManager.StopSpeaking();
            UAP_AccessibilityManager
                .Say("Correct Code: Welcome to the show.",
                false,
                true,
                UAP_AudioQueue.EInterrupt.All);
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

            gmobj.GetComponent<UIB_Page>().OnActivated +=
                delegate ()
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
        }
        var ShowName = name.Split('_')[0];

        var button =
            GameObject
                .Find(ShowName + "-Info_Button")
                .GetComponent<UnityEngine.UI.Button>();

        var ab = button.GetComponent<UIB_Button>();
        ab.Init();
        button.onClick.Invoke();
        UAP_AccessibilityManager.StopSpeaking();
    }
/*
    IEnumerator SetInputPositionBack()
    {
        while (true)
        {
            if (TouchScreenKeyboard.isSupported && TouchScreenKeyboard.visible)
            {
                yield return null;
            }
            else
            {
                //set back to initial position;
                frame.GetComponent<RectTransform>().localPosition = initPos;
                frame.GetComponent<RectTransform>().sizeDelta =
                    new Vector2(initSize.x, initSize.y);
                hasMoved = false;
                yield break;
            }
        }
    }
*/
    public int GetKeyboardSize()
    {

#if UNITY_ANDROID && !UNITY_EDITOR
        /*
        using (
            AndroidJavaClass UnityClass =
                new AndroidJavaClass("com.unity3d.player.UnityPlayer")
        )
        {
            AndroidJavaObject View =
                UnityClass
                    .GetStatic<AndroidJavaObject>("currentActivity")
                    .Get<AndroidJavaObject>("mUnityPlayer")
                    .Call<AndroidJavaObject>("getView");

            using (
                AndroidJavaObject Rct =
                    new AndroidJavaObject("android.graphics.Rect")
            )
            {
                View.Call("getWindowVisibleDisplayFrame", Rct);

                return Screen.height - Rct.Call<int>("height");
            }
        }
*/

#endif

        return 873;
    }
}
