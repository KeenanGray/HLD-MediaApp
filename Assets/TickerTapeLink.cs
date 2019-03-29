using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI_Builder;

public class TickerTapeLink : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<UIB_Button>().Button_Opens = UIB_Button.UIB_Button_Activates.InAppUrl;
        GetComponent<UIB_Button>().Title = "Ticker Tape (CC Score)";
        GetComponent<UIB_Button>().s_link = "https://player.vimeo.com/video/326222070";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
