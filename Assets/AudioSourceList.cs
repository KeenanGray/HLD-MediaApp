using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceList : MonoBehaviour
{
    List<BluetoothAudioSource> initialOrder;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ChildSelected()
    {
        //sort the children. 


        foreach (BluetoothAudioSource blas in GetComponentsInChildren<BluetoothAudioSource>())
        {
            blas.transform.SetSiblingIndex(blas.childIndex);
            //order by selected
            if (blas.Selected)
            {
                //blas.transform.SetSiblingIndex(0);
            }
        }
    }
}
