using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Bio_Page : MonoBehaviour {

    public TextMeshProUGUI Name;
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Description;

    // Use this for initialization
    [ExecuteInEditMode]
    public void Initialize () {
        foreach(TextMeshProUGUI tm in GetComponentsInChildren<TextMeshProUGUI>()){
            if (tm.name == "Name")
                Name = tm;
            else if (tm.name == "Title")
                Title = tm;
            else if (tm.name == "Description")
                Description = tm;
        }

        Name.text = "";
        Title.text = "";
        Description.text = "";
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetName(string str){
        Name.text = str;
    }
    public void SetTitle(string str)
    {
        Title.text = str;
    }
    public void SetDesc(string str)
    {
        Description.text = str;
    }

}
