using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI_Builder;

namespace HLD
{
    [RequireComponent(typeof(UIB_Button))]
    public class AssignInstaLink : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            var b = GetComponent<UIB_Button>();
            b.Button_Opens = UIB_Button.UIB_Button_Activates.Website;
            b.s_link = "https://www.instagram.com/heidilatskydance/";
        }
    }
}