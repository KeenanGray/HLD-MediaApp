using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeApp : MonoBehaviour {
    //The App's pages need to be loaded and initialized in a specific order since they depend on eachother.
    //Outline of this order is as follows:
    //

	// Use this for initialization
	void Start () {
        StartCoroutine("LoadApp");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator LoadApp(){
       // yield return new WaitForSeconds(2.0f);

        foreach (Button_ChangePage bcp in GetComponentsInChildren<Button_ChangePage>())
        {
            bcp.Initialize();
        }

        foreach (Button_OpenSubMenu bom in GetComponentsInChildren<Button_OpenSubMenu>())
        {
            bom.Initialize();
        }

        foreach (SubMenu sm in GetComponentsInChildren<SubMenu>())
        {
            sm.Initialize();
        }

        foreach (Page p in GetComponentsInChildren<Page>())
        {
            p.Initialize();
        }

        yield break;
    }
}
