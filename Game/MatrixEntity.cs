using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GameProject
{
    public class MatrixEntity : Entity
    {
        public Matrix LocalWorld;
        public Vector3 LocalPosition;
        public float LocalRotationSpin;
        public float LocalRotationOrbit;
        public Vector3 LocalScale = Vector3.One;
        public MatrixEntity Parent;
        public readonly List<MatrixEntity> Children = new List<MatrixEntity>();
        public Matrix AbsoluteWorld; // updated by UpdateAbsolute
        public Vector3 AbsolutePosition; // updated by UpdateAbsolute
        public float AbsoluteRotation;
        public Vector3 AbsoluteScale;
    }
}