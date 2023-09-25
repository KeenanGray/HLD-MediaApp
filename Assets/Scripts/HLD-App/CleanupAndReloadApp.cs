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

        StartCoroutine("ReOpenApp");
    }

    IEnumerator ReOpenApp()
    {
        if (UAP_AccessibilityManager.IsActive() && UAP_AccessibilityManager.IsEnabled())
        {
            UAP_AccessibilityManager.Say("Loading, please wait a moment");
            yield return new WaitForSeconds(2.0f);
        }
        foreach (AssetBundle ab in AssetBundle.GetAllLoadedAssetBundles())
        {
            ab.Unload(true);
        }

        // yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene(0);
        yield break;
    }
}
