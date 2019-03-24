using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI_Builder;

public class AssignAccessibleInstructions : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<UIB_Button>().Button_Opens = UIB_Button.UIB_Button_Activates.Accessibletext;
        GetComponent<UIB_Button>().s_link = "This app provides several accessibility features. " +
            "You can explore by touch, or swipe left and right to navigate menus. Swiping up and down will" +
            "jump between headings or groups of items. " +
            "A two finger double-tap will stop the current voiceover speech. " +
            "Swipe right with two fingers to go back a page. " +
            "A three finger tap will repeat the highlighted item, without activating it.";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
