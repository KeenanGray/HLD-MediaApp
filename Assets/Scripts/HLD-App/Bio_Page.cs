using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UI_Builder;

public class Bio_Page : MonoBehaviour
{

    public TextMeshProUGUI Name;
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Description;
    public Image BioImage;

    // Use this for initialization
    public void Initialize()
    {
        foreach (TextMeshProUGUI tm in GetComponentsInChildren<TextMeshProUGUI>())
        {
            if (tm.name == "Name")
                Name = tm;
            else if (tm.name == "Title")
                Title = tm;
            else if (tm.name == "Description")
                Description = tm;
        }
        foreach (Image i in GetComponentsInChildren<Image>())
        {
            if (i.name == "Image")
            {
                BioImage = i;
            }
        }

        Name.text = "";
        Title.text = "";
        Description.text = "";
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetName(string str)
    {
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

    public void SetImage(string PathToImage)
    {
        byte[] fileData = null;

        if (FileManager.FileExists(PathToImage, UIB_FileTypes.Images))
        {
            fileData = FileManager.ReadFromBytes(PathToImage, UIB_FileTypes.Images);
            if (fileData == null)
            {
                Debug.Log("HERE");
                return;
            }
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);
            var newSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0), 100.0f);
            if (BioImage != null)
            {
                BioImage.sprite = newSprite;
                BioImage.rectTransform.sizeDelta = new Vector2(1000, 1000);
            }
        }
    }
}
