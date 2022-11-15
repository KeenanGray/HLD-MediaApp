file type explanation


1. Unzip Plugins.zip
--------------------------------------
2. Add this function to the UAP_Accessibility Manager

 using System.Linq;

 
 internal static GameObject TrueFirstElement()
    {
        if (instance.m_ActiveContainers.Count >= 0)
        {
            var sorted = instance.m_ActiveContainers.OrderBy(go => go.GetComponent<AccessibleUIGroupRoot>().m_Priority).ToList();
            sorted.Reverse();

            if (sorted.Count > 0)
            {
                if (sorted[0].GetElements().Count > 0)
                    return sorted[0].GetElements().ToArray()[0].m_Object.gameObject;
                else
                    return null;
            }
        }
        return null;
    }

      public static bool GetAndroidAccessibility()
    {
        return IsTalkBackEnabled();
    }


4. 
And add this code to the UAP_AccessibilityManager in the "UpdateCurrentItem" method

                     	try{
						m_CurrentItem.m_Object.GetComponent<UIB_Button>().SetupButtonColors();
						}
						catch
						{}
						try{
						m_PreviousItem.m_Object.GetComponent<UIB_Button>().ResetButtonColors();
						}
						catch{}
						