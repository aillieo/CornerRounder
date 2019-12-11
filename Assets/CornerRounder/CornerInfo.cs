using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AillieoUtils
{
    public struct CornerInfo
    {
        public static readonly float Uninitiated = -1f;

        public struct NeighbourInfo
        {
            public bool dirty;
            // 由自身指向相邻点的方向
            public Vector3 direction;
            public float distance;
        }

        public bool isRounded;

        // 邻居信息
        public NeighbourInfo toPrevious;
        public NeighbourInfo toNext;

        // 角的信息
        public Vector3 vertex;
        public float angle;

        // 倒圆过程参数
        public float radiusExpected;
        public float radiusAdjusted;

        // 倒圆后的信息
        public Vector3 arcCenter;
        public Vector3 arcPoint1;
        public Vector3 arcPoint2;

        public CornerInfo(Vector3 point)
        {
            isRounded = false;
            vertex = point;

            angle = Uninitiated;
            toPrevious = new NeighbourInfo() { dirty = true };
            toNext = new NeighbourInfo() { dirty = true };
            arcCenter = arcPoint1 = arcPoint2 = Vector3.zero;
            radiusExpected = radiusAdjusted = Uninitiated;
        }

        // 规范地讲 round时其实只需要一个半径 
        public static void Round(ref CornerInfo cornerInfo, Vector3 center, Vector3 arcPoint1, Vector3 arcPoint2)
        {
            cornerInfo.isRounded = true;
            cornerInfo.arcCenter = center;
            cornerInfo.arcPoint1 = arcPoint1;
            cornerInfo.arcPoint2 = arcPoint2;
        }

        public static void EnsureNeighbourInfo(ref CornerInfo first, ref CornerInfo second)
        {
            //if (first.toNext.dirty || second..toPrevious.dirty)
            {
                float dist = Vector3.Distance(first.vertex, second.vertex);
                first.toNext.distance = second.toPrevious.distance = dist;
                first.toNext.direction = second.vertex - first.vertex;
                second.toPrevious.direction = first.vertex - second.vertex;
                first.toNext.dirty = false;
                second.toPrevious.dirty = false;
            }
        }

        public static void Round(ref CornerInfo cornerInfo, float radius)
        {
            float angle = cornerInfo.angle;
            float halfAngle = angle * 0.5f;
            float tan = Mathf.Tan(halfAngle);
            float cutLen = radius / tan;

            /*

            // 截断点
            Vector3 cut1 = p1 + (p0 - p1).normalized * cutLen;
            Vector3 cut2 = p1 + (p2 - p1).normalized * cutLen;

            // 顶点到圆心距离 
            float vertexToCenter = radius / Mathf.Sin(halfAngle);

            // 圆心
            Vector3 center = ((cut1 - p1) + (cut2 - p1)).normalized * vertexToCenter + p1;
            CornerInfo value = current.Value;
            CornerInfo.Round(ref value, center, cut1, cut2);

             */
        }

        public static implicit operator Vector3(CornerInfo cornerInfo)
        {
            return cornerInfo.vertex;
        }

    }
}

