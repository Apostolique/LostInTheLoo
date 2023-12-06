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