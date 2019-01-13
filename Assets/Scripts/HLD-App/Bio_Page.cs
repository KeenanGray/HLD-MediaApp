using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Bio_Page : MonoBehaviour {

    public TextMeshProUGUI Name;
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Description;
    public Image BioImage;

    // Use this for initialization
    public void Initialize () {
        foreach(TextMeshProUGUI tm in GetComponentsInChildren<TextMeshProUGUI>()){
            if (tm.name == "Name")
                Name = tm;
            else if (tm.name == "Title")
                Title = tm;
            else if (tm.name == "Description")
                Description = tm;
        }
        foreach(Image i in GetComponentsInChildren<Image>()){
            if (i.name == "Image") {
                BioImage = i;
            }
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

    public void SetImage(string PathToImage){
        var ImageToUse = Resources.Load<Sprite>(PathToImage) as Sprite;
        if (ImageToUse != null)
        {
            //Debug.Log(PathToImage + " : name " + ImageToUse.name);
        }
        else{
            Debug.LogWarning("Failed to load " + PathToImage);
        }
        if (BioImage != null)
        {
            BioImage.sprite = ImageToUse;
            BioImage.rectTransform.sizeDelta = new Vector2(1000, 1000);
        }
    }

}
