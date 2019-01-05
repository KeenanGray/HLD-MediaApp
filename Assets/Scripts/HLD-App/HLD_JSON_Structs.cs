using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class HLD_JSON_Structs : MonoBehaviour {

    [System.Serializable]
    public class BioArray
    {
        public Biography[] data;
    }

    [System.Serializable]
    public class Biography{
        public string _id;
        public string Name;
        public string Title;
        public string Bio;
    }

}
