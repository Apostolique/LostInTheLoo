using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GameProject
{
    public class LacrymariaOlorEntity : Entity
    {
        public float NeckMaxLength = 75.0f;
        public float NeckLengthCurrent; // -1..1
        public float NeckLengthSpeed = 3.0f;
        public float NeckLengthMultiplier = 10.0f;
        public float HeadRotationCurrent; // 0..1
        public float HeadRotationSpeed = 1.2f;
        public float HeadRotationMultiplier = 1.0f;
        public Vector2 BodyPosition;
        public Vector2 HeadPosition;
    }
}
