using System.Collections;
using UnityEngine;

namespace PerfectlyParanormal.Bezier
{

    public static class BezierUtils
    {

        public static Vector3 CalculateBezierCurve(float t, Vector3 p1, Vector3 p2, Vector3 c1, Vector3 c2)
        {


            if (p1 == c1 && p2 == c2)
                return CalculateLinearInterpolation(t, p1, p2);


            float u = 1f - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector3 p = uuu * p1; //first term
            p += 3 * uu * t * c1; //second term
            p += 3 * u * tt * c2; //third term
            p += ttt * p2; //fourth term

            return p;
        }

        public static Vector3 CalculateLinearInterpolation(float t, Vector3 p1, Vector3 p2)
        {
            return p1+(p2 - p1)*t;
        }

    }

}