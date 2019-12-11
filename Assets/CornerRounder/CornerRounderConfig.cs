using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AillieoUtils
{
    [CreateAssetMenu(fileName = "@CornerRounderConfig.asset", menuName = "AillieoUtils/CornerRounderConfig")]
    public class CornerRounderConfig : ScriptableObject
    {
        public enum AcuteAngleStrategy
        {
            Keep = 0,
            Remove = 1,
        }

        public enum PointMergeStrategy
        {
            Never = 0,
            Forward = 1,
            MiddlePoint = 2,
            Backward = 3,
            LeastMove = 4,
        }

        public enum RadiusStrategy
        {
            Never = 0,
            Unified = 1,
            Adaptive = 2,
        }

        public enum ResolutionStrategy
        {
            Directly = 0,
            ByAngle = 1,
            ByArc = 2,
        }

        // todo
        // will support later
        public bool isLoop;

        public AcuteAngleStrategy acuteAngleStrategy;
        public float acuteAngleThreshold;

        public PointMergeStrategy closePointMergeStrategy;
        public float mergeThreshold;

        public RadiusStrategy radiusStrategy;
        public float radiusTarget;
        public float radiusMax;
        public float radiusMin;

        public ResolutionStrategy cornerResolutionStrategy;
        public float cornerResolution;

        public static bool Validate(CornerRounderConfig config)
        {
            if(config == null)
            {
                return false;
            }
            return true;
        }
    }
}
