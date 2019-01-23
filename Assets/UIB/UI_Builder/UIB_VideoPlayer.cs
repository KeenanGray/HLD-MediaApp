using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;

namespace UI_Builder
{
    public class UIB_VideoPlayer : MonoBehaviour, UIB_IPage
    {

        GameObject cover;
        GameObject CaptionsCanvas;

        public VideoClip myClip;
        VideoPlayer myPlayer;
        TextAsset VideoCaptions;
        public RenderTexture blank;
        RenderTexture videoTexture;
        public GameObject OriginScreen;

        // Use this for initialization
        void Start()
        {
            cover = transform.Find("Cover").gameObject;
            CaptionsCanvas = transform.Find("CaptionsCanvas").gameObject;
            if (CaptionsCanvas == null)
                Debug.LogError("no canvas");

            myClip = GetComponent<VideoPlayer>().clip;
            myClip = null;

            myPlayer = GetComponent<VideoPlayer>();
            videoTexture = myPlayer.targetTexture;

            UnityEngine.UI.Button b = GetComponentInChildren<UnityEngine.UI.Button>();
            b.onClick.AddListener(OnStoppedByButton);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public IEnumerator PlayVideo()
        {
            WaitForSeconds delay = new WaitForSeconds(0.5f);

            while (myClip == null)
            {
                myClip = myPlayer.clip;
                yield return delay;
            }

            if (myClip == null)
                yield break;

            myPlayer.targetTexture = videoTexture;

            yield return delay;
            myPlayer.Prepare();
            yield return delay;

            myPlayer.enabled = false;
            myPlayer.enabled = true;


            myClip = myPlayer.clip;
            WaitForEndOfFrame wf = new WaitForEndOfFrame();
            myPlayer.Play();

            for (int j = 0; j < 10; j++)
                yield return wf;

            // yield return new WaitForSeconds(1.0f);
            cover.SetActive(false);

            StartCoroutine("PlayCaptionsWithVideo");

            var t0 = Time.time;
            var t1 = Time.time;

            //        Debug.Log(myClip.length + " passed " + (myClip.length - (t1 - t0)));

            double CurrLength = -1.0;

            if (myClip != null)
                CurrLength = myClip.length;

            while (CurrLength - (t1 - t0) >= 0)
            {
                t1 = Time.time;
                yield return delay;
            }


            yield return new WaitForSeconds(1.0f);
            OnClipEnd();

            yield break;
        }

        private void OnStoppedByButton()
        {
            cover.SetActive(true);

            CaptionsCanvas.GetComponentInChildren<TextMeshProUGUI>().text = "";
            StopCoroutine("PlayCaptionsWithVideo");
            Graphics.Blit(blank, videoTexture);

            myPlayer.targetTexture = blank;
            myPlayer.clip = null;
            myPlayer.Stop();
        }

        void OnClipEnd()
        {
            OnStoppedByButton();
            GetComponentInParent<UIB_Page>().StartCoroutine("MoveScreenOut",false);
            var listPage = GameObject.Find("#MeOnDisplay_Page");
            listPage.GetComponent<UIB_Page>().StartCoroutine("MoveScreenIn",false);
            UAP_AccessibilityManager.SelectElement(listPage);

        }

        public void SetVideoCaptions(TextAsset newText)
        {
            if (newText == null)
            {
                Debug.Log("Null Text Given for Captions");
                return;
            }
            VideoCaptions = newText;
        }

        IEnumerator PlayCaptionsWithVideo()
        {
            //set up video captions
            TextMeshProUGUI tmp = CaptionsCanvas.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp == null)
                Debug.LogWarning("couldnt find text");

            var words = GetNumberOfLines();

            int start = 0;
            int WordsPerLine = 6;
            string line = "";

            var TimePerLine = (myClip.length - 2)
                    /
                (words.Length / WordsPerLine);

            for (int i = start; i < words.Length; i += WordsPerLine)
            {
                line = "";
                for (int j = 0; j < WordsPerLine; j++)
                {
                    if (i + j < words.Length)
                        line += words[i + j] + " ";
                    else
                        break;
                }
                start += WordsPerLine;
                tmp.text = line;
                yield return new WaitForSeconds((float)TimePerLine);
            }

            yield break;
        }

        string[] GetNumberOfLines()
        {
            //Count all the words
            var words = VideoCaptions.text.Split(' ');
            return words;
        }

        public void Init()
        {
            GetComponent<UIB_Page>().OnActivated += PageActivatedHandler;
            GetComponent<UIB_Page>().OnDeActivated += PageDeActivatedHandler;

        }

        public void PageActivatedHandler()
        {
        }

        public void PageDeActivatedHandler()
        {
            StopAllCoroutines();
        }
    }
}