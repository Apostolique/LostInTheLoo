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
        public Action<UnidentifiedBlob1Entity, GameTime> State;
        public bool IsDying;
        public float Scale = 1.0f;
        public StaticFoodEntity TargetFood;
        public double NextFoodScan;
        public Vector3 TargetPosition;
        public float DistanceToTargetPositon;
        public double DeathFromStarvationTime;
        public double DeathTime;
        public double DigestTimer;

        public virtual float Radius1
        {
            get;
            set;
        }

        public virtual float Radius2
        {
            get;
            set;
        }
    }
}