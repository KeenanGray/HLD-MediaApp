using System.Collections;
using System.Collections.Generic;
using UI_Builder;
using UnityEngine;

public class AssignOnDisplayFeedbackLink : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var b = GetComponent<UIB_Button>();
        b.Button_Opens = UIB_Button.UIB_Button_Activates.InAppUrl;
        b.Title = "On Display Feedback";
        b.s_link = "https://forms.gle/v7EbU8tPciHQSUjy7";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
