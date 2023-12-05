using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GameProject
{
    public static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach(var element in source)
            {
                action(element);
            }
        }

        public static Vector2 ToVector2XY(this Vector3 vector)
        {
            var result = new Vector2(vector.X, vector.Y);
            return result;
        }

        public static Vector3 NextVector3(this Random random, Vector3 min, Vector3 max)
        {
            var x = random.NextSingle(min.X, max.X);
            var y = random.NextSingle(min.Y, max.Y);
            var z = random.NextSingle(min.Z, max.Z);
            var result = new Vector3(x, y, z);
            return result;
        }

        public static Vector3 ToVector3XY(this Vector2 vector, float z)
        {
            var result = new Vector3(vector.X, vector.Y, z);
            return result;
        }

        public static Vector3 ToVector3XY(this MonoGame.Extended.Point2 vector, float z)
        {
            var result = new Vector3(vector.X, vector.Y, z);
            return result;
        }

        /// <summary>
        /// Due to the nature of single and we want it to perform, the return value might be slightly out of range from requested min/max.
        /// </summary>
        public static float NextSingle(this Random random, float min, float max)
        {
            var result = random.NextSingle() * (max - min) + min;
            return result;
        }

        public static Vector3 NextVector3FromRadius(this Random random, Vector3 center, float radius)
        {
            var rotation = random.NextSingle(-MathHelper.Pi, MathHelper.Pi);
            var x = MathF.Cos(rotation);
            var y = MathF.Sin(rotation);
            var vector = new Vector3(x, y, 0.0f);
            vector.Normalize();
            var result = center + vector * radius;
            return result;
        }
    }
}