using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjPoolManager {

    public enum Pool{
        Bio,
        About,
        Watch,
        Program,
        Button
    }

    static List<GameObject> Biographies_Pool;
    static List<GameObject> Button_Pool;
    static List<GameObject> AboutPage_Pool;

    public static void Init(){
        Biographies_Pool = new List<GameObject>(GameObject.FindGameObjectsWithTag("App_Biography"));
        Button_Pool = new List<GameObject>(GameObject.FindGameObjectsWithTag("App_SubMenuButton"));
        AboutPage_Pool = new List<GameObject>(GameObject.FindGameObjectsWithTag("App_AboutPage"));

        GameObject Bio_PoolGO = GameObject.Find("BioPagePool");
        GameObject Button_PoolGO = GameObject.Find("ButtonPool");
        GameObject About_PoolGO = GameObject.Find("AboutPagePool");

        foreach (GameObject go in Biographies_Pool)
        {
            go.transform.SetParent(Bio_PoolGO.transform);      
        }
        foreach (GameObject go in Button_Pool)
        {
            go.transform.SetParent(Button_PoolGO.transform);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

#if UNITY_EDITOR
            UnityEditor.PrefabUtility.ResetToPrefabState(go);
#endif
        }

        foreach (GameObject go in AboutPage_Pool)
        {
            go.transform.SetParent(About_PoolGO.transform);
        }

    }

    public static GameObject RetrieveFromPool(Pool pool){
        GameObject go = null;
        if(pool.Equals(Pool.Bio)){
            go = Biographies_Pool[0];
            Biographies_Pool.Remove(go);
        }

        if (pool.Equals(Pool.Button)){
            if (Button_Pool.Count > 0)
            {
                go = Button_Pool[0];
                Button_Pool.Remove(go);
            }
            else {
                Debug.LogWarning("No Button Pool");
            }
        }
        return go;
    }

}
