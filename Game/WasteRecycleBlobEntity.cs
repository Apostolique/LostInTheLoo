using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GameProject
{
    public class WasteRecycleBlobEntity : MatrixEntity
    {
        public const float MinRadius = 2;
        public const float MaxRadius = 3;
        public const float RoamSpeedMultiplier = 5;
        public const float SeekSpeedMultiplier = 40;
        public const double FoodScanDelay = 5;

        public Vector3 TargetPosition;
        public EllipseSegment Segment;
        public float MovementSpeedMultiplier = 1.0f;
        public StaticFoodEntity TargetFood;
        public double NextFoodScan;
        public double DeathFromStarvationTime;
        public Action<WasteRecycleBlobEntity, GameTime> State;
        public double DigestTimer;
        public float TargetRotation;
        public float Scale;
        public float OriginalRadius1;
        public float OriginalRadius2;
    }
}
