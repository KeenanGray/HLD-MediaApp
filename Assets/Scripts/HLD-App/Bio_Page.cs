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

    private void Start()
    {
        Initialize();
    }
    // Use this for initialization
    public void Initialize()
    {
        foreach (TextMeshProUGUI tm in GetComponentsInChildren<TextMeshProUGUI>())
        {
            if (tm.name == "Name")
                Name = tm;
            if (tm.name == "Title")
                Title = tm;
            if (tm.name == "Description")
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
//        Title.text = "";
        Description.text = "";
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetName(string str)
    {
        Name.text = "<b>" + str + "</b>";
    }
    public void SetTitle(string str)
    {
        //Title.text = str;
    }
    public void SetDesc(string str)
    {
        if (Description != null)
            Description.text = str;
    }

    public void SetImageFromPath(string PathToImage)
    {
        byte[] fileData = null;

        if (UIB_FileManager.FileExists(PathToImage))
        {
            fileData = UIB_FileManager.ReadFromBytes(PathToImage, UIB_FileTypes.Images);
            if (fileData == null)
            {
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

    public void SetImageFromAssetBundle(string name, string bundleString)
    {
        AssetBundle tmp = null;
        foreach (AssetBundle b in AssetBundle.GetAllLoadedAssetBundles())
        {
            if (b.name == bundleString)
            {
                tmp = b;
            }
        }
        if (BioImage != null && tmp != null)
        {
            Debug.Log(tmp.name);
            Sprite newSprite = tmp.LoadAsset<Sprite>(name);
            if (newSprite != null)
            {
                // Debug.Log("loaded it");
            }
            else
                Debug.Log("did not load " + name);

            BioImage.sprite = tmp.LoadAsset<Sprite>(name);
            //  BioImage.rectTransform.sizeDelta = new Vector2(1000, 1000);
        }
        else
        {
            Debug.Log("bundle not found " + bundleString);
        }
    }
}

