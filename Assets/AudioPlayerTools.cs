using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class AudioPlayerTools : MonoBehaviour                                                        {

    AudioSource source;
    Button playbutton;
    Button backButton;
    Button fwdButton;
    TextMeshProUGUI time_label;
    TextMeshProUGUI maxtime_label;

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

        foreach (Button b in GetComponentsInChildren<Button>())
        {
            if (b.gameObject.name.Equals("Play_Button"))
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
            if (b.gameObject.name.Equals("Back_Button"))
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
            if (b.gameObject.name.Equals("Fwd_Button"))
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

            timeScroll.onValueChanged.AddListener(delegate { OnScrollValueChanged(); });
        }
        if (timeScroll != null)
        {

        }
        else
        {
            Debug.LogWarning("No play button");
        }

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

      
    }

  

    void PauseAudio(){

    }

    public void OnGUI()
    {
        time_label.text = ConvertToClockTime(source.time);

        var t = Mathf.InverseLerp(0, source.clip.length, source.time);
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
        }
        else
        {
            source.Play();
            playbutton.transform.GetChild(0).gameObject.SetActive(false); //turn off the play button
            playbutton.transform.GetChild(1).gameObject.SetActive(true); //turn on the pause button
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

    public void OnDrag()
    {
        Debug.Log("on Drag");
        source.Pause();
        if (source.isPlaying)
            restartAtEndOfDrag = true;
       // Debug.Log("Dragging");
    }
    public void OnDragEnd()
    {
        Debug.Log("HAHAH");
        source.UnPause();
        if (restartAtEndOfDrag)
        {
            source.UnPause();
            restartAtEndOfDrag = false;
        }

        // Debug.Log("Dragging");
    }
}
