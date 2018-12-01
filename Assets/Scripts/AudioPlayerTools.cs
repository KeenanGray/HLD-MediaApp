using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

public class AudioPlayerTools : MonoBehaviour {

    AudioSource source;
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
    private float scrollPos;
    
    bool trig;
    private bool restartAtEndOfDrag;

    // Use this for initialization
    void Start()
    {
        source = gameObject.GetComponentInChildren<AudioSource>();

        //Set up buttons for audiocontroller
        foreach (Button b in GetComponentsInChildren<Button>())
        {
            if (b.gameObject.name.Equals("Play"))
                playbutton = b;
        }
        if (playbutton != null)
        {
            playbutton.onClick.AddListener(PlayButtonPressed);
            playbutton.transform.GetChild(0).gameObject.SetActive(true); //turn on the play button
            playbutton.transform.GetChild(1).gameObject.SetActive(false); //turn off the pause button
        }
        else
        {
            Debug.LogWarning("No play button");
        }

        foreach (Button b in GetComponentsInChildren<Button>())
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

        foreach (Button b in GetComponentsInChildren<Button>())
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
        foreach (Scrollbar sb in GetComponentsInChildren<Scrollbar>())
        {
            if (sb.gameObject.name.Equals("Time_Scroll"))
            {
                timeScroll = sb;
                var t = Mathf.InverseLerp(0, source.clip.length, source.time);
                timeScroll.value = t;

                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.InitializePotentialDrag;
                entry.callback.AddListener((eventData) => { OnDrag(); });
                timeScroll.GetComponent<EventTrigger>().triggers.Add(entry);


                entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerUp;
                entry.callback.AddListener((eventData) => { OnDragEnd(); });
                timeScroll.GetComponent<EventTrigger>().triggers.Add(entry);
            }

            timeScroll.onValueChanged.AddListener(MoveToEndOfLine);
        }
        if (timeScroll != null)
        {

        }
        else
        {
            Debug.LogWarning("No play button");
        }

        //Add time field for audio length
        foreach (TextMeshProUGUI tl in GetComponentsInChildren<TextMeshProUGUI>())
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
        foreach (TextMeshProUGUI tl in GetComponentsInChildren<TextMeshProUGUI>())
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
        AudioTimerInput = GetComponentInChildren<TMP_InputField>();

        displayText = AudioTimerInput.transform.GetChild(0).Find("DisplayText").GetComponent<TextMeshProUGUI>();
        if(displayText==null)
            Debug.LogWarning("Uh Oh gameObject is missing");

        AudioTimerInput.onValueChanged.AddListener(OnInputFieldChanged);
        AudioTimerInput.onSubmit.AddListener(OnInputFieldSubmitted);
        AudioTimerInput.onSelect.AddListener(delegate { AudioTimerInput.MoveToEndOfLine(true,true); });
        AudioTimerInput.text = "";
    }

    private void MoveToEndOfLine(float arg0)
    {
        Debug.Log("selected");
        AudioTimerInput.MoveTextEnd(false);
        AudioTimerInput.MoveToEndOfLine(false,false);
        AudioTimerInput.caretPosition = 5;
    }

    private void OnInputFieldSubmitted(string arg0)
    {
        string str = displayText.text.Split(':')[0] + displayText.text.Split(':')[1];

        AudioTimerInput.text = "";
        if (source.clip.length > StringToSecondsCount(str,ref arg0)){
            source.time = StringToSecondsCount(str, ref arg0);
        }
        else{
            Debug.LogWarning("length exceeds time remaining in clip");
        }

        AudioTimerInput.DeactivateInputField();

        if (!source.isPlaying)
            PlayButtonPressed();
        //TODO:deselect the input field
    }

    int timerIndex;
    private void OnInputFieldChanged(string arg0)
    {
        /*    AudioTimerInput.caretPosition = 5;
            arg0 = arg0.Insert(5, "_");
            var customTime = arg0.Split('_')[1];

            ConvertToClockTime(StringToSecondsCount(customTime,ref customTime));
            AudioTimerInput.text = customTime;

            customTime = customTime.Insert(5, "/");

            displayText.text = customTime.Split('/')[0];
            */

        var outstr = "";
            displayText.text = ConvertToClockTime(StringToSecondsCount(AudioTimerInput.text, ref outstr));


    }

    void HandleUnityAction(string arg0)
    {
    }

    void PauseAudio(){

    }

    public void OnGUI()
    {
        time_label.text = ConvertToClockTime(source.time);

        var t = Mathf.InverseLerp(0, source.clip.length, source.time);
        if(timeScroll!=null)
            timeScroll.value = t;

    }
    // Update is called once per frame
    void Update () {
		
	}

    void OnScrollValueChanged(){
        source.time = Mathf.Lerp(0, source.clip.length, timeScroll.value);
    }

    void PlayButtonPressed(){
        if (source.isPlaying)
        {
            source.Pause();
            playbutton.transform.GetChild(0).gameObject.SetActive(true); //turn off the play button
            playbutton.transform.GetChild(1).gameObject.SetActive(false); //turn on the pause button
            var sab = playbutton.GetComponent<Special_AccessibleButton>();
            sab.m_Text = "Play";
            sab.SelectItem(true);
        }
        else
        {
            source.Play();
            playbutton.transform.GetChild(0).gameObject.SetActive(false); //turn off the play button
            playbutton.transform.GetChild(1).gameObject.SetActive(true); //turn on the pause button
            var sab = playbutton.GetComponent<Special_AccessibleButton>();
            sab.m_Text = "Pause";
            sab.SelectItem(true);

        }
    }

    void FwdButtonPressed()
    {
        if (source.isPlaying)
        {
            if (source.time < source.clip.length - 30)
                source.time += 30;
            else
                source.time = source.clip.length;
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
            secondsAsString = ""+seconds;

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
        outStr = min + ":" + sec  + v;
        return (int.Parse(min) * 60) + int.Parse(sec);
    }

    public void OnDrag()
    {
        source.Pause();
        if (source.isPlaying)
            restartAtEndOfDrag = true;
       // Debug.Log("Dragging");
    }
    public void OnDragEnd()
    {
        source.UnPause();
        if (restartAtEndOfDrag)
        {
            source.UnPause();
            restartAtEndOfDrag = false;
        }

        // Debug.Log("Dragging");
    }
}
