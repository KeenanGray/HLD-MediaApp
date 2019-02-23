using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI_Builder;

public class SimulateBluetoothButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UIB_Button b = GetComponent<UIB_Button>();
        b.Button_Opens = UIB_Button.UIB_Button_Activates.Scene;
        b.s_link = "BlueToothSim";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
