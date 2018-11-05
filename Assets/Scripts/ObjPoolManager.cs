using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjPoolManager {

    public enum Pool{
        Bio,
        Button
    }

    static List<GameObject> Biographies_Pool;
    static List<GameObject> Button_Pool;

    [ExecuteInEditMode]
    public static void Init(){
            Biographies_Pool = new List<GameObject>(GameObject.FindGameObjectsWithTag("App_Biography"));
            Button_Pool = new List<GameObject>(GameObject.FindGameObjectsWithTag("App_SubMenuButton"));

            GameObject Bio_PoolGO = GameObject.Find("BioPagePool");
            GameObject Button_PoolGO = GameObject.Find("ButtonPool");

            foreach (GameObject go in Biographies_Pool)
                go.transform.SetParent(Bio_PoolGO.transform);
            foreach (GameObject go in Button_Pool)
                go.transform.SetParent(Button_PoolGO.transform);

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
