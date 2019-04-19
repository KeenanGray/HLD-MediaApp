using System;
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
    public static class Epoch
    {

        public static int Current()
        {
            DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            int currentEpochTime = (int)(DateTime.UtcNow - epochStart).TotalSeconds;

            return currentEpochTime;
        }

        public static int SecondsElapsed(int t1)
        {
            int difference = Current() - t1;

            return Mathf.Abs(difference);
        }

        public static int SecondsElapsed(int t1, int t2)
        {
            int difference = t1 - t2;

            return Mathf.Abs(difference);
        }

    }
}
