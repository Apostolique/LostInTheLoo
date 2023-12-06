using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GameProject
{
    public class TinyPetEntity : MatrixEntity
    {
        public TinyPetDefinition Definition;
        public double NextPoopTime;
        public Action<TinyPetEntity, GameTime> State;
        public bool IsDying;
        public float Scale = 1.0f;
        public StaticFoodEntity TargetFood;
        public double NextFoodScan;
        public Vector3 TargetPosition;
        public float DistanceToTargetPositon;
        public double DeathFromStarvationTime;
        public double DeathTime;
        public double DigestTimer;
        public float RandomMovementSpeedMultiplier = 1.0f;
        public double NextRandomMovementSpeedMultiplierChange;
        public float TotalMovementSpeedMultiplier = 1.0f;
        public float SinusMovementSpeedMultiplier;
        public double SinusMovementSpeedMultiplierSpeed;
        public double SinusMovementSpeedMultiplierOffset;
        public double SinusMovementSpeedMultiplierScale;
        public float CourseDiviationSpeed;
        public EllipseSegment Segment;
        public TinyPetEntity Twin;
        public double SplitTimer;
        public float SplitScaleChangePerSecond;
        public float TargetRotation;

        public float Radius1 { get => this.Segment.Radius1; set => this.Segment.Radius1 = value; }
        public float Radius2 { get => this.Segment.Radius2; set => this.Segment.Radius2 = value; }
    }
}