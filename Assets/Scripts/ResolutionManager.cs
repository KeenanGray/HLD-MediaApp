using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ResolutionManager
{
   public static Resolution GetCurrentResolution()
    {
        return Screen.currentResolution;
    }
}


