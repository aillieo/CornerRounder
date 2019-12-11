using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AillieoUtils
{ 
    public static class MathUtils
    {
        public static bool CloseEnough(Vector3 point1, Vector3 point2, float thresholdSqr)
        {
            return Vector3.SqrMagnitude(point1 - point2) < thresholdSqr;
        }

        // value in radians
        public static float Angle(Vector3 point1, Vector3 point2, Vector3 point3)
        {
            return Vector3.Angle(point1 - point2, point3 - point2) * Mathf.Deg2Rad;
        }

        public static bool CalculatePerpendicular(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, out Vector3 p4, out Vector3 p5)
        {
            Vector3 v01 = p1 - p0;
            Vector3 v32 = p3 - p2;

            v01.Normalize();
            v32.Normalize();

            Vector3 perpend = Vector3.Cross(v01, v32);
            float sqrLen = perpend.sqrMagnitude;

            if (sqrLen == 0)
            {
                // 平行
                p4 = p5 = Vector3.zero;
                return false;
            }

            Vector3 v12 = p2 - p1;

            // 公垂线的两个端点 p4 p5
            float t1 = Vector3.Dot(Vector3.Cross(v12, v32), perpend) / sqrLen;
            float t2 = Vector3.Dot(Vector3.Cross(v12, v01), perpend) / sqrLen;

            p4 = p1 + t1 * v01;
            p5 = p2 + t2 * v32;
            return true;
        }

        public static Vector3 Project(Vector3 point, Vector3 linePoint1, Vector3 linePoint2)
        {
            return linePoint1 + Vector3.Project(point - linePoint1, linePoint2 - linePoint1);
        }

        public static void ArcToPoints(CornerInfo corner, float step, IList<Vector3> points)
        {
            Vector3 cut1 = corner.arcPoint1;
            Vector3 cut2 = corner.arcPoint2;
            Vector3 center = corner.arcCenter;

            Vector3 vec1 = cut1 - center;
            Vector3 vec2 = cut2 - center;

            step = Mathf.Clamp(step, 0.001f, 1);

            float t = 0;
            while(true)
            {
                points.Add(Vector3.Slerp(vec1, vec2, t) + center);
                t += step;
                if (t > 1)
                {
                    points.Add(Vector3.Slerp(vec1, vec2, t) + center);
                    break;
                }
            }
        }
    }
}

