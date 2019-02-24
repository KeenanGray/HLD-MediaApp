using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using UI_Builder;

namespace HLD
{
    public class JSON_Structs : UIB_Struct
    {
        [System.Serializable]
        public class BiographyArray : UIB_Struct
        {
            public Biography[] data;
        }

        [System.Serializable]
        public class Biography : UIB_Struct
        {
            public string _id;
            public string Name;
            public string Title;
            public string Bio;
        }

    }

}