using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject
{
    public class CircleWithMaskSegment : Segment
    {
        public Vector2 Center;
        public float Radius;
        public Color Color;
        public float Z;
        public Texture2D Texture;
        public float Scale;
        public Vector2 Offset;
        public Color ClearColor;

        public CircleWithMaskSegment(float centerX, float centerY, float radius, Color color, float z, Texture2D texture, float scale, Vector2 offset, Color clearColor)
        {
            this.Center = new Vector2(centerX, centerY);
            this.Radius = radius;
            this.Color = color;
            this.Z = z;
            this.Texture = texture;
            this.Scale = scale;
            this.Offset = offset;
            this.ClearColor = clearColor;
        }
    }
}