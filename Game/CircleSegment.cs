using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GameProject
{
    public class CircleSegment : Segment
    {
        public Vector2 Center;
        public float Radius;
        public Color Color1;
        public Color Color2;
        public float Thickness;
        public float Z;

        public CircleSegment(float centerX, float centerY, float radius, Color color1, Color color2, float thickness, float z)
        {
            this.Center = new Vector2(centerX, centerY);
            this.Radius = radius;
            this.Color1 = color1;
            this.Color2 = color2;
            this.Thickness = thickness;
            this.Z = z;
        }
    }
}
