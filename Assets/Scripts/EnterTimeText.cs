using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnterTimeText : MonoBehaviour, ISelectHandler
{
    UIB_AudioPlayerTools tools;

    public void OnSelect(BaseEventData eventData)
    {
        tools.OnSelect(eventData);
    }

    // Start is called before the first frame update
    void Start()
    {
        tools = GameObject.Find("AudioSourceAndTools").GetComponent<UIB_AudioPlayerTools>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
