using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GameProject
{
    public class UnidentifiedBlob1Entity : MatrixEntity
    {
        public const float MinPoopTimeDelay = 5;
        public const float MaxPoopTimeDelay = 10;

        public Vector3 TargetPosition;
        public float CourseDiviationSpeed;
        public EllipseSegment Segment;
        public float MovementSpeedMultiplier = 1.0f;
        public double NextMovementSpeedMultiplierChange;
        public StaticFoodEntity TargetFood;
        public double NextFoodScan;
        public double DeathFromStarvationTime;
        public Action<UnidentifiedBlob1Entity, GameTime> State;
        public double DigestTimer;
        public UnidentifiedBlob1Entity Twin;
        public double SplitTimer;
        public float SplitRadius1ChangePerSecond;
        public float SplitRadius2ChangePerSecond;
        public float TargetRotation;
        public double NextPoopTime;
    }
}
