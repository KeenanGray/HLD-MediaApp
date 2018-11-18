using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MeOnDisplayPage : MonoBehaviour {


    List<string> Dancers;
    ScrollRect scroll;

	// Use this for initialization
	void Start () {
        Dancers = new List<string>(){"Desmond Cadogan", "Chris Braz", "Peter Trojic", "Victoria Dombroski", "Tianshi Suo", "Tiffany Geigel", "Donald Lee",
                                      "Louisa Mann", "Leslie Taub", "Jerron Herman", "Kelly Ramis", "Nico Gonzales", "Meredith Fages", "Amy Meisner", "Jillian Hollis",
                                      "Jaclyn Rea", "Carmen Schoenster"};

        scroll = GetComponentInChildren<ScrollRect>();

       GetComponent<Page>().OnActivated += PageActivated;
        GetComponent<Page>().OnDeActivated += PageDeActivated;

    }

    private void PageActivated()
    {
        //sort alphabetically
        var OrderedByName = Dancers.OrderBy(x => x);

        foreach (string dancer in OrderedByName){
            var b = ObjPoolManager.RetrieveFromPool(ObjPoolManager.Pool.Button);
            if (b != null)
            {
                var ab = b.GetComponent<App_Button>();
                ab.SetButtonText(dancer);
                ab.Button_Opens = App_Button.Button_Activates.Video;
                b.name = dancer + " video";
                ab.Init();

                b.GetComponent<Button>().onClick.AddListener(PlayVideo);

                b.transform.SetParent(scroll.content.transform);

            }
            else
                Debug.LogError("Not enough objects in pool");
        }
    }
    private void PageDeActivated()
    {
        //Return the objecst to the pool
        ObjPoolManager.Init();
    }

    void PlayVideo()
    {
        GameObject.FindWithTag("App_VideoPlayer").GetComponent<VideoPlayer>().Play();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
