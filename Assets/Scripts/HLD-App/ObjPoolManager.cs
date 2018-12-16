using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjPoolManager
{

    public enum Pool
    {
        Bio,
        About,
        Watch,
        Program,
        Button
    }

    static List<GameObject> Biographies_Pool;
    static List<GameObject> Button_Pool;
    static List<GameObject> AboutPage_Pool;

    public static void Init()
    {
        Biographies_Pool = new List<GameObject>(GameObject.FindGameObjectsWithTag("App_Biography"));
        Button_Pool = new List<GameObject>(GameObject.FindGameObjectsWithTag("App_SubMenuButton"));
        AboutPage_Pool = new List<GameObject>(GameObject.FindGameObjectsWithTag("App_AboutPage"));

        GameObject Bio_PoolGO = GameObject.Find("BioPagePool");
        GameObject Button_PoolGO = GameObject.Find("ButtonPool");
        GameObject About_PoolGO = GameObject.Find("AboutPagePool");

        foreach (GameObject go in Biographies_Pool)
        {
            go.transform.SetParent(Bio_PoolGO.transform);
            go.name = "Bio_Page";
        }
        foreach (GameObject go in Button_Pool)
        {
            go.transform.SetParent(Button_PoolGO.transform);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            go.GetComponent<UI_Builder.UIB_Button>().SetButtonText("");
            go.name = "App_SubMenuButton";

            go.GetComponent<UI_Builder.UIB_Button>().Init();
        }

        foreach (GameObject go in AboutPage_Pool)
        {
            go.transform.SetParent(About_PoolGO.transform);
        }

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
    }

    public static void DisablePools()
    {
        GameObject Bio_PoolGO = GameObject.Find("BioPagePool");
        GameObject Button_PoolGO = GameObject.Find("ButtonPool");
        GameObject About_PoolGO = GameObject.Find("AboutPagePool");

        Bio_PoolGO.SetActive(false);
        Button_PoolGO.SetActive(false);
        About_PoolGO.SetActive(false);
    }
}
