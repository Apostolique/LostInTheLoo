using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GameProject
{
    public class UnidentifiedBlob1Entity : MatrixEntity
    {
        public Vector3 TargetPosition;
        public float CourseDiviationSpeed;
        public EllipseSegment Segment;
        public int Leaf;
        public float MovementSpeedMultiplier = 1.0f;
        public double NextMovementSpeedMultiplierChange;
    }
}