using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI_Builder
{
    public static class Utilities
    {
        public static float Map(float from, float fromMin, float fromMax, float toMin, float toMax)
        {
            var fromAbs = from - fromMin;
            var fromMaxAbs = fromMax - fromMin;

            var normal = fromAbs / fromMaxAbs;

            var toMaxAbs = toMax - toMin;
            var toAbs = toMaxAbs * normal;

            var to = toAbs + toMin;

            return to;
        }

        private static void printElapsedTime(int pos, float t1)
        {
            var t2 = Time.time;
            var elapsed = t2 - t1;
            Debug.Log("elapsed:" + elapsed + " at pos " + pos);
        }
    }
}
