using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GameProject
{
    public class EllipseSegment : Segment
    {
        public Vector2 Center;
        public float Radius1;
        public float Radius2;
        public Color Color1;
        public Color Color2;
        public float Thickness;
        public float Roation;
        public float Z;
        public float BokehBlurRadius;
        public float Rotation;

        public EllipseSegment(float centerX, float centerY, float radius1, float radius2, Color color1, Color color2, float thickness, float rotation, float z, float bokehBlurRadius)
        {
            this.Center = new Vector2(centerX, centerY);
            this.Radius1 = radius1;
            this.Radius2 = radius2;
            this.Color1 = color1;
            this.Color2 = color2;
            this.Thickness = thickness;
            this.Roation = rotation;
            this.Z = z;
            this.BokehBlurRadius = bokehBlurRadius;
        }

    }
}