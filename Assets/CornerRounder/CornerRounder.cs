using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static AillieoUtils.CornerRounderConfig;

namespace AillieoUtils
{
    public static class CornerRounder
    {
        public static void RoundCorner(IList<Vector3> source, IList<Vector3> target, CornerRounderConfig config)
        {
            CornerRounderContext context = new CornerRounderContext(config, source);

            RemoveAcuteAngle(context);

            MergeClosePoints(source,context);

            AnglesToArcs(context);

            target.Clear();
            ArcsToPoints(context, target);
        }

        #region remove acute

        private static void RemoveAcuteAngle(CornerRounderContext context)
        {
            CornerRounderConfig config = context.config;
            if (config.acuteAngleStrategy == AcuteAngleStrategy.Keep)
            {
                return;
            }

            LinkedList<CornerInfo> cornerInfo = context.cornerInfo;
            LinkedListNode<CornerInfo> current = cornerInfo.First;

            float acuteAngle = Mathf.Deg2Rad * config.acuteAngleThreshold;
            // 填充corner 同时填充angle
            while (current != null)
            {
                if (current.Previous == null || current.Next == null)
                {
                    current = current.Next;
                    continue;
                }

                CornerInfo corner = current.Value;
                corner.angle = MathUtils.Angle(
                    current.Previous.Value.vertex,
                    current.Value.vertex,
                    current.Next.Value.vertex);
                current.Value = corner;

                if (corner.angle <= acuteAngle)
                {
                    var toRemove = current;
                    current = current.Previous;
                    cornerInfo.Remove(toRemove);
                    continue;
                }

                current = current.Next;
            }
        }

        #endregion remove acute

        #region merge close points

        private static void MergeClosePoints(IList<Vector3> source, CornerRounderContext context)
        {
            CornerRounderConfig config = context.config;
            PointMergeStrategy strategy = config.closePointMergeStrategy;
            float threshold = config.mergeThreshold;
            LinkedList<CornerInfo> cornerInfo = context.cornerInfo;

            if(source.Count <= 2 || strategy == PointMergeStrategy.Never)
            {
                return;
            }

            float thresholdSqr = threshold * threshold;

            switch (strategy)
            {
                case PointMergeStrategy.Forward:
                    MergeForward(cornerInfo, thresholdSqr);
                    break;
                case PointMergeStrategy.MiddlePoint:
                    MergeMiddlePoint(cornerInfo, thresholdSqr);
                    break;
                case PointMergeStrategy.Backward:
                    MergeBackward(cornerInfo, thresholdSqr);
                    break;
                case PointMergeStrategy.LeastMove:
                    MergeLeastMove(cornerInfo, thresholdSqr);
                    break;
            }
        }

        private static void MergeForward(LinkedList<CornerInfo> cornerInfo, float thresholdSqr)
        {
            LinkedListNode<CornerInfo> current = cornerInfo.First;
            while(current != null)
            {
                if(current.Previous == null || current.Next == null)
                {
                    // 第一个或最后一个
                    current = current.Next;
                    continue;
                }
                if(MathUtils.CloseEnough(current.Value.vertex, current.Next.Value.vertex, thresholdSqr))
                {
                    var toRemove = current;
                    current = current.Previous;
                    cornerInfo.Remove(toRemove);
                    continue;
                }

                current = current.Next;
            }
            FixFirstPoint(cornerInfo, thresholdSqr);
        }

        private static void MergeBackward(LinkedList<CornerInfo> cornerInfo, float thresholdSqr)
        {
            LinkedListNode<CornerInfo> current = cornerInfo.First;

            while (current != null)
            {
                if (current.Previous == null || current.Next == null)
                {
                    // 第一个或最后一个
                    current = current.Next;
                    continue;
                }

                if (MathUtils.CloseEnough(current.Value.vertex, current.Previous.Value.vertex, thresholdSqr))
                {
                    var toRemove = current;
                    current = current.Previous;
                    cornerInfo.Remove(toRemove);
                    continue;
                }

                current = current.Next;
            }

            FixLastPoint(cornerInfo, thresholdSqr);

        }

        private static void MergeMiddlePoint(LinkedList<CornerInfo> cornerInfo, float thresholdSqr)
        {
            LinkedListNode<CornerInfo> current = cornerInfo.First;
            while (current != null)
            {
                if (current.Previous == null || current.Previous.Previous == null || current.Next == null)
                {
                    // 第一个 第二个 或最后一个
                    current = current.Next;
                    continue;
                }

                if (MathUtils.CloseEnough(current.Value.vertex, current.Previous.Value.vertex, thresholdSqr))
                {
                    var toRemove = current;

                    Vector3 middlePoint = (current.Value.vertex + current.Previous.Value.vertex) * 0.5f;
                    current.Previous.Value = new CornerInfo(middlePoint);

                    current = current.Previous;
                    cornerInfo.Remove(toRemove);
                    continue;
                }

                current = current.Next;
            }

            FixFirstPoint(cornerInfo, thresholdSqr);
            FixLastPoint(cornerInfo, thresholdSqr);
        }
        private static void MergeLeastMove(LinkedList<CornerInfo> cornerInfo, float thresholdSqr)
        {
            // 1. 短线段 两端连接的长线段 投影到中间平面上
            // 2. 长线段 延长相交到一点
            // 3. 性能较大 视觉效果最佳
            LinkedListNode<CornerInfo> current = cornerInfo.First;
            while (current != null)
            {
                if (current.Previous == null || current.Previous.Previous == null || current.Next == null)
                {
                    // 第一个 第二个 或最后一个
                    current = current.Next;
                    continue;
                }

                if (MathUtils.CloseEnough(current.Value.vertex, current.Previous.Value.vertex, thresholdSqr))
                {
                    var toRemove = current;

                    // 完成这一操作 需要4个点
                    // 依次 p0 p1 p2 p3 目标是把 p1和p2 merge到一起
                    if (current.Next.Next != null)
                    {
                        Vector3 mergedPoint = CalculatePointLeastMove(
                        current.Previous.Previous.Value.vertex,
                        current.Previous.Value.vertex,
                        current.Value.vertex,
                        current.Next.Value.vertex);
                        current.Previous.Value = new CornerInfo(mergedPoint);
                    }

                    current = current.Previous;
                    cornerInfo.Remove(toRemove);
                    continue;
                }

                current = current.Next;
            }

            FixFirstPoint(cornerInfo, thresholdSqr);
            FixLastPoint(cornerInfo, thresholdSqr);
        }

        private static void FixFirstPoint(LinkedList<CornerInfo> cornerInfo, float thresholdSqr)
        {
            LinkedListNode<CornerInfo> first = cornerInfo.First;
            LinkedListNode<CornerInfo> second = first.Next;

            while (true)
            {
                if (second == null)
                {
                    break;
                }
                if (MathUtils.CloseEnough(first.Value.vertex, second.Value.vertex, thresholdSqr))
                {
                    cornerInfo.Remove(second);
                    second = first.Next;
                }
                else
                {
                    break;
                }
            }
        }

        private static void FixLastPoint(LinkedList<CornerInfo> cornerInfo, float thresholdSqr)
        {
            LinkedListNode<CornerInfo> last = cornerInfo.Last;
            LinkedListNode<CornerInfo> lastButOne = last.Previous;

            while (true)
            {
                if (lastButOne == null)
                {
                    break;
                }
                if (MathUtils.CloseEnough(last.Value.vertex, lastButOne.Value.vertex, thresholdSqr))
                {
                    cornerInfo.Remove(lastButOne);
                    lastButOne = last.Previous;
                }
                else
                {
                    break;
                }
            }
        }

        private static Vector3 CalculatePointLeastMove(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            // 折线为 p0-p1 - p2-p3
            // 保留p0和p3两个点 删除p1和p2
            // 如果两条线平行 直接取p1和p2中点 否则取公垂线的中点

            Vector3 p4 = Vector3.zero;
            Vector3 p5 = Vector3.zero;
            if (MathUtils.CalculatePerpendicular(p0, p1, p2, p3, out p4, out p5))
            {
                // 不平行
                return (p4 + p5) * 0.5f;
            }
            else
            {
                // 平行
                return (p1 + p2) * 0.5f;
            }
        }

        #endregion merge close points

        #region angle to arc

        private static void AnglesToArcs(CornerRounderContext context)
        {
            PrepareRadiusInfo(context);

            CornerRounderConfig config = context.config;

            RadiusStrategy strategy = config.radiusStrategy;

            if (strategy == RadiusStrategy.Never)
            {
                return;
            }

            LinkedList<CornerInfo> cornerInfo = context.cornerInfo;

            LinkedListNode<CornerInfo> current = cornerInfo.First;

            while (current != null)
            {
                if (current.Previous != null && current.Next != null)
                {
                    CornerInfo c0 = current.Previous.Value;
                    CornerInfo c1 = current.Value;
                    CornerInfo c2 = current.Next.Value;

                    Vector3 p0 = c0.vertex;
                    Vector3 p1 = c1.vertex;
                    Vector3 p2 = c2.vertex;

                    float radius = c1.radiusAdjusted;

                    if (radius != 0)
                    {
                        float halfAngle = c1.angle * 0.5f;
                        float tan = Mathf.Tan(halfAngle);
                        float cutLen = radius / tan;

                        // 截断点
                        Vector3 cut1 = p1 + (p0 - p1).normalized * cutLen;
                        Vector3 cut2 = p1 + (p2 - p1).normalized * cutLen;

                        // 顶点到圆心距离 
                        float vertexToCenter = radius / Mathf.Sin(halfAngle);

                        // 圆心
                        Vector3 center = ((cut1 - p1) + (cut2 - p1)).normalized * vertexToCenter + p1;
                        CornerInfo value = current.Value;
                        CornerInfo.Round(ref value, center, cut1, cut2);
                        current.Value = value;
                    }
                }

                current = current.Next;
            }

        }

        private static void PrepareRadiusInfo(CornerRounderContext context)
        {
            CornerRounderConfig config = context.config;

            RadiusStrategy strategy = config.radiusStrategy;

            if (strategy == RadiusStrategy.Never)
            {
                return;
            }

            LinkedList<CornerInfo> cornerInfo = context.cornerInfo;

            LinkedListNode<CornerInfo> current = cornerInfo.First;

            while (current != null)
            {
                if (current.Previous != null && current.Next != null)
                {
                    CornerInfo info = current.Value;
                    float angle = MathUtils.Angle(current.Previous.Value, current.Value, current.Next.Value);
                    info.angle = angle;
                    switch (strategy)
                    {
                    case RadiusStrategy.Unified:
                        info.radiusExpected = config.radiusTarget;
                        break;
                    case RadiusStrategy.Adaptive:
                        info.radiusExpected = Mathf.Lerp(config.radiusMin, config.radiusMax, angle / 3.14f);
                        break;
                    }

                    current.Value = info;
                }

                current = current.Next;
            }

            // 重置 再循环一遍
            current = cornerInfo.First;
            while (current != null)
            {
                if (current.Previous != null && current.Next != null)
                {
                    CornerInfo c0 = current.Previous.Value;
                    CornerInfo c1 = current.Value;
                    CornerInfo c2 = current.Next.Value;
                    bool previousIsDirty = RadiusAdjust(ref c0, ref c1, ref c2);
                    current.Previous.Value = c0;
                    current.Value = c1;
                    current.Next.Value = c2;

                    if(previousIsDirty)
                    {
                        current = current.Previous;
                    }
                }

                current = current.Next;
            }

        }

        private static bool RadiusAdjust(ref CornerInfo c0, ref CornerInfo c1, ref CornerInfo c2)
        {
            float radiusExpected = c1.radiusExpected;

            CornerInfo.EnsureNeighbourInfo(ref c0, ref c1);
            CornerInfo.EnsureNeighbourInfo(ref c1, ref c2);

            float angle = c1.angle;
            float tanHalf = Mathf.Tan(angle * 0.5f);
            float radiusAdjusted = Mathf.Min(
                radiusExpected,
                c1.toPrevious.distance * 0.5f * tanHalf,
                c1.toNext.distance * 0.5f * tanHalf);
            c1.radiusAdjusted = Mathf.Max(radiusAdjusted, 0);

            /*

            bool isEnough0 = c1.previous.distance > c0.radiusExpected + c1.radiusExpected;
            bool isEnough2 = c1.next.distance > c2.radiusExpected + c1.radiusExpected;


            // 两边的距离都充足
            if (isEnough0 && isEnough2)
            {
                c1.radiusAdjusted = c1.radiusExpected;
                return false;
            }

            float radiusCorner0 = (c0.radiusAdjusted == CornerInfo.Uninitiated) ? 0 : c0.radiusAdjusted;
            float radiusCorner2 = (c2.radiusAdjusted == CornerInfo.Uninitiated) ? 0 : c2.radiusAdjusted;

            float radiusP = Mathf.Clamp(radiusExpected, 0, c1.previous.distance - radiusCorner0);
            float radiusN = Mathf.Clamp(radiusExpected, 0, c1.next.distance - radiusCorner2);

            float radiusAdjusted = Mathf.Min(radiusP, radiusN);
            c1.radiusAdjusted = radiusAdjusted;
            c0.radiusAdjusted = (c0.radiusAdjusted == CornerInfo.Uninitiated) ? radiusAdjusted : c0.radiusAdjusted;
            c2.radiusAdjusted = (c2.radiusAdjusted == CornerInfo.Uninitiated) ? radiusAdjusted : c2.radiusAdjusted;

            */

            return false;
        }

        #endregion angle to arc

        #region arc to points

        private static void ArcsToPoints(CornerRounderContext context, IList<Vector3> target)
        {
            CornerRounderConfig config = context.config;
            ResolutionStrategy strategy = config.cornerResolutionStrategy;

            LinkedList<CornerInfo> cornerInfo = context.cornerInfo;

            float stepFactor = 1.0f / config.cornerResolution;

            target.Clear();
            foreach (var corner in cornerInfo)
            {
                if(!corner.isRounded)
                {
                    target.Add(corner.vertex);
                }
                else
                {
                    switch (strategy)
                    {
                    case ResolutionStrategy.Directly:
                        target.Add(corner.arcPoint1);
                        target.Add(corner.arcPoint2);
                        break;
                    case ResolutionStrategy.ByAngle:
                        MathUtils.ArcToPoints(corner, stepFactor, target);
                        break;
                    case ResolutionStrategy.ByArc:
                        MathUtils.ArcToPoints(corner, stepFactor * (corner.arcCenter - corner.arcPoint1).magnitude, target);
                        break;
                    }
                }
            }

        }

        #endregion arc to points


    }

}
