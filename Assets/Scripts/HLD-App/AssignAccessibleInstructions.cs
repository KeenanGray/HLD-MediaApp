﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI_Builder;

public class AssignAccessibleInstructions : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<UIB_Button>().Button_Opens = UIB_Button.UIB_Button_Activates.Accessibletext;
        GetComponent<UIB_Button>().s_link = "This app provides several accessibility features. You can explore by touch, or swipe left and right to navigate menus. Swiping up and down will" +
            "jump between groups of items. A two finger tap will stop the current voiceover speech. A three finger tap will pause all audio";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
