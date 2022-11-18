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
      
        StartCoroutine ("ReOpenApp");
    }

    IEnumerator ReOpenApp()
    {
          foreach (AssetBundle ab in AssetBundle.GetAllLoadedAssetBundles())
        {
            ab.Unload(true);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene(0);
        yield break;
    }
}
