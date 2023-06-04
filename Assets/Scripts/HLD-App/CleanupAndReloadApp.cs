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
        Debug.Log("Reload");

        if (UAP_AccessibilityManager.IsActive() && UAP_AccessibilityManager.IsEnabled())
        {
            UAP_AccessibilityManager.Say("Loading, please wait a moment");
            Debug.Log(UAP_AccessibilityManager.IsSpeaking());

            yield return new WaitForSeconds(2.0f);

        }


        // yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene(0);
        yield break;
    }
}
