using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CleanupAndReloadApp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StopAllCoroutines();
        foreach (AssetBundle ab in AssetBundle.GetAllLoadedAssetBundles())
        {
            ab.Unload(true);
        }
        StartCoroutine ("ReOpenApp");
    }

    IEnumerator ReOpenApp()
    {
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene(0);
        yield break;
    }
}
