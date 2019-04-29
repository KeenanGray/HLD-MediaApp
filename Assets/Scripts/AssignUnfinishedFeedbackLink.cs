using System.Collections;
using System.Collections.Generic;
using UI_Builder;
using UnityEngine;

public class AssignUnfinishedFeedbackLink : MonoBehaviour
{
    // Start is called before the first frame update
    // Start is called before the first frame update
    void Start()
    {
        var b = GetComponent<UIB_Button>();
        b.Button_Opens = UIB_Button.UIB_Button_Activates.InAppUrl;
        b.Title = "\"Unfinished\" Feedback";
        b.s_link = "https://forms.gle/AGpPbe3AVzUhSgsq9";
    }

}
