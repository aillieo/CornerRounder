using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AillieoUtils
{
    public class CornerRounderContext
    {
        public CornerRounderContext(CornerRounderConfig config, IList<Vector3> source)
        {
            this.config = config;
            this.cornerInfo = new LinkedList<CornerInfo>();
            foreach (var point in source)
            {
                cornerInfo.AddLast(new CornerInfo(point));
            }
        }

        public CornerRounderConfig config { get; private set; }

        public LinkedList<CornerInfo> cornerInfo { get; private set; }

    }
}
