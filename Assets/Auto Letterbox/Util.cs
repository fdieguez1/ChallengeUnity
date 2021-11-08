using UnityEngine;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using System;

namespace AutoLetterbox {

    /* Util.cs
     * 
     * Utility Class with multiple use mathmatical functions
     * 
     * Written by Tom Elliott and Milo Keeble */

    public static class Util {
        /// <summary>
        /// Bezier Interpolation between multiple points
        /// </summary>
        public static float BezierCurve(float[] p, float t) {
            if (p.Length > 2) {
                float[] newPoints = new float[p.Length - 1];
                for (int i = 0; i < newPoints.Length; i++) {
                    newPoints[i] = Lerp(p[i], p[i + 1], t);
                }
                return BezierCurve(newPoints, t);

            } else if (p.Length == 2) {
                return Lerp(p[0], p[1], t);

            } else {
                Debug.Log("WARNING: A class attempted to get a Bezier Curve with less than two points!");
                return 0;
            }
        }

        /// <summary>
        /// Bezier Interpolation between multiple points
        /// </summary>
        public static Vector3 BezierCurve(Vector3[] p, float t) {
            if (p.Length > 2) {
                Vector3[] newPoints = new Vector3[p.Length - 1];
                for (int i = 0; i < newPoints.Length; i++) {
                    newPoints[i] = Lerp(p[i], p[i + 1], t);
                }
                return BezierCurve(newPoints, t);

            } else if (p.Length == 2) {
                return Lerp(p[0], p[1], t);

            } else {
                //Debug.Log("WARNING: A class attempted to get a Bezier Curve with less than two points!");
                return Vector3.zero;
            }
        }
        /// <summary>
        /// Linear Interpolation between two points
        /// </summary>
        public static Vector3 Lerp(Vector3 p1, Vector3 p2, float t) {
            return p1 + (p2 - p1) * t;
        }

        /// <summary>
        /// Linear Interpolation between two points
        /// </summary>
        public static float Lerp(float p1, float p2, float t) {
            return p1 + (p2 - p1) * t;
        }
    }
}