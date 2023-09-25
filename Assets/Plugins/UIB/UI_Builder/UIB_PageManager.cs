using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI_Builder
{
    public class UIB_PageManager : MonoBehaviour
    {
        public static GameObject LastPage;
        public static GameObject CurrentPage;

        public static bool InternetActive { get; set; }

        // Use this for initialization
        void Start()
        {
            CurrentPage = GameObject.Find("Landing_Page");
            UIB_InputManager.SwipeDelegate += SwipeHandler;
            UIB_Page.pageParent = GameObject.Find("Pages");
        }

        void SwipeHandler(SwipeData swipe)
        {


        }

    }
}