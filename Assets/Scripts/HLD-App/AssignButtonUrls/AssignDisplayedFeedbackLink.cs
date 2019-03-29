using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI_Builder;

namespace HLD
{
    [RequireComponent(typeof(UIB_Button))]
    public class AssignDisplayedFeedbackLink : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            var b = GetComponent<UIB_Button>();
            b.Button_Opens = UIB_Button.UIB_Button_Activates.InAppUrl;
            b.Title = "D.I.S.P.L.A.Y.E.D Feedback";
            b.s_link = "https://docs.google.com/forms/d/e/1FAIpQLSdQloPNXW4MrAlN2uycWdWSkkBtVFmCpMgTP0ELNGkXr6zuxw/viewform";
        }
    }
}