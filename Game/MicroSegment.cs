using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace GameProject {
    public class MicroSegment : Segment {
        public MicroSegment(Batch.MicroShapes shape, Batch.MicroRamps ramp, float centerX, float centerY, float size, float rotation, Color color, float z, float begin, float end) {
            Shape = shape;
            Ramp = ramp;
            this.Center = new Vector2(centerX, centerY);
            this.Size = size;
            this.Rotation = rotation;
            this.Color = color;
            this.Z = z;
            CoreBlendBegin = begin;
            CoreBlendEnd = end;
        }

        public Batch.MicroShapes Shape;
        public Batch.MicroRamps Ramp;
        public Vector2 Center;
        public float Size;
        public float Rotation;
        public Color Color;
        public float Z;
        public float CoreBlendBegin = 0.8f;
        public float CoreBlendEnd = 1.0f;

        public static Vector2 Rotate(Vector2 a, Vector2 origin, float rotation) {
            return new Vector2(origin.X + (a.X - origin.X) * MathF.Cos(rotation) - (a.Y - origin.Y) * MathF.Sin(rotation), origin.Y + (a.X - origin.X) * MathF.Sin(rotation) + (a.Y - origin.Y) * MathF.Cos(rotation));
        }
        public static RectangleF GetAABB(Vector2 xy, float size, float rotation = 0f) {
            var half = new Vector2(size / 2f, size / 2f);
            var xy1 = xy - half;
            var topLeft = xy1;
            var topRight = xy1 + new Vector2(size, 0);
            var bottomRight = xy1 + new Vector2(size, size);
            var bottomLeft = xy1 + new Vector2(0, size);

            if (rotation != 0f) {
                Vector2 center = xy;
                topLeft = Rotate(topLeft, center, rotation);
                topRight = Rotate(topRight, center, rotation);
                bottomRight = Rotate(bottomRight, center, rotation);
                bottomLeft = Rotate(bottomLeft, center, rotation);
            }

            var realTopLeft = Vector2.Min(Vector2.Min(Vector2.Min(topLeft, topRight), bottomRight), bottomLeft);
            var realBottomRight = Vector2.Max(Vector2.Max(Vector2.Max(topLeft, topRight), bottomRight), bottomLeft);

            return new RectangleF(realTopLeft, realBottomRight - realTopLeft);
        }
    }
}
