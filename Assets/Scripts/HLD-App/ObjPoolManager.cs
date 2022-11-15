using System.Collections;
using System.Collections.Generic;
using UI_Builder;
using UnityEngine;
using UnityEngine.UI;

public static class ObjPoolManager
{

    public enum Pool
    {
        Bio,
        Narrative,
        About,
        Watch,
        Program,
        Button
    }

    static GameObject ObjectPoolCanvas;
    static List<GameObject> Biographies_Pool;
    static List<GameObject> Button_Pool;
    static List<GameObject> AboutPage_Pool;
    static List<GameObject> Narrative_Pool;

    public static void Init()
    {
        ObjectPoolCanvas = GameObject.Find("ObjectPoolCanvas");

        if (ObjectPoolCanvas == null)
        {
            Debug.LogWarning("Object pool is null");
        }

        Biographies_Pool = new List<GameObject>(GameObject.FindGameObjectsWithTag("App_Biography"));
        Button_Pool = new List<GameObject>(GameObject.FindGameObjectsWithTag("App_SubMenuButton"));
        AboutPage_Pool = new List<GameObject>(GameObject.FindGameObjectsWithTag("App_AboutPage"));
        Narrative_Pool = new List<GameObject>(GameObject.FindGameObjectsWithTag("App_Narrative"));

    }

    public static void RefreshPool()
    {
        if (ObjectPoolCanvas == null)
        {
            Debug.LogWarning("No object pool canvas found");
            return;
        }

        //ObjectPoolCanvas.SetActive(true);

        Biographies_Pool = new List<GameObject>(GameObject.FindGameObjectsWithTag("App_Biography"));
        Button_Pool = new List<GameObject>(GameObject.FindGameObjectsWithTag("App_SubMenuButton"));
        AboutPage_Pool = new List<GameObject>(GameObject.FindGameObjectsWithTag("App_AboutPage"));
        Narrative_Pool = new List<GameObject>(GameObject.FindGameObjectsWithTag("App_Narrative"));

        GameObject Bio_PoolGO = GameObject.Find("BioPagePool");
        GameObject Button_PoolGO = GameObject.Find("ButtonPool");
        GameObject About_PoolGO = GameObject.Find("AboutPagePool");
        GameObject Narrative_PoolGO = GameObject.Find("NarrativePagePool");

        foreach (GameObject go in Biographies_Pool)
        {
            go.transform.SetParent(Bio_PoolGO.transform);
            go.GetComponent<UIB_Page>().StartCoroutine(go.GetComponent<UIB_Page>().ResetUAP(false));

            go.name = "Bio_Page";
            go.GetComponent<UIB_Page>().Init();
        }
        foreach (GameObject go in Button_Pool)
        {
            go.transform.SetParent(Button_PoolGO.transform);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            go.GetComponent<UI_Builder.UIB_Button>().SetButtonText("");
            go.GetComponent<Button>().onClick.RemoveAllListeners();

            go.GetComponent<UAP_BaseElement>().enabled = false;
            go.name = "App_SubMenuButton+edited";

            go.GetComponent<UI_Builder.UIB_Button>().Init();
        }

        foreach (GameObject go in AboutPage_Pool)
        {
            go.transform.SetParent(About_PoolGO.transform);
        }

        foreach (GameObject go in Narrative_Pool)
        {
            go.transform.SetParent(Narrative_PoolGO.transform);
        }

        //ObjectPoolCanvas.SetActive(false);
    }

    public static void RetrieveFromPool(Pool pool, ref GameObject returned)
    {

        if (pool.Equals(Pool.Bio))
        {
            if (Biographies_Pool.Count > 0)
            {
                returned = Biographies_Pool[0];
                Biographies_Pool.Remove(returned);
            }
            else
            {
                Debug.LogWarning("No Bio Pool");
            }

        }

        if (pool.Equals(Pool.Button))
        {
            if (Button_Pool.Count > 0)
            {
                returned = Button_Pool[0];
                Button_Pool.Remove(returned);
            }
            else
            {
                Debug.LogWarning("No Button Pool");
            }
        }

        if (pool.Equals(Pool.Narrative))
        {
            if (Narrative_Pool.Count > 0)
            {
                returned = Narrative_Pool[0];
                Narrative_Pool.Remove(returned);
            }
            else
            {
                Debug.LogWarning("No Narrative Pool");
            }
        }
    }


}
