using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI_Builder;
using UnityEngine;
using UnityEngine.UI;

public class InternetFileErrorPage : MonoBehaviour, UIB_IPage
{
    public void Init()
    {
        GetComponent<UIB_Page>().OnActivated += PageActivatedHandler;
        GetComponent<UIB_Page>().OnDeActivated += PageDeActivatedHandler;
    }

    public void PageActivatedHandler()
    {
        foreach(TextMeshProUGUI tmp in GetComponentsInChildren<TextMeshProUGUI>())
        {
            transform.SetAsLastSibling();
            if (tmp.text == "Continue")
            {
                tmp.gameObject.transform.parent.name = UIB_PageManager.LastPage.name.Replace("_Page", "_Button");
                tmp.gameObject.transform.parent.GetComponent<UIB_Button>().Init();
                tmp.gameObject.transform.parent.GetComponent<Button>().enabled = true;
            }
        }
        GetComponent<UIB_Page>().ActivateButtonsOnScreen();
        GetComponent<UIB_Page>().ActivateUAP();

    }

    public void PageDeActivatedHandler()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
