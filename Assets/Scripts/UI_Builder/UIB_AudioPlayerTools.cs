using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

public class UIB_AudioPlayerTools : MonoBehaviour
{

    public AudioSource source;
    Button playbutton;
    Button backButton;
    Button fwdButton;
    TextMeshProUGUI time_label;
    TextMeshProUGUI maxtime_label;
    TMP_InputField AudioTimerInput;
    TextMeshProUGUI displayText;

    Image playImage;
    Image pauseImage;

    Scrollbar timeScroll;

    bool trig;
    bool shouldContinuePlaying;
    private bool DragOccurring;

    Transform ParentOfAudioToolComponents;

    // Use this for initialization
    public void Init()
    {
        ParentOfAudioToolComponents = transform.parent;
        shouldContinuePlaying = false;

        source = gameObject.GetComponentInChildren<AudioSource>();

        //Set up buttons for audiocontroller
        foreach (Button b in ParentOfAudioToolComponents.GetComponentsInChildren<Button>())
        {
            if (b.gameObject.name.Equals("Play"))
                playbutton = b;
        }
        if (playbutton != null)
        {
            playbutton.onClick.RemoveAllListeners();

            playbutton.onClick.AddListener(PlayButtonPressed);
            playbutton.transform.GetChild(0).gameObject.SetActive(true); //turn on the play button
            playbutton.transform.GetChild(1).gameObject.SetActive(false); //turn off the pause button
        }
        else
        {
            Debug.LogWarning("No play button");
        }

        foreach (Button b in ParentOfAudioToolComponents.GetComponentsInChildren<Button>())
        {
            if (b.gameObject.name.Equals("Back"))
                backButton = b;
        }
        if (playbutton != null)
        {
            backButton.onClick.AddListener(BackButtonPressed);
        }
        else
        {
            Debug.LogWarning("No Back button button");
        }

        foreach (Button b in ParentOfAudioToolComponents.GetComponentsInChildren<Button>())
        {
            if (b.gameObject.name.Equals("Forward"))
                fwdButton = b;
        }
        if (playbutton != null)
        {
            fwdButton.onClick.AddListener(FwdButtonPressed);
        }
        else
        {
            Debug.LogWarning("No fwd button");
        }

        //Set up scrollbar for audio controls
        foreach (Scrollbar sb in ParentOfAudioToolComponents.GetComponentsInChildren<Scrollbar>())
        {
            if (sb.gameObject.name.Equals("Time_Scroll"))
            {
                timeScroll = sb;
                var t = Mathf.InverseLerp(0, source.clip.length, source.time);

                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.InitializePotentialDrag;
                entry.callback.AddListener((eventData) => { OnBeginDrag(); });
                timeScroll.GetComponent<EventTrigger>().triggers.Add(entry);

                entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerUp;
                entry.callback.AddListener((eventData) => { OnDragEnd(); });
                timeScroll.GetComponent<EventTrigger>().triggers.Add(entry);
            }

        }
        if (timeScroll != null)
        {

        }
        else
        {
            Debug.LogWarning("No time scroll button");
        }

        //Add time field for audio length
        foreach (TextMeshProUGUI tl in transform.parent.GetComponentsInChildren<TextMeshProUGUI>())
        {
            if (tl.gameObject.name.Equals("Time_Text"))
                time_label = tl;
        }
        if (time_label != null)
        {

        }
        else
        {
            Debug.LogWarning("No time_label");
        }

        //Add time field for audio length
        foreach (TextMeshProUGUI tl in ParentOfAudioToolComponents.GetComponentsInChildren<TextMeshProUGUI>())
        {
            if (tl.gameObject.name.Equals("MaxTime_Text"))
                maxtime_label = tl;
        }
        if (maxtime_label != null)
        {
            maxtime_label.text = ConvertToClockTime(source.clip.length);
        }
        else
        {
            Debug.LogWarning("No maxtime_label");
        }

        //Set up the input field
        AudioTimerInput = ParentOfAudioToolComponents.GetComponentInChildren<TMP_InputField>();

        displayText = AudioTimerInput.transform.GetChild(0).Find("DisplayText").GetComponent<TextMeshProUGUI>();
        if (displayText == null)
            Debug.LogWarning("Uh Oh gameObject is missing");

        AudioTimerInput.onValueChanged.AddListener(OnInputFieldChanged);
        AudioTimerInput.onSubmit.AddListener(OnInputFieldSubmitted);
        // AudioTimerInput.onSubmit.AddListener(delegate { playbutton.onClick.Invoke(); });
        AudioTimerInput.onSelect.AddListener(delegate { AudioTimerInput.MoveToEndOfLine(true, true); });
        AudioTimerInput.text = "";

    }

    private void OnInputFieldSubmitted(string arg0)
    {
        string str = displayText.text.Split(':')[0] + displayText.text.Split(':')[1];

        AudioTimerInput.text = "";
        if (source.clip.length > StringToSecondsCount(str, ref arg0))
        {
            source.time = StringToSecondsCount(str, ref arg0);
        }
        else
        {
            Debug.LogWarning("length exceeds time remaining in clip");
        }

        AudioTimerInput.DeactivateInputField();

        shouldContinuePlaying = true;
        playbutton.onClick.Invoke();
        //TODO:deselect the input field
    }

    int timerIndex;
    private void OnInputFieldChanged(string arg0)
    {
        var outstr = "";
        displayText.text = ConvertToClockTime(StringToSecondsCount(AudioTimerInput.text, ref outstr));
    }

    // Update is called once per frame
    void Update()
    {
        if (time_label != null)
            time_label.text = ConvertToClockTime(source.time);

        if (timeScroll != null && !DragOccurring)
            timeScroll.value = Mathf.InverseLerp(0, source.clip.length, source.time);

        if (source != null)
        {
            //detect end of audio clip
            if (float.Equals(source.time, source.clip.length))
            {
                OnAudioClipEnd();
            }
        }
    }

    void OnAudioClipEnd()
    {
        Debug.Log("Haha");
;        timeScroll.value = 0;
        source.time = 0;
        if (source.isPlaying)
            PlayButtonPressed();
        playbutton.transform.GetChild(1).gameObject.SetActive(false); //turn off the play button
        playbutton.transform.GetChild(0).gameObject.SetActive(true); //turn on the pause button
    }

    void OnScrollValueChanged()
    {
        source.time = Mathf.Lerp(0, source.clip.length, timeScroll.value);
    }

    public void PlayButtonPressed()
    {
        if (shouldContinuePlaying)
        {
            shouldContinuePlaying = false;
            if (!source.isPlaying)
            {
                source.Play();
                playbutton.transform.GetChild(0).gameObject.SetActive(false); //turn off the play button
                playbutton.transform.GetChild(1).gameObject.SetActive(true); //turn on the pause button
                var sab = playbutton.GetComponent<Special_AccessibleButton>();
                sab.m_Text = "Pause";
                sab.SelectItem(true);
            }
            return;
        }

        if (source.isPlaying)
        {
            source.Pause();
            playbutton.transform.GetChild(0).gameObject.SetActive(true); //turn off the play button
            playbutton.transform.GetChild(1).gameObject.SetActive(false); //turn on the pause button
            var sab = playbutton.GetComponent<Special_AccessibleButton>();
            sab.m_Text = "Play";
            sab.SelectItem(true);
            return;
        }
        else if(!source.isPlaying)
        {
            source.Play();
            playbutton.transform.GetChild(0).gameObject.SetActive(false); //turn off the play button
            playbutton.transform.GetChild(1).gameObject.SetActive(true); //turn on the pause button
            var sab = playbutton.GetComponent<Special_AccessibleButton>();
            sab.m_Text = "Pause";
            sab.SelectItem(true);
            return;
        }
    }

    void FwdButtonPressed()
    {
        if (source.isPlaying)
        {
            if (source.time < source.clip.length - 30)
                source.time += 30;
            else
                source.time = source.clip.length - .01f;
        }
    }

    void BackButtonPressed()
    {
        if (source.time > 30)
            source.time -= 30;
        else
            source.time = 0;
    }

    string ConvertToClockTime(float t)
    {
        int minutes = Mathf.FloorToInt(t / 60);
        int seconds = Mathf.FloorToInt(t % 60);

        string secondsAsString;
        string minutesAsString;

        if (seconds < 10)
            secondsAsString = "0" + seconds;
        else
            secondsAsString = "" + seconds;

        if (minutes < 10)
            minutesAsString = "0" + minutes;
        else
            minutesAsString = "" + minutes;

        return minutesAsString + ":" + secondsAsString;
    }

    private int StringToSecondsCount(string v, ref string outStr)
    {
        var min = "00";
        var sec = "00";

        switch (v.Length)
        {
            case 0:
                min = "00";
                sec = "00";
                break;
            case 1:
                min = "00";
                sec = "0" + v;
                break;
            case 2:
                min = "00";
                sec = v;
                break;
            case 3:
                min = "0" + v[0];
                sec = v[1] + "" + v[2];
                break;
            case 4:
                min = v[0] + "" + v[1];
                sec = v[2] + "" + v[3];
                break;
            default:
                v = "";
                min = "00";
                sec = "00";

                AudioTimerInput.OnSelect(null);
                OnInputFieldSubmitted(min + ":" + sec + v);
                break;
        }
        outStr = min + ":" + sec + v;
        return (int.Parse(min) * 60) + int.Parse(sec);
    }

    public void OnBeginDrag()
    {
        if (source.isPlaying)
        {
            source.Pause();
            DragOccurring = true;
        }
    }

    public void OnDragEnd()
    {
        source.UnPause();
        if (timeScroll.value < 1)
            source.time = Mathf.Lerp(0, source.clip.length, timeScroll.value);
        else
            source.time = source.clip.length - .1f;

        if (!source.isPlaying)
        {
            source.UnPause();
            DragOccurring = false;
        }
    }


}
